using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotesController : MonoBehaviour
{
    [SerializeField]
    protected TimeController timeCountroller = null;
    // 1次元がレーン、2次元がノーツ
    protected NormalNotes[][] notes = null;
    [SerializeField]
    protected NotesObjectData noteObjData = null;
    [SerializeField]
    protected Material shinyMaterial = null;

    protected int _totalCombo = 0;
    protected int _totalGaugeCount = 0;

    public int totalCombo { get { return _totalCombo; } }
    public int totalGaugeCount { get { return _totalGaugeCount; } }

    public void UpdateManage(float noteSpeed)
    {
        if (notes == null) return;

        int showTimingCount = timeCountroller.ShowTimingCount;
        for (int i = 0; i < notes.Length; i++)
        {
            for (int j = 0; j < notes[i].Length; j++) 
            {
                if (notes[i][j] == null) continue;
                UpdateNotes(notes[i][j], showTimingCount, noteSpeed);
            }
        }
    }

    protected virtual void UpdateNotes(NormalNotes notes, int showTiming, float noteSpeed)
    {
        notes.MovePos(showTiming, noteSpeed, timeCountroller.EnableShowCount);
    }

    public NormalNotes GetNotes(int lane, int count)
    {
        if (lane >= notes.Length) return null;
        if (count >= notes[lane].Length) return null;

        return notes[lane][count];
    }

    public IEnumerator GenerateNotes(RuleStruct gaugeRule, ChartStruct chart, GameObject[] laneObjs)
    {
        Array.Resize(ref notes, laneObjs.Length);

        int[] laneNoteCount = new int[notes.Length];

        for (int i = 0; i < notes.Length; i++) {
            Array.Resize(ref notes[i], 1);
            laneNoteCount[i] = 0;
        }

        float nextFlameTime = Clac.NextFlameTime();
        int notesCount = 0;

        foreach (NotesStruct note in chart.notes)
        {
            // 処理時間がフレームレートの80%を超えたら次のフレームへ
            if (Time.realtimeSinceStartup >= nextFlameTime) {

                yield return null;
                nextFlameTime = Clac.NextFlameTime();
            }

            int lane = note.lane - 1;

            Transform noteObj = GenerateNote(note.type, note.lane, laneObjs).transform;
            NormalNotes generateNotes = noteObj.GetComponent<NormalNotes>();
            // 光るノーツならマテリアルを変える
            Material noteMaterial = note.shiny ? shinyMaterial : null;
            generateNotes.Init(note, chart, noteMaterial);

            _totalCombo += generateNotes.ClacComboCount();
            _totalGaugeCount += generateNotes.ClacGaugeCount(gaugeRule);

            Array.Resize(ref notes[lane], laneNoteCount[lane] + 1);
            notes[lane][laneNoteCount[lane]] = generateNotes;

            laneNoteCount[lane]++;
            notesCount++;
        }
    }

    /// <summary>
    /// ノーツのゲームオブジェクトを生成する
    /// </summary>
    /// <param name="type"></param>
    /// <param name="lane"></param>
    /// <param name="laneObjs"></param>
    /// <returns></returns>
    protected GameObject GenerateNote(NotesType type, int lane, GameObject[] laneObjs)
    {
        GameObject notes = null;

        const int blueNotes = 0;
        const int redNotes = 1;
        const int grayNotes = 2;

        switch (type)
        {
            case NotesType.Normal:

                if (lane == Constant.redLane) {
                    notes = noteObjData.normalNoteObj[redNotes];
                }
                else if (lane == Constant.leftGrayLane || lane == Constant.rightGrayLane)  {
                    notes = noteObjData.normalNoteObj[grayNotes];
                }
                else {
                    notes = noteObjData.normalNoteObj[blueNotes];
                }
                
                break;

            case NotesType.Hold:

                if (lane == Constant.redLane) {
                    notes = noteObjData.holdNoteObj[redNotes];
                }
                else if (lane == Constant.leftGrayLane || lane == Constant.rightGrayLane) {
                    notes = noteObjData.holdNoteObj[grayNotes];
                }
                else {
                    notes = noteObjData.holdNoteObj[blueNotes];
                }

                break;

            case NotesType.Barrage:

                notes = noteObjData.barrageObj;
                break;
        }

        GameObject generateObj = Instantiate(notes, laneObjs[lane - 1].transform, false);

        return generateObj;
    }

    public struct chartStatusStruct 
    {
        public int totalScoreCount;
        public int totalConbo;
    }
}
