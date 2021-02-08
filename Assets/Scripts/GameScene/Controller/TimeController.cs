using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    protected int flameRate = 60;
    [SerializeField]
    protected int _deltaFlame = 0;
    protected int beforeFlame = 0;

    [SerializeField]
    protected float _playTime = 0;
    [SerializeField]
    protected int _playFlame = 0;
    [SerializeField]
    protected int _playTimingCount = 0;
    [SerializeField]
    protected int _showTimingCount = 0;
    [SerializeField]
    protected int _measureCount = 0;

    protected float _nowBpm = 0;
    protected int _nowBeat = 0;

    [SerializeField]
    protected bool _enableShowCount = true;
    [SerializeField]
    protected float _startMusicOffset = -1.0f;

    public int DeltaFlame { get { return _deltaFlame; } }
    public float PlayTime { get { return _playTime; } }
    public int PlayFlame { get { return _playFlame; } }
    public int PlayTimingCount { get { return _playTimingCount; } }
    public int ShowTimingCount
    {
        get 
        {
            return _enableShowCount ? _showTimingCount : _playTimingCount;
        } 
    }
    public float NowBpm { get { return _nowBpm; } }
    public int NowBeat { get { return _nowBeat; } }
    public float StartMusicOffset { get { return _startMusicOffset; } }
    public int MeasureCount { get { return _measureCount; } }
    public bool EnableShowCount { get { return _enableShowCount; } }

    public void Init(ChartStruct chart)
    {
        flameRate = Application.targetFrameRate;
        _playTime = _startMusicOffset;

        _playTimingCount = Clac.TimingCountAndBeat(
            time: _playTime - chart.musicOffset,
            oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
            bpm: chart.bpm,
            nowBpm: ref _nowBpm,
            nowBeat: ref _nowBeat,
            tempoChanger: chart.tempoChangers
        );

        _showTimingCount = Clac.ShowTimingCount(
            timingCount: _playTimingCount,
            chartEffecters: chart.chartEffecters
        );
    }

    public void UpdateTime(ChartStruct chart)
    {
        _playTime += Time.deltaTime;
        _playFlame = Mathf.RoundToInt(_playTime * flameRate) + StaticValue.judgeOffset;
        _deltaFlame = _playFlame - beforeFlame;
        beforeFlame = _playFlame;

        _playTimingCount = Clac.TimingCountAndBeat(
            time: _playTime - chart.musicOffset,
            oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
            bpm: chart.bpm,
            nowBpm: ref _nowBpm,
            nowBeat: ref _nowBeat,
            tempoChanger: chart.tempoChangers
        );

        _showTimingCount = Clac.ShowTimingCount(
            timingCount: _playTimingCount,
            chartEffecters: chart.chartEffecters
        );

        _measureCount = Clac.MeasureByTimingCount(_playTimingCount, chart.beat, chart.beatParam, chart.tempoChangers);
    }
}
