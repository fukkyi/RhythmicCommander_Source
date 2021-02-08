using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Chart;

public class CreateChartController : MonoBehaviour
{
    public bool isOpen = false;

    [SerializeField]
    private ChartDataController chartDataController = null;
    [SerializeField]
    private CreateNotesController notesController = null;
    [SerializeField]
    private CreateMusicController musicController = null;

    [SerializeField]
    private Animator windowAnim = null;

    [SerializeField]
    private TMP_Dropdown dificultyDD = null;
    [SerializeField]
    private TMP_InputField BpmIF = null;
    [SerializeField]
    private TMP_InputField levelIF = null;
    [SerializeField]
    private TMP_InputField beatIF = null;
    [SerializeField]
    private TMP_InputField beatParamIF = null;
    [SerializeField]
    private TMP_InputField chartLengthIF = null;
    [SerializeField]
    private TMP_InputField musicOffsetIF = null;
    [SerializeField]
    private TMP_InputField notesDesignerIF = null;

    public void Init()
    {
        chartDataController.SetDifficult();
        SetValueToUI();
    }

    public void SetValueToUI()
    {
        ChartStruct chart = chartDataController.GetSelectingChart();

        dificultyDD.value = (int)chartDataController.selectingDif;
        BpmIF.text = chart.bpm.ToString("F2");
        levelIF.text = chart.level.ToString("F2");
        beatIF.text = chart.beat.ToString();
        beatParamIF.text = chart.beatParam.ToString();
        chartLengthIF.text = chart.chartLength.ToString();
        musicOffsetIF.text = chart.musicOffset.ToString("F4");
        notesDesignerIF.text = chart.notesDesigner;
    }

    public void PlayWindowAnim(bool show)
    {
        if (show) SetValueToUI();

        isOpen = show;
        windowAnim.SetBool("Show", show);
    }

    public void SetDifficulty()
    {
        chartDataController.SetNotesStruct();

        int value = dificultyDD.value;
        chartDataController.SetSelectingDifficulty(value);
        chartDataController.SetDifficult();
        SetValueToUI();

        notesController.GenerateNotesFromChart();
    }

    public void SetBpm(string bpm)
    {
        if (bpm == string.Empty) return;
        chartDataController.SetBpm(float.Parse(bpm));
    }

    public void SetLevel(string level)
    {
        if (level == string.Empty) return;
        chartDataController.SetLevel(float.Parse(level));
    }

    public void SetBeat(string beat)
    {
        if (beat == string.Empty) return;
        chartDataController.SetBeat(int.Parse(beat));
    }

    public void SetBeatParam(string beatParam)
    {
        if (beatParam == string.Empty) return;
        chartDataController.SetBeatParam(int.Parse(beatParam));
    }

    public void SetChartLength(string chartLength)
    {
        if (chartLength == string.Empty) return;
        int LengthCount = int.Parse(chartLength);
        chartDataController.SetChartLength(LengthCount);
    }

    public void SetMusicOffset(string offset)
    {
        if (offset == string.Empty) return;
        chartDataController.SetMusicOffset(float.Parse(offset));
    }

    public void SetNotesDesigner(string notesDesigner)
    {
        chartDataController.SetNotesDesigner(notesDesigner);
    }

    public void AdjustChartLength()
    {
        ChartStruct chart = chartDataController.GetSelectingChart();

        float nowBpm = 0;

        int chartLength = Clac.TimingCount(
            time: musicController.audioSource.clip.length,
            oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
            bpm: chart.bpm,
            nowBpm: ref nowBpm,
            tempoChanger: chart.tempoChangers
        );

        chartDataController.SetChartLength(chartLength);
        chartLengthIF.text = chartLength.ToString();
    }
}
