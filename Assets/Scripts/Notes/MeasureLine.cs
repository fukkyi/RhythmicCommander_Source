using UnityEngine;

public class MeasureLine : NotesObject
{
    protected new void Awake()
    {
        base.Awake();
        notesTrans = transform;
    }

    public void Init(int showTiming, int timing)
    {
        timingCount = timing;
        showTimingCount = showTiming;
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}
