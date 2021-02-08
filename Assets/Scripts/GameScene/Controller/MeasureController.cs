using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureController : MonoBehaviour
{
    [SerializeField]
    protected TimeController timeController = null;
    [SerializeField]
    protected MeasureLine measureObj = null;

    protected MeasureLine[] measureLines = null;
    protected int currentElement = 0;

    public IEnumerator Init(ChartStruct chart, Transform laneParent)
    {
        float nextFlameTime = Clac.NextFlameTime();
        int measureLength = Clac.MeasureByTimingCount(chart.chartLength, chart.beat, chart.beatParam, chart.tempoChangers);
        System.Array.Resize(ref measureLines, measureLength);

        for (int i = 0; i < measureLines.Length; i++)
        {
            measureLines[i] = Instantiate(measureObj, laneParent);
            int measureTiming = Clac.TimingCountByMeasure(i, chart.beat, chart.beatParam, chart.tempoChangers);
            measureLines[i].Init(Clac.ShowTimingCount(measureTiming, chart.chartEffecters), measureTiming);
            // 処理時間がフレームレートの80%を超えたら次のフレームへ
            if (Time.realtimeSinceStartup >= nextFlameTime)
            {
                yield return null;
                nextFlameTime = Clac.NextFlameTime();
            }
        }
    }

    public void MoveMeasure(float noteSpeed, ChartStruct chart)
    {
        foreach(MeasureLine measureLine in measureLines)
        {
            measureLine.MovePos(timeController.ShowTimingCount, noteSpeed, timeController.EnableShowCount);
        }
    }
}
