using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNotes : NormalNotes
{
    public int EndTimingFlame { get; protected set; } = 0;

    protected SpriteRenderer lineSpriteRenderer = null;

    protected Transform endNotesTrans = null;
    protected Transform lineTrans = null;

    protected Vector3 endPos = Vector3.zero;
    protected Vector3 lineScale = Vector3.one;

    protected int endTiming = 0;
    protected int endShowTimingCount = 0;
    protected float endSpeed = 1.0f;

    protected new void Awake()
    {
        notesTrans = transform.GetChild(Constant.startNoteIndex);
        endNotesTrans = transform.GetChild(Constant.endNoteIndex);
        lineTrans = transform.GetChild(Constant.lineIndex);
        spriteRenderer = notesTrans.GetComponent<SpriteRenderer>();
        lineSpriteRenderer = lineTrans.GetComponent<SpriteRenderer>();

        notesTrans.localPosition = inVisiblePos;
        endNotesTrans.localPosition = inVisiblePos;
        lineTrans.localPosition = inVisiblePos;
    }

    public override void Init(NotesStruct notesStruct, ChartStruct chartStruct, Material shinyMaterial = null)
    {
        base.Init(notesStruct, chartStruct, shinyMaterial);

        endTiming = timingCount + notesStruct.length;
        endShowTimingCount = Clac.ShowTimingCount(endTiming, chartStruct.chartEffecters);
        EndTimingFlame = Clac.TimingFlame(endTiming, chartStruct);
        endSpeed = notesStruct.speed;
    }

    public override void MovePos(int timing, float Notespeed, bool enableShowCount = true)
    {
        int startPosCount = enableShowCount ? showTimingCount : timingCount;
        int endPosCount = enableShowCount ? endShowTimingCount : endTiming;

        notesPos.y = (startPosCount - timing) / (float)Constant.showRangeCount * 80 * mySpeed * Notespeed + Constant.judgeLinePosY;
        endPos.y = (endPosCount - timing) / (float)Constant.showRangeCount * 80 * endSpeed * Notespeed + Constant.judgeLinePosY;

        if (notesPos.y < visibleMaxHeight && endPos.y > visibleMinHeight)
        {
            isVisible = true;

            lineScale.y = (endPos.y - notesPos.y) / Constant.oneScaleLineRange;

            notesTrans.localPosition = notesPos;
            endNotesTrans.localPosition = endPos;
            lineTrans.localPosition = notesPos;
            lineTrans.localScale = lineScale;
        }
        else if (isVisible)
        {
            isVisible = false;

            notesTrans.localPosition = inVisiblePos;
            endNotesTrans.localPosition = inVisiblePos;
            lineTrans.localPosition = inVisiblePos;
            lineTrans.localScale = Vector3.zero;
        }
    }

    public override NotesStruct GetNoteData()
    {
        NotesStruct noteData = base.GetNoteData();
        noteData.length = endTiming - timingCount;

        return noteData;
    }

    /// <summary>
    /// ノーツラインのマテリアルを設定する
    /// </summary>
    /// <param name="material"></param>
    public void SetLineMaterial(Material material)
    {
        lineSpriteRenderer.material = material;
    }
}
