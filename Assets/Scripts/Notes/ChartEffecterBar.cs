using UnityEngine;
using TMPro;

public class ChartEffecterBar : NotesObject
{
    public int number = 0;

    public int TimingCount { get { return timingCount; } }

    [SerializeField]
    private TextMeshPro chartSpeedText = null;

    private ChartEffecterStruct effecterStruct = new ChartEffecterStruct();

    private new void Awake()
    {
        base.Awake();
        notesTrans = transform;
    }

    public void Init(int timing, float chartSpeed, ChartStruct chartStruct)
    {
        timingCount = timing;
        showTimingCount = Clac.ShowTimingCount(timingCount, chartStruct.chartEffecters);
        chartSpeedText.SetText("{0:2}", chartSpeed);

        effecterStruct.timing = timing;
        effecterStruct.chartSpeed = chartSpeed;
    }

    public void RefreshCount(ChartStruct chartStruct)
    {
        showTimingCount = Clac.ShowTimingCount(timingCount, chartStruct.chartEffecters);
    }

    public ChartEffecterStruct GetEffecterStruct()
    {
        return effecterStruct;
    }
}
