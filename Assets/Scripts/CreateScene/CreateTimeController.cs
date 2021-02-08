using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateTimeController : TimeController
{
    private bool isPlaying = false;
    private bool isSnap = false;
    private bool needsClacCount = true;

    private float _timeLength = 0;
    public float timeLength { get { return _timeLength; } }

    [SerializeField]
    private ChartDataController chartData = null;
    [SerializeField]
    private CreateMeasureController measureController = null;
    [SerializeField]
    private CreateMusicController musicController = null;
    [SerializeField]
    private CreateNotesController notesController = null;

    [SerializeField]
    private Slider timeSlider = null;
    [SerializeField]
    private Toggle snapToggle = null;
    [SerializeField]
    private TMP_Dropdown intervalDD = null;

    [SerializeField]
    private TextMeshProUGUI timeText = null;
    [SerializeField]
    private TextMeshProUGUI countText = null;
    [SerializeField]
    private TextMeshProUGUI showCountText = null;
    [SerializeField]
    private TextMeshProUGUI measureCountText = null;
    [SerializeField]
    private TextMeshProUGUI flameCountText = null;

    [SerializeField]
    private GameObject stopImage = null;
    [SerializeField]
    private GameObject playImage = null;

    [SerializeField]
    private int[] snapIntervalList = null;
    private int currentSnapInterval = 0;
    private int minTimingCount = 0;

    public void Init()
    {
        flameRate = Application.targetFrameRate;

        ChartStruct chart = chartData.GetSelectingChart();

        _playTimingCount = Clac.TimingCount(
            time: _playTime - chart.musicOffset,
            oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
            bpm: chart.bpm,
            nowBpm: ref _nowBpm,
            tempoChanger: chart.tempoChangers
        );

        SetSnap(snapToggle.isOn);
        SetSnapInterval(intervalDD.value);
        SetTimeLength();
        SetTimingCount(_playTime);
    }

    public void OnChangeTimeSlide()
    {
        SetTimingCount(timeSlider.value);
        measureController.MoveMeasure();
        measureCountText.SetText(MeasureCount.ToString());
        notesController.RefreshNotes(isPlaying);

        if (isPlaying) return;

        _playTime = timeSlider.value;
        FlameManage();
    }

    public void SetTimingCount(float time)
    {
        ChartStruct chart = chartData.GetSelectingChart();

        if (needsClacCount) { 

            _playTimingCount = Clac.TimingCount(
                time: time - chart.musicOffset,
                oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
                bpm: chart.bpm,
                nowBpm: ref _nowBpm,
                tempoChanger: chart.tempoChangers
            );
            _showTimingCount = Clac.ShowTimingCount(_playTimingCount, chart.chartEffecters);
            _measureCount = Clac.MeasureByTimingCount(_playTimingCount, chart.beat, chart.beatParam, chart.tempoChangers);
        }
        else {
            needsClacCount = true;
        }

        countText.SetText(_playTimingCount.ToString());
        showCountText.SetText(_showTimingCount.ToString());
        timeText.SetText("{0:3}", time);
    }

    public void SetTimeLength()
    {
        ChartStruct chart = chartData.GetSelectingChart();

        _timeLength = Clac.TimeByTimingCount(
            count: chart.chartLength,
            oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
            musicOffset: chart.musicOffset,
            bpm: chart.bpm,
            tempoChanger: chart.tempoChangers
        );

        minTimingCount = Clac.TimingCount(
                time: 0 - chart.musicOffset,
                oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
                bpm: chart.bpm,
                nowBpm: ref _nowBpm,
                tempoChanger: chart.tempoChangers
        );

        timeSlider.minValue = 0;
        timeSlider.maxValue = timeLength;
    }

    public void UpdateTime()
    {
        if (isPlaying) {

            ChartStruct chart = chartData.GetSelectingChart();

            _playTime += Time.deltaTime;
            FlameManage();

            _playTimingCount = Clac.TimingCount(
                time: _playTime - chart.musicOffset,
                oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
                bpm: chart.bpm,
                nowBpm: ref _nowBpm,
                tempoChanger: chart.tempoChangers
            );

            if (_playTimingCount > chart.chartLength) {

                _playTimingCount = chart.chartLength;
                SetTimePlay(false);
            }

            _measureCount = Clac.MeasureByTimingCount(_playTimingCount, chart.beat, chart.beatParam, chart.tempoChangers);

            timeSlider.value = _playTime;
        }
        else {

            UpdateTimeFromInput();
        }
    }

    private void FlameManage()
    {
        _playFlame = (int)(_playTime * flameRate);
        _deltaFlame = _playFlame - beforeFlame;
        beforeFlame = _playFlame;

        flameCountText.SetText(_playFlame.ToString());
    }

    public void SetTimePlay(bool play)
    {
        if (isPlaying == play) return;

        isPlaying = !play;
        ToggleTimePlay();
    }

    public void ToggleTimePlay()
    {
        if (isPlaying) {

            isPlaying = false;
            musicController.audioSource.Stop();

            notesController.ResetNotesClap();

            playImage.SetActive(true);
            stopImage.SetActive(false);
        }
        else {

            isPlaying = true;
            musicController.PlayMusic(_playTime);

            notesController.playTiming = PlayTimingCount;

            playImage.SetActive(false);
            stopImage.SetActive(true);
        }
    }

    public void SetSnapInterval(int value)
    {
        currentSnapInterval = snapIntervalList[value];
    }

    public void SetSnap(bool isOn)
    {
        isSnap = isOn;
    }

    private void UpdateTimeFromInput()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheels");

        if (mouseScroll == 0) return;

        int addValue = 1;

        if (isSnap && currentSnapInterval != 0) {

            addValue = Constant.oneMeasureCount / currentSnapInterval;

            if (_playTimingCount % addValue != 0) {

                _playTimingCount = (_playTimingCount / addValue) * addValue;
            }
        }

        if (mouseScroll > 0)
        {
            _playTimingCount += addValue;            
        }
        else if (mouseScroll < 0)
        {
            _playTimingCount -= addValue;
        }

        ChartStruct chart = chartData.GetSelectingChart();
        _playTimingCount = Mathf.Clamp(_playTimingCount, minTimingCount, chart.chartLength);
        _showTimingCount = Clac.ShowTimingCount(_playTimingCount, chart.chartEffecters);

        _playTime = Clac.TimeByTimingCount(
            count: _playTimingCount,
            oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
            musicOffset: chart.musicOffset,
            bpm: chart.bpm,
            tempoChanger: chart.tempoChangers
        );

        _measureCount = Clac.MeasureByTimingCount(_playTimingCount, chart.beat, chart.beatParam, chart.tempoChangers);

        needsClacCount = false;
        timeSlider.value = _playTime;
    }

    public void SetTime(float time)
    {
        _playTime = time;
        timeSlider.value = time;
    }

    public void SetEnableShowCount(bool enable)
    {
        _enableShowCount = enable;
        notesController.RefreshNotes();
        measureController.MoveMeasure();
    }
}
