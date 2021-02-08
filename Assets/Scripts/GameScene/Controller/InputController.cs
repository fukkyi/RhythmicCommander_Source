using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public int fastCount = 0;
    public int lateCount = 0;

    [SerializeField]
    private TimeController timeCountroller = null;
    [SerializeField]
    private NotesController notesController = null;
    [SerializeField]
    private ParticleController particleController = null;

    [SerializeField]
    private BeamColorData beamColor = null;    
    [SerializeField]
    private float judgeLineShiny = 1.0f;
    [SerializeField]
    private float autoBarrageInterval = 0.5f;
    [SerializeField]
    private bool isPlayTapSe = true;
    
    private int targetBarrageCount = 0;

    [SerializeField]
    private Animator cookpitCameraAnim = null;
    [SerializeField]
    private Animator judgeLineAnim = null;
    [SerializeField]
    private Animator fastLateAnim = null;
    [SerializeField]
    private Material judgeLineMaterial = null;

    [SerializeField]
    private int[] laneNextCount = null;
    [SerializeField]
    private Transform[] judgeLineTrans = null;
    [SerializeField]
    private LaneBeam[] laneBeams = null;
    [SerializeField]
    private Material[] lineMaterials = null;

    [SerializeField]
    private AudioSource inputSoundsSource = null;

    private NormalNotes[] nextNotes = null; // 要素数はノーツが降ってくるレーンの数

    private string[] inputName = null;
    private float autoBarrageTime = 0;

    public IEnumerator Init(int laneLength)
    {
        float nextFlameTime = Clac.NextFlameTime();

        Array.Resize(ref nextNotes, laneLength);
        Array.Resize(ref laneNextCount, laneLength);
        Array.Resize(ref inputName, laneLength);
        

        for (int i = 0; i < laneLength; i++) {

            // 処理時間がフレームレートの80%を超えたら次のフレームへ
            if (Time.realtimeSinceStartup >= nextFlameTime) {

                yield return null;
                nextFlameTime = Clac.NextFlameTime();
            }

            laneNextCount[i] = 0;
            inputName[i] = Enum.GetName(typeof(InputButton), i);
            SetNextNote(i);
        }
    }

    public void UpdateManage(ref UIDataStruct uiData, RuleStruct gaugeRule, GameObject[] laneObjs, JudgeData timingRange, bool isStarted)
    {
        int currentFlame = timeCountroller.PlayFlame;
        int deltaFlame = timeCountroller.DeltaFlame;

        for (int i = 0; i < laneObjs.Length; i++) {

            NotesType type = NotesType.Normal;
            JudgeType judgeResult = JudgeType.None;

            if (isStarted) {

                type = NotesType.Normal;
                judgeResult = Judge(
                    type: ref type,
                    currentFlame: currentFlame,
                    judgeLane: i,
                    deltaFlame: deltaFlame,
                    judgeData: timingRange
                );

                uiData.barrageCount = targetBarrageCount;
            }

            LaneBeamManage(i);

            if (judgeResult == JudgeType.None) continue;

            uiData.judgeCount[(int)judgeResult]++;
            uiData.scoreCount += timingRange.judges[(int)judgeResult].scoreCount;

            float typeRate = 1.0f;
            if (type == NotesType.Hold) typeRate = gaugeRule.holdParRate;

            if (judgeResult == JudgeType.Miss) {

                float missPenalty = gaugeRule.missPenalty;

                if (gaugeRule.dyingRate >= uiData.gaugePer) missPenalty /= 2;

                uiData.conboCount = 0;
                uiData.gaugePer = Mathf.Clamp01(uiData.gaugePer - missPenalty * typeRate / 100);
            }
            else
            {
                float addPer = gaugeRule.gaugeCount[(int)judgeResult] / (float)uiData.totalGaugeCount * gaugeRule.parRate * typeRate;
                uiData.gaugePer = Mathf.Clamp01(uiData.gaugePer + addPer);

                uiData.conboCount++;
                judgeLineAnim.SetTrigger("Shiny");

                if (uiData.maxConbo < uiData.conboCount)
                {
                    uiData.maxConbo = uiData.conboCount;
                }
            }
        }

        judgeLineMaterial.SetFloat("_Shiny", judgeLineShiny);
    }

    /// <summary>
    /// InputManagerで設定したボタンが押されているかどうか判定する
    /// </summary>
    /// <param name="laneCount"></param>
    /// <returns></returns>
    public bool ButtonDownInput(int laneCount) {

        return Input.GetButtonDown(inputName[laneCount]);
    }

    public bool ButtonInput(int laneCount) {

        return Input.GetButton(inputName[laneCount]);
    }

    /// <summary>
    /// 次に判定するノーツをセットする
    /// </summary>
    /// <param name="judgeLane"></param>
    public void SetNextNote(int judgeLane)
    {        
        nextNotes[judgeLane] = notesController.GetNotes(judgeLane, laneNextCount[judgeLane]);
        laneNextCount[judgeLane]++;
    }

    /// <summary>
    /// ノーツの判定処理
    /// </summary>
    /// <param name="currentFlame"></param>
    /// <param name="judgeLane"></param>
    /// <param name="judgeData"></param>
    public JudgeType Judge(ref NotesType type, int currentFlame, int judgeLane, int deltaFlame, JudgeData judgeData)
    {
        JudgeType judgeResult = JudgeType.None;

        if (nextNotes[judgeLane] == null) {

            if (ButtonDownInput(judgeLane)) SetLaneBeamColor(judgeLane, beamColor.noneColor);
            return judgeResult;
        }
        
        int differenceFlame = nextNotes[judgeLane].TimingFlame - currentFlame;
        int endDifferenceFlame = 0;
        type = nextNotes[judgeLane].Type;

        switch (type) {

            case NotesType.Normal:

                judgeResult = NormalNoteJudge(differenceFlame, judgeLane, judgeData);
                break;

            case NotesType.Hold:

                HoldNotes holdNotes = (HoldNotes)nextNotes[judgeLane];
                endDifferenceFlame = holdNotes.EndTimingFlame - currentFlame;
                judgeResult = HoldNoteJudge(differenceFlame, endDifferenceFlame, judgeLane, deltaFlame, judgeData);
                break;

            case NotesType.Barrage:

                BarrageNotes barrageNotes = (BarrageNotes)nextNotes[judgeLane];
                endDifferenceFlame = barrageNotes.EndTimingFlame - currentFlame;                
                judgeResult = BarrageNoteJudge(differenceFlame, endDifferenceFlame, judgeLane, judgeData);
                break;
        }

        return judgeResult;
    }

    public JudgeType NormalNoteJudge(int differenceFrame, int judgeLane, JudgeData judgeData)
    {
        JudgeType judgeResult = JudgeType.None;
        JudgeStruct[] judges = judgeData.judges;
        bool isPush = false;

        // フレームの差が一番長い判定よりも大きくなったらミスにする
        if (differenceFrame < -judges[judges.Length - 1].flameRange)
        {
            particleController.ShowJudgeText(judgeLineTrans[judgeLane], judgeData.judges[(int)JudgeType.Miss].sprite);
            SetNextNote(judgeLane);
            judgeResult = JudgeType.Miss;
        }

        if (StaticValue.isAuto)
        {
            isPush = differenceFrame <= 0 ? true : false;
        }
        else
        {
            isPush = ButtonDownInput(judgeLane);
        }

        if (isPush)
        {
            for (int i = (int)JudgeType.Just; i < judges.Length; i++)
            {
                if (Mathf.Abs(differenceFrame) <= judges[i].flameRange)
                {
                    //輝くノーツの場合は必ず最高判定にする
                    if (nextNotes[judgeLane].IsShiny)
                    {
                        i = (int)JudgeType.Just;
                        particleController.PlayParticle(judgeLineTrans[judgeLane], 4);
                    }

                    particleController.ShowJudgeText(judgeLineTrans[judgeLane], judges[i].sprite);
                    particleController.PlayParticle(judgeLineTrans[judgeLane], i);
                    SetLaneBeamColor(judgeLane, beamColor.beamColors[i]);
                    nextNotes[judgeLane].JudgeHide();

                    PlayTapSE(NotesType.Normal, judgeLane, nextNotes[judgeLane].IsShiny);

                    MightPlayFastLateAnim(differenceFrame, i);

                    SetNextNote(judgeLane);

                    return (JudgeType)Enum.ToObject(typeof(JudgeType), i);
                }
            }

            SetLaneBeamColor(judgeLane, beamColor.noneColor);
        }

        return judgeResult;
    }

    public JudgeType HoldNoteJudge(int differenceFrame, int endDifferenceFlame, int judgeLane, int deltaFlame, JudgeData judgeData)
    {
        HoldNotes judgeNotes = (HoldNotes)nextNotes[judgeLane];
        JudgeType judgeResult = JudgeType.None;
        JudgeStruct[] judges = judgeData.judges;
        JudgeStruct lastJudge = judges[judgeData.judges.Length - 1];
        bool isPush = false;

        // 終点が判定ラインを超えたか
        if (endDifferenceFlame <= 0)
        {
            // 始点が押されず、始点の判定がされる前に終点が来た場合はミスにする
            if (!judgeNotes.IsPushedJudge)
            {
                particleController.ShowJudgeText(judgeLineTrans[judgeLane], judgeData.judges[(int)JudgeType.Miss].sprite);
                judgeResult = JudgeType.Miss;
                judgeNotes.IsPushedJudge = true;
            }
            else 
            {
                bool isButtonPushed = false;
                if (StaticValue.isAuto) 
                {
                    isButtonPushed = endDifferenceFlame >= 0 ? true : false;
                }
                else 
                {
                    isButtonPushed = ButtonInput(judgeLane);
                }
                judgeResult = judgeNotes.HoldManage(deltaFlame, isButtonPushed, judgeData.judges);

                if (judgeResult != JudgeType.None) 
                {
                    particleController.ShowJudgeText(judgeLineTrans[judgeLane], judgeData.judges[(int)judgeResult].sprite);
                    SetLaneBeamColor(judgeLane, beamColor.beamColors[(int)judgeResult]);

                    if (judgeResult != JudgeType.Miss) 
                    {
                        particleController.PlayParticle(judgeLineTrans[judgeLane], (int)judgeResult);
                    }
                }
            }

            SetNextNote(judgeLane);
            return judgeResult;
        }

        if (differenceFrame <= 0) 
        {
            if (1 < (differenceFrame + deltaFlame)) deltaFlame = Mathf.Abs(differenceFrame);

            bool isButtonPushed = false;
            if (StaticValue.isAuto)
            {
                isButtonPushed = endDifferenceFlame >= 0 ? true : false;
            }
            else 
            {
                isButtonPushed = ButtonInput(judgeLane);
            }

            if (isButtonPushed && judgeNotes.IsPushed)
            {
                judgeNotes.SetLineMaterial(lineMaterials[(int)LineMaterialType.Shiny]);
            }
            else if (differenceFrame < -lastJudge.flameRange)
            {
                judgeNotes.SetLineMaterial(lineMaterials[(int)LineMaterialType.Dark]);
            }
            
            judgeResult = judgeNotes.HoldManage(deltaFlame, isButtonPushed, judgeData.judges);

            if (judgeResult != JudgeType.None) 
            {
                particleController.ShowJudgeText(judgeLineTrans[judgeLane], judgeData.judges[(int)judgeResult].sprite);                
                SetLaneBeamColor(judgeLane, beamColor.beamColors[(int)judgeResult]);

                if (judgeResult != JudgeType.Miss) 
                {
                    particleController.PlayParticle(judgeLineTrans[judgeLane], (int)judgeResult);
                }
            }
        }

        if (StaticValue.isAuto) 
        {
            isPush = differenceFrame <= 0 ? true : false;
        }
        else 
        {
            isPush = ButtonDownInput(judgeLane);
        }

        // 始点の判定の最大範囲が過ぎた場合は始点の判定をミスにする
        if (!judgeNotes.IsPushedJudge && differenceFrame < -lastJudge.flameRange)
        {
            particleController.ShowJudgeText(judgeLineTrans[judgeLane], judgeData.judges[(int)JudgeType.Miss].sprite);
            judgeResult = JudgeType.Miss;
            judgeNotes.IsPushedJudge = true;
        }

        // ボタンを離してから押さないと反応しないように
        if (isPush && !judgeNotes.IsPushed)
        {
            if (differenceFrame < lastJudge.flameRange)
            {
                judgeNotes.IsPushed = true;
                // 始点の判定をする
                if (!judgeNotes.IsPushedJudge)
                {
                    for (int i = (int)JudgeType.Just; i < judges.Length; i++)
                    {
                        if (Mathf.Abs(differenceFrame) <= judges[i].flameRange)
                        {
                            //輝くノーツの場合は必ず最高判定にする
                            if (judgeNotes.IsShiny) 
                            {
                                i = (int)JudgeType.Just;
                                particleController.PlayParticle(judgeLineTrans[judgeLane], 4);
                            }

                            particleController.ShowJudgeText(judgeLineTrans[judgeLane], judges[i].sprite);
                            particleController.PlayParticle(judgeLineTrans[judgeLane], i);

                            SetLaneBeamColor(judgeLane, beamColor.beamColors[i]);

                            MightPlayFastLateAnim(differenceFrame, i);

                            judgeNotes.IsPushedJudge = true;
                            PlayTapSE(NotesType.Hold, judgeLane, judgeNotes.IsShiny);

                            judgeResult = (JudgeType)Enum.ToObject(typeof(JudgeType), i);

                            break;
                        }
                    }
                }
            }
            else
            {
                SetLaneBeamColor(judgeLane, beamColor.noneColor);
            }
        }

        return judgeResult;
    }

    public JudgeType BarrageNoteJudge(int differenceFrame, int endDifferenceFlame, int judgeLane, JudgeData judgeData)
    {
        BarrageNotes judgeNotes = (BarrageNotes)nextNotes[judgeLane];
        JudgeType judgeResult = JudgeType.None;
        int currentCount = judgeNotes.CurrentCount;     

        if (differenceFrame <= 0) 
        {
            bool isPush = false;
            if (targetBarrageCount <= 0)
            {
                targetBarrageCount = currentCount;
                judgeNotes.SetLineMaterial(lineMaterials[(int)LineMaterialType.Shiny]);
            }
            if (StaticValue.isAuto) 
            {
                if (autoBarrageTime <= 0)
                {
                    isPush = true;
                    autoBarrageTime += autoBarrageInterval;
                }
                else 
                {
                    autoBarrageTime -= Time.deltaTime;
                }                
            }
            else 
            {
                isPush = ButtonDownInput(judgeLane);
            }
            if (isPush)
            {
                currentCount--;
                judgeNotes.CurrentCount = currentCount;
                particleController.PlayParticle(judgeLineTrans[judgeLane], 0);
                targetBarrageCount = currentCount;

                PlayTapSE(NotesType.Barrage, 0);
            }
        }

        if (currentCount <= 0)
        {
            judgeResult = JudgeType.Just;
            judgeNotes.SetLineMaterial(lineMaterials[(int)LineMaterialType.Dark]);
            SetNextNote(judgeLane);
            particleController.ShowJudgeText(judgeLineTrans[judgeLane], judgeData.judges[(int)JudgeType.Just].sprite);
            particleController.PlayParticle(judgeLineTrans[judgeLane], (int)judgeResult);

            cookpitCameraAnim.SetTrigger("MashShake");
            PlayTapSE(NotesType.Barrage, -1);

            return judgeResult;
        }
        if (endDifferenceFlame <= 0) 
        {
            SetNextNote(judgeLane);
            judgeResult = JudgeType.Miss;
            particleController.ShowJudgeText(judgeLineTrans[judgeLane], judgeData.judges[(int)JudgeType.Miss].sprite);
            targetBarrageCount = -1;

            return judgeResult;
        }

        return judgeResult;
    }

    public void SetLaneBeamColor(int lane, Color showColor)
    {
        if (lane >= laneBeams.Length) return; 

        laneBeams[lane].SetColor(showColor);
    }

    public void LaneBeamManage(int lane)
    {
        if (lane >= laneBeams.Length) return;

        if (ButtonInput(lane))
        {
            if (laneBeams[lane].isShow) return;

            laneBeams[lane].Show();
        }
        else
        {
            if (!laneBeams[lane].isShow) return;

            laneBeams[lane].Hide();
        }
    }

    private void PlayTapSE(NotesType type, int lane, bool isShiny = false)
    {
        if (!isPlayTapSe) return;

        if (isShiny)
        {
            //Find.GetAudioManager().PlaySE("Accent_1");
            CriAudioManager.Instance.PlaySE("Accent_1");
        }

        if (type == NotesType.Barrage) {

            if (lane == -1) {
                Find.GetAudioManager().PlayOneShotSE("Impact_1");
                //CriAudioManager.Instance.PlaySE("Impact_1");
            }
            else {
                Find.GetAudioManager().PlayOneShotSE("Tap_4");
                //CriAudioManager.Instance.PlaySE("Tap_3");
            }
        }
        else {

            if (lane == 5 || lane == 6) {
                // Find.GetAudioManager().PlayOneShotSE("Tap_3");
                CriAudioManager.Instance.PlaySE("Tap_3");
            }
            else {
                // Find.GetAudioManager().PlayOneShotSE("Tap_5");
                CriAudioManager.Instance.PlaySE("Tap_5");
            }
        }
    }

    private void MightPlayFastLateAnim(int differenceFrame, int judgeType)
    {
        if (judgeType != (int)JudgeType.Just) {

            if (differenceFrame > 0) {
                fastLateAnim.SetTrigger("Fast");
                fastCount++;
            }
            else {
                fastLateAnim.SetTrigger("Late");
                lateCount++;
            }
        }
    }

    public void SetAutoBarrageInterval(float interval)
    {
        autoBarrageInterval = interval;
    }
}
