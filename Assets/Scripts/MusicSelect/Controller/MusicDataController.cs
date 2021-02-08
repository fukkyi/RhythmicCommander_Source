using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Chart;

public class MusicDataController : MonoBehaviour
{
    private MusicInfoStruct[] _musics = new MusicInfoStruct[0];
    private MusicInfoStruct[] _queryMusics = null;
    private MusicCategory _category = MusicCategory.All;

    [SerializeField]
    private int _selectMusicElement = 0;

    public DifficultyData difficultyData;
    public ClearTypeData clearTypeData;

    public MusicInfoStruct[] musics { get { return _musics; } }
    public MusicInfoStruct[] queryMusics { get { return _queryMusics; } }
    public MusicCategory category { get { return _category; } }
    public int selectMusicElement { set { if (value >= 0) _selectMusicElement = value; } get { return _selectMusicElement; } }

    public Difficulty selectingDif; 

    public IEnumerator Init()
    {
        if (Constant.CompareEnv(GameEnvironment.local)) {

            LoadLocalAllMusic();
        }
        else if (Constant.CompareEnv(GameEnvironment.webGL)) {

            yield return StartCoroutine(LoadAllMusic());
        }

        QueryMusicData(MusicCategory.All);
    }

    private void LoadLocalAllMusic()
    {
        string[] musicFileNames = ChartUtility.GetAllFileNames();

        foreach(string musicFileName in musicFileNames) {

            MusicStruct musicStruct = ChartUtility.LoadMusicData(musicFileName);
            ScoreDataStruct scoreData = ChartUtility.LoadScoreData(musicFileName);

            if (musicStruct == null || !musicStruct.isShowMusicList) continue;

            Array.Resize(ref _musics, _musics.Length + 1);

            _musics[_musics.Length - 1] = makeMusicInfo(musicStruct, scoreData, musicFileName);
        }
    }

    private IEnumerator LoadAllMusic()
    {
        if (StaticValue.cacheStructs == null) {
            yield return StartCoroutine(ChartUtility.LoadMusicDataFromNCMB());
        }

        for (int i = 0; i < StaticValue.cacheStructs.Length; i++) {

            if (StaticValue.cacheStructs[i].musicStruct == null) continue;
            Array.Resize(ref _musics, _musics.Length + 1);
            _musics[_musics.Length - 1] = makeMusicInfo(
                musicStruct: StaticValue.cacheStructs[i].musicStruct,
                scoreData: StaticValue.cacheStructs[i].scoreStruct,
                folderName: StaticValue.cacheStructs[i].musicName
            );
        }
    }

    public void QueryMusicData(MusicCategory category)
    {
        _category = category;

        if (_category == MusicCategory.All) {

            _queryMusics = musics;
        }
        else {
            // query構文
        }
    }

    public MusicInfoStruct makeMusicInfo(MusicStruct musicStruct, ScoreDataStruct scoreData, string folderName)
    {
        MusicInfoStruct musicInfo = new MusicInfoStruct();

        musicInfo.folderName = folderName;
        musicInfo.title = musicStruct.musicName;
        musicInfo.composer = musicStruct.composer;
        musicInfo.previewTime = musicStruct.previewTime;
        musicInfo.musicFileName = musicStruct.musicFileName;
        musicInfo.musicVolume = musicStruct.musicVolume;

        int chartLength = 5;
        musicInfo.levels = new float?[chartLength];
        musicInfo.bpms = new float[chartLength][];
        musicInfo.score = new int[chartLength];
        musicInfo.clearRate = new float?[chartLength];
        musicInfo.clearType = new int[chartLength];
        musicInfo.rankingStructs = new RankingStruct[chartLength][];

        foreach (ChartStruct chart in musicStruct.charts) {

            musicInfo.levels[(int)chart.difficulty] = chart.level;
            musicInfo.bpms[(int)chart.difficulty] = SetInfoBpm(chart.bpm, chart.tempoChangers);
        }

        if (scoreData == null) return musicInfo;

        for (int i = 0; i < scoreData.scores.Length; i++) {

            ScoreStruct score = scoreData.scores[i];

            if (score == null) continue;

            musicInfo.score[i] = score.bestScore;
            musicInfo.rankingStructs[i] = score.scoreRanking;
            musicInfo.clearType[i] = (int)score.clearType;

            if (score.playCount > 0) {
                musicInfo.clearRate[i] = ((float)score.clearCount / score.playCount) * 100;
            }
            else {
                musicInfo.clearRate[i] = null;
            }
        }

        return musicInfo;
    }

    /// <summary>
    /// BPMの最大値と最小値を求めて配列で返す
    /// [0] 最小値 or 変化しないBPM
    /// [1] 最大値
    /// </summary>
    /// <param name="bpm"></param>
    /// <param name="tempoChangers"></param>
    /// <returns></returns>
    public float[] SetInfoBpm(float bpm, TempoChangerStruct[] tempoChangers = null)
    {
        float[] infobpm = new float[1];

        if (tempoChangers == null) {

            infobpm[0] = bpm;            
        }
        else {

            float minBpm = bpm;
            float maxBpm = bpm;

            foreach(TempoChangerStruct tempoChanger in tempoChangers) {                

                minBpm = Mathf.Min(tempoChanger.bpm, minBpm);
                maxBpm = Mathf.Max(tempoChanger.bpm, maxBpm);
            }

            if (Mathf.Abs(minBpm - maxBpm) < Mathf.Epsilon) {

                infobpm[0] = bpm;
            }
            else {

                infobpm = new float[2];
                infobpm[0] = minBpm;
                infobpm[1] = maxBpm;
            }
        }

        return infobpm;
    }

    public MusicInfoStruct GetSelectingMusicStruct()
    {
        if (selectMusicElement == 0) return new MusicInfoStruct();

        return queryMusics[selectMusicElement - 1];
    }
}

public struct MusicInfoStruct {

    public string folderName;
    public string title;
    public string composer;
    public float previewTime;
    public float musicVolume;
    public string musicFileName;
    public float?[] levels;
    public float[][] bpms;
    public int[] score;
    public int[] clearType;
    public float?[] clearRate;
    public RankingStruct[][] rankingStructs;
}
