using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class MusicListController : MonoBehaviour
{
    [SerializeField]
    private SelectInputController input = null;
    [SerializeField]
    private MusicDataController musicData = null;
    [SerializeField]
    private MusicAudioController musicAudio = null;
    [SerializeField]
    private ChartInfoUIController chartUI = null;
    [SerializeField]
    private RankingController rankingController = null;
    [SerializeField]
    private ControllerInputUI controllerUI = null;
    
    [SerializeField]
    private TextMeshProUGUI InfoMusicTitle = null;
    [SerializeField]
    private TextMeshProUGUI InfoMusicComposer = null;
    [SerializeField]
    private TextMeshProUGUI trackCountText = null;

    [SerializeField]
    private MusicContent insContent = null;
    [SerializeField]
    private BannerUI[] confrimUI = null;
    [SerializeField]
    private AnimationCurve moveCurve = null;
    [SerializeField]
    private Transform insTrans = null;
    [SerializeField]
    private int contentCount = 9;    
    [SerializeField]
    private float contentInterval = 1.8f;
    [SerializeField]
    private float maxMoveTime = 0.4f;
    [SerializeField]
    private float minMoveTime = 0.1f;
    [SerializeField]
    private float decMoveTimeRange = 0.08f;

    [SerializeField]
    private Animator SelectMusicInfoAnim = null;

    private int confrimElement = 0;

    private float moveTime = 0.4f;
    private float currentMoveTime = 0;

    private bool isMoving = false;
    private bool isSelected = false;
    private bool isBacked = false;

    private MusicContent[] contents = null;

    public void Init()
    {
        Array.Resize(ref contents, contentCount);
        int posCount = (contentCount - 1) / 2;

        // MusicContent生成
        for (int i = 0; i < contents.Length; i++) {

            Vector3 pos = new Vector3(0, (posCount - i) * contentInterval, 0);
            Quaternion rot = Quaternion.Euler(0, 0, -insTrans.rotation.eulerAngles.z);

            contents[i] = Instantiate(insContent, insTrans);
            contents[i].transform.localPosition = pos;
            contents[i].transform.localRotation = rot;
            contents[i].contentNum = i;
            contents[i].musicNum = SetMusicDataElement(i);
            contents[i].SetSelectingAnim(contentCount);
            AdjustContentUI(contents[i]);
        }

        trackCountText.SetText("Track " + StaticValue.trackCount.ToString());
        controllerUI.SetMusicSelectTexts();
    }

    public void UpdateManage(bool isOpenOption)
    {
        if (isBacked) return;

        if (!isSelected) {

            SelectMusic();
            MoveContents();
            ChangeDifficulty();
            chekeBackTitle();
        }
        else {

            if (!isOpenOption && !rankingController.isEneble) {

                SelectConfrimUI();
            }
            if (!isOpenOption) {

                ChangeDifficulty();
            }
        }
    }

    /// <summary>
    /// 楽曲リストコンテンツを動かす。
    /// </summary>
    /// <param name="direction"></param>
    public void MoveContents()
    {
        if (isSelected) return;

        if (isMoving) {

            AnimContents();
            return;
        }

        bool upInput = input.GetInput(InputName.ListUp);
        bool downInput = input.GetInput(InputName.ListDown);

        if (upInput || downInput) {

            currentMoveTime = 0;

            NextContentManage(upInput, downInput);
            chartUI.RefreshUI();
            musicAudio.ResetIsWait();

            moveTime = AdjustMoveTime();
            musicAudio.StopMusic();
            Find.GetAudioManager().PlayOneShotSE("ListMove");

            isMoving = true;
        }
        else {

            StartCoroutine(musicAudio.LoadAndPlayMusic());
            moveTime = maxMoveTime;
        }
    }

    public void ChangeDifficulty()
    {
        if (input.GetInputDown(InputName.DifficultyUp)) {

            if (musicData.difficultyData.difficlties.Length - 1 > (int)musicData.selectingDif) {

                musicData.selectingDif++;
                ChengeDifiicultyManage();
            }
        }

        if (input.GetInputDown(InputName.DifficultyDown)) {

            if (musicData.selectingDif > 0) {

                musicData.selectingDif--;
                ChengeDifiicultyManage();
            }
        }
    }

    private void ChengeDifiicultyManage()
    {
        RefreshContentUI();
        chartUI.RefreshUI();

        if (rankingController.isEneble) {

            rankingController.SetRanking(musicData.GetSelectingMusicStruct().rankingStructs[(int)musicData.selectingDif]);
        }

        Find.GetAudioManager().PlayOneShotSE("ValueMove");
    }

    public void NextContentManage(bool upInput, bool downInput)
    {
        foreach (MusicContent content in contents) {

            content.SetNextContentNum(upInput, downInput, contents.Length);
            content.ClacNextPos(contents.Length, contentInterval);

            if (content.contentNum == (contentCount - 1) / 2) {

                musicData.selectMusicElement = content.musicNum;
            }
        }

        foreach (MusicContent content in contents) {

            if (content.contentNum != 0 && content.contentNum != contentCount - 1) continue;

            content.musicNum = SetMusicDataElement(content.contentNum);
            AdjustContentUI(content);
        }
    }

    public void RefreshContentUI()
    {
        foreach (MusicContent content in contents) {

            AdjustContentUI(content);
        }
    }

    public void AdjustContentUI(MusicContent content) {

        if (content.musicNum == 0) {

            content.SetContentUIToCategory();
        }
        else {

            content.SetContentUI(
                musicInfoStruct: musicData.queryMusics[content.musicNum - 1],
                difficulty: musicData.selectingDif,
                levelColor: musicData.difficultyData.difficlties[(int)musicData.selectingDif].color
            );
        }
    }

    public void AnimContents()
    {
        currentMoveTime += Time.deltaTime;

        if (currentMoveTime >= moveTime) {

            currentMoveTime = moveTime;

            isMoving = false;
        }

        foreach (MusicContent content in contents) {

            float basePosY = content.basePos;
            float movePosY = basePosY + (moveCurve.Evaluate(currentMoveTime / moveTime) * (content.nextPos - basePosY));

            content.MovePosFromY(movePosY);
        }
    }

    private float AdjustMoveTime()
    {
        return moveTime = Mathf.Clamp(moveTime - decMoveTimeRange, minMoveTime, maxMoveTime);
    }

    public int SetMusicDataElement(int contentNum)
    {
        int musicDataElement = musicData.selectMusicElement;
        int maxElement = musicData.queryMusics.Length;
        int contentDifference = contentNum - (contentCount - 1) / 2;

        if (contentDifference > 0) {

            for (int i = contentDifference; i > 0; i--) {

                if (musicDataElement >= maxElement) {

                    musicDataElement = 0;
                }
                else {

                    musicDataElement++;
                }
            }
        } else if (contentDifference < 0){

            for (int i = contentDifference; i < 0; i++) {

                if (musicDataElement <= 0) {

                    musicDataElement = maxElement;
                }
                else {

                    musicDataElement--;
                }
            }
        }

        return musicDataElement;
    }

    public void SelectMusic()
    {
        bool isPushSubmit = input.GetInputDown(InputName.Submit);

        if (!isPushSubmit || isMoving || musicData.selectMusicElement == 0) return;

        if (musicData.GetSelectingMusicStruct().levels[(int)musicData.selectingDif] == null) {
            Find.GetAudioManager().PlayOneShotSE("Cancel");
            return;
        }

        if (!isSelected) {

            confrimElement = 0;
            confrimUI[confrimElement].SetSelectingAnim(true);

            MusicInfoStruct selectMusic = musicData.GetSelectingMusicStruct();
            InfoMusicTitle.SetText(selectMusic.title);
            InfoMusicComposer.SetText(selectMusic.composer);

            Find.GetAudioManager().PlayOneShotSE("Confrim_1");
            controllerUI.SetConfrimTexts();

            SetMusicInfoAnim(true);
            isSelected = true;
        }
    }

    public void SetMusicInfoAnim(bool show)
    {
        foreach (MusicContent content in contents) {

            content.PlayHideAnim(show);
        }

        SelectMusicInfoAnim.SetBool("Show", show);
    }

    public void SelectConfrimUI()
    {
        if (SelectMusicInfoAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f) return;

        bool isPushUp = input.GetInputDown(InputName.ListUp);
        bool isPushDown = input.GetInputDown(InputName.ListDown);
        bool isPushSubmit = input.GetInputDown(InputName.Submit);
        bool isPushCancel = input.GetInputDown(InputName.Cancel);

        if (isPushCancel) {
            
            SetMusicInfoAnim(false);
            confrimUI[confrimElement].SetSelectingAnim(false);
            controllerUI.SetMusicSelectTexts();
            isSelected = false;
            Find.GetAudioManager().PlayOneShotSE("Cancel");

            return;
        }

        if (isPushUp) {

            changeConfrimElement(confrimElement - 1);
        }
        if (isPushDown) {

            changeConfrimElement(confrimElement + 1);
        }
        if (isPushSubmit) {

            if (musicData.GetSelectingMusicStruct().levels[(int)musicData.selectingDif] == null) {
                Find.GetAudioManager().PlayOneShotSE("Cancel");
                return;
            }

            confrimUI[confrimElement].Select();
        }
    }

    public void changeConfrimElement(int element)
    {
        if (element < 0) {

            element = 0;
        }
        else if (element >= confrimUI.Length){

            element = confrimUI.Length - 1;
        }
        else {

            Find.GetAudioManager().PlayOneShotSE("ListMove");
        }

        confrimUI[confrimElement].SetSelectingAnim(false);
        confrimElement = element;
        confrimUI[confrimElement].SetSelectingAnim(true);        
    }

    public void chekeBackTitle()
    {
        bool isPushUp = input.GetInputDown(InputName.Exit);

        if (!isPushUp) return;
        bool isWork = Find.GetSceneLoadManager().LoadTitleScene();

        if (!isWork) return;
        Find.GetAudioManager().PlayOneShotSE("Confrim_2");
        isBacked = true;
    }
}
