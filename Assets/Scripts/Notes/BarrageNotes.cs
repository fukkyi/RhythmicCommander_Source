using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageNotes : LongNotes
{
    private int totalCount = 0;

    public int CurrentCount { set; get; } = 0;

    protected new void Awake()
    {
        base.Awake();
        Type = NotesType.Barrage;
        lineTrans.gameObject.AddComponent<BoxCollider>().isTrigger = true;
    }

    public override void Init(NotesStruct notesStruct, ChartStruct chartStruct, Material shinyMaterial = null)
    {
        base.Init(notesStruct, chartStruct, shinyMaterial);

        endTiming = timingCount + notesStruct.length;
        endShowTimingCount = Clac.ShowTimingCount(endTiming, chartStruct.chartEffecters);
        EndTimingFlame = Clac.TimingFlame(endTiming, chartStruct);
        totalCount = notesStruct.count;
        CurrentCount = totalCount;
        endSpeed = notesStruct.speed;
    }

    public override void MovePos(int timing, float Notespeed, bool enableShowCount = true)
    {
        // スケールを伸ばすのではなく、Sprite自体を伸ばす
        base.MovePos(timing, Notespeed, enableShowCount);
        lineTrans.localScale = Vector3.one;
        lineSpriteRenderer.size = Vector2.right * Constant.oneScaleLineRange + Vector2.up * (endPos.y - notesPos.y);
    }

    public override NotesStruct GetNoteData()
    {
        NotesStruct noteData = base.GetNoteData();

        noteData.length = endTiming - timingCount;
        noteData.count = totalCount;

        return noteData;
    }
}