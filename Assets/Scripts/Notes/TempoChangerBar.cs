using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TempoChangerBar : NotesObject
{
    public int number = 0;

    public int TimingCount { get { return timingCount; } }

    [SerializeField]
    private TextMeshPro bpmText = null;
    [SerializeField]
    private TextMeshPro beatText = null;

    private TempoChangerStruct changerStruct = new TempoChangerStruct();

    protected new void Awake()
    {
        base.Awake();
        notesTrans = transform;
    }

    public void Init(int timing, float bpm, int beat, int beatParam, ChartStruct chartStruct)
    {
        timingCount = timing;
        showTimingCount = Clac.ShowTimingCount(timingCount, chartStruct.chartEffecters);
        bpmText.SetText("{0:0}", bpm);
        beatText.SetText(beat.ToString() + "/" + beatParam.ToString());

        changerStruct.timing = timing;
        changerStruct.bpm = bpm;
        changerStruct.beat = beat;
        changerStruct.beatParam = beatParam;
    }

    public void RefreshCount(ChartStruct chartStruct)
    {
        showTimingCount = Clac.ShowTimingCount(timingCount, chartStruct.chartEffecters);
    }

    public TempoChangerStruct GetChangerStruct()
    {
        return changerStruct;
    }
}
