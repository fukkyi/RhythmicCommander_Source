using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMeasureController : MeasureController
{
    [SerializeField]
    private ChartDataController chartDataController = null;
    [SerializeField]
    private CreateController createController = null;
    [SerializeField]
    private CreateNotesController notesController = null;

    public void Init()
    {
        ChartStruct chart = chartDataController.GetSelectingChart();
        int measureLength = Clac.MeasureByTimingCount(chart.chartLength, chart.beat, chart.beatParam, chart.tempoChangers);
        System.Array.Resize(ref measureLines, measureLength);

        for (int i = 0; i < measureLines.Length; i++)
        {
            measureLines[i] = Instantiate(measureObj, createController.lanePrarent);
            int measureTiming = Clac.TimingCountByMeasure(i, chart.beat, chart.beatParam, chart.tempoChangers);
            measureLines[i].Init(Clac.ShowTimingCount(measureTiming, chart.chartEffecters), measureTiming);
        }
    }

    public void RemoveAllMeasure()
    {
        foreach (MeasureLine measureLine in measureLines) {
            Destroy(measureLine.gameObject);
        }
    }

    public void OnChangeChart()
    {
        RemoveAllMeasure();
        Init();
        MoveMeasure();
    }

    public void MoveMeasure()
    {
        ChartStruct chart = chartDataController.GetSelectingChart();
        MoveMeasure(notesController.noteSpeed, chart);
    }
}
