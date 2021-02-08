using System.Windows.Forms;
using UnityEngine;

public class NotesObject : MonoBehaviour
{
    protected Transform notesTrans = null;
    protected Vector3 notesPos = new Vector3(0, 0, 0);

    protected bool isVisible = false;
    protected int timingCount = 0;
    protected int showTimingCount = 0;

    protected float mySpeed = 1.0f;

    protected static readonly float visibleMaxHeight = 90;
    protected static readonly float visibleMinHeight = -5;

    protected Vector3 inVisiblePos = new Vector3(0, -100, 0);

    protected void Awake()
    {
        transform.localPosition = inVisiblePos;
    }

    public virtual void MovePos(int timing, float notesSpeed, bool enableShowCount = true)
    {
        int posCount = enableShowCount ? showTimingCount : timingCount;
        // ノーツの位置を計算する
        notesPos.y = (posCount - timing) / (float)Constant.showRangeCount * 80 * mySpeed * notesSpeed + Constant.judgeLinePosY;
        // 
        if (notesPos.y < visibleMaxHeight && notesPos.y > visibleMinHeight)
        {
            isVisible = true;
            notesTrans.localPosition = notesPos;
        }
        else if (isVisible)
        {
            isVisible = false;
            transform.localPosition = inVisiblePos;
        }
    }
}
