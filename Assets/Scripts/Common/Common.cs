using UnityEngine;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

public static class Constant {

#if UNITY_EDITOR
    public const GameEnvironment env = GameEnvironment.local;
#elif UNITY_STANDALONE_WIN
    public const GameEnvironment env = GameEnvironment.local;
#elif UNITY_STANDALONE_OSX
    public const GameEnvironment env = GameEnvironment.local;
#elif WebPlayer
    public const GameEnvironment env = GameEnvironment.webGL;
#else
    public const GameEnvironment env = GameEnvironment.local;
#endif
    public const string version = "v0.4";

    public const string chartExtension = ".fmn";
    public const string scoreExtension = ".score";
    public const string musicExtension = ".ogg";
    public const string musicDataDirectory = "/MusicData";
    public const string audioResourceDirectory = "AudioData/";
    public const string controllerName = "REVIVE Micro";
    public const AudioType musicAudioType = AudioType.OGGVORBIS;
    public const int oneMeasureCount = 960;
    public const int showRangeCount = 3000;
    public const int generateMeasureCount = 10;
    public const int judgeLinePosY = 4;
    public const int holdJudgeFlame = 10;
    public const int maxScore = 1000000;
    public const int rankingLength = 20;

    public const int startNoteIndex = 0;
    public const int endNoteIndex = 1;
    public const int lineIndex = 2;

    public const int redLane = 3;
    public const int leftGrayLane = 6;
    public const int rightGrayLane = 7;

    public const float oneScaleLineRange = 5.12f;

    public static bool CompareEnv(GameEnvironment env) {
        return Constant.env == env;
    }

    /// <summary>
    /// 現在接続されている入力機器を取得する
    /// </summary>
    /// <returns></returns>
    public static bool IsConnectingController()
    {
        foreach(string joystick in Input.GetJoystickNames()) 
        {
            if (joystick == controllerName) 
            {
                return true;
            }
        }

        return false;
    }
}

public static class Find {

    public static AudioManager GetAudioManager()
    {
        return GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public static SceneLoadManager GetSceneLoadManager()
    {
        GameObject loadManagerObj = GameObject.FindWithTag("SceneLoadManager");
        SceneLoadManager loadManager = null;

        if (loadManagerObj == null) {

            loadManagerObj = Object.Instantiate(new GameObject());
            loadManagerObj.tag = "SceneLoadManager";

            loadManager = loadManagerObj.AddComponent<SceneLoadManager>();
        }
        else {

            loadManager = loadManagerObj.GetComponent<SceneLoadManager>();
        }

        return loadManager;
    }

    public static LoadSceneController GetLoadSceneController()
    {
        return GameObject.FindWithTag("LoadSceneController").GetComponent<LoadSceneController>();
    }

    public static GameController GetGameController()
    {
        return GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
}

public static class MusicPath {

    public static string getMusicDataPath(string directoryName, string musicName) {

        return Application.streamingAssetsPath + Constant.musicDataDirectory + "/" + directoryName + "/" + musicName + Constant.musicExtension;
    }
}

public static class Clac {

    public static int OneBeatCount(int beat, int beatParam)
    {
        return (int)(Constant.oneMeasureCount * ((float)beat / beatParam)) / beat;
    }

    public static float NextFlameTime() {

        return Time.realtimeSinceStartup + (0.8f / Application.targetFrameRate);
    }

    public static int TimingCount(float time, int oneBeatCount, float bpm, ref float nowBpm, TempoChangerStruct[] tempoChanger = null)
    {
        int count = 0;
        float clacTime = 0;

        if (tempoChanger != null) {

            for (int i = 0; i < tempoChanger.Length; i++) {

                float changeTime = (tempoChanger[i].timing - count) / (bpm / 60) / oneBeatCount;

                if (clacTime + changeTime > time) {
                    break;
                }

                oneBeatCount = OneBeatCount(tempoChanger[i].beat, tempoChanger[i].beatParam);
                clacTime += changeTime;
                bpm = tempoChanger[i].bpm;
                count = tempoChanger[i].timing;
            }
        }

        float differenceTime = time - clacTime;
        count += (int)(differenceTime * oneBeatCount * (bpm / 60));
        nowBpm = bpm;

        return count;
    }

    public static int ShowTimingCount(int timingCount, ChartEffecterStruct[] chartEffecters)
    {
        int count = 0;
        int clacCount = 0;
        float chartSpeed = 1.0f;

        if (chartEffecters != null)
        {
            foreach(ChartEffecterStruct chartEffecter in chartEffecters)
            {
                if (chartEffecter.timing > timingCount) break;

                count += (int)((chartEffecter.timing - clacCount) * chartSpeed);
                clacCount = chartEffecter.timing;
                chartSpeed = chartEffecter.chartSpeed;
            }
        }

        count += (int)((timingCount - clacCount) * chartSpeed);

        return count;
    }

    public static int TimingCountAndBeat(float time, int oneBeatCount, float bpm, ref float nowBpm, ref int nowBeat, TempoChangerStruct[] tempoChanger = null)
    {
        int count = 0;
        int beatCount = 0;
        float clacTime = 0;

        if (tempoChanger != null) {

            for (int i = 0; i < tempoChanger.Length; i++) {

                float changeTime = (tempoChanger[i].timing - count) / (bpm / 60) / oneBeatCount;

                if (clacTime + changeTime > time) break;

                oneBeatCount = OneBeatCount(tempoChanger[i].beat, tempoChanger[i].beatParam);
                clacTime += changeTime;
                bpm = tempoChanger[i].bpm;
                count = tempoChanger[i].timing;
                beatCount = count / oneBeatCount;
            }
        }

        float differenceTime = time - clacTime;
        count += (int)(differenceTime * oneBeatCount * (bpm / 60));
        beatCount += count / oneBeatCount;
        nowBpm = bpm;
        nowBeat = beatCount;

        return count;
    }

    public static float TimeByTimingCount(int count, int oneBeatCount, float musicOffset, float bpm, TempoChangerStruct[] tempoChanger = null)
    {
        float time = 0;
        int clacCount = 0;

        if (tempoChanger != null) {

            for (int i = 0; i < tempoChanger.Length; i++) {

                if (tempoChanger[i].timing > count) break;

                time += (tempoChanger[i].timing - clacCount) / (bpm / 60) / oneBeatCount;

                oneBeatCount = OneBeatCount(tempoChanger[i].beat, tempoChanger[i].beatParam);
                clacCount = tempoChanger[i].timing;
                bpm = tempoChanger[i].bpm;
            }
        }

        int differenceCount = count - clacCount;
        time += differenceCount / (bpm / 60) / oneBeatCount + musicOffset;

        return time;
    }

    public static int TimingFlame(int timingCount, ChartStruct chart)
    {
        float time = TimeByTimingCount(
            count: timingCount,
            oneBeatCount: OneBeatCount(chart.beat, chart.beatParam),
            musicOffset: chart.musicOffset,
            bpm: chart.bpm,
            tempoChanger: chart.tempoChangers
        );

        return (int)(time * Application.targetFrameRate);
    }

    public static int TimingCountByMeasure(int measureCount, int beat, int beatParam, TempoChangerStruct[] tempoChangers)
    {
        int count = 0;
        int clacMeasureCount = 0;

        if (tempoChangers != null)
        {
            for (int i = 0; i < tempoChangers.Length; i++)
            {
                int changerMeasureCount = tempoChangers[i].timing / OneMeasureCount(beat, beatParam);
                if (changerMeasureCount > measureCount) break;

                count = tempoChangers[i].timing;
                clacMeasureCount = tempoChangers[i].timing / OneMeasureCount(beat, beatParam);
                beat = tempoChangers[i].beat;
                beatParam = tempoChangers[i].beatParam;
            }
        }

        int difMeasureCount = measureCount - clacMeasureCount;
        count += difMeasureCount * OneMeasureCount(beat, beatParam);

        return count;
    }

    public static int OneMeasureCount(int beat, int beatParam)
    {
        return (int)(Constant.oneMeasureCount * ((float)beat / beatParam));
    }

    public static int MeasureByTimingCount(int timingCount, int beat, int beatParam, TempoChangerStruct[] tempoChangers)
    {
        int measure = 0;
        int clacCount = 0;
        int differenceCount;

        if (tempoChangers != null)
        {
            for (int i = 0; i < tempoChangers.Length; i++)
            {
                if (tempoChangers[i].timing > timingCount) break;

                differenceCount = tempoChangers[i].timing - clacCount;
                measure += differenceCount / OneMeasureCount(beat, beatParam);
                clacCount = tempoChangers[i].timing;
                beat = tempoChangers[i].beat;
                beatParam = tempoChangers[i].beatParam;
            }
        }

        differenceCount = timingCount - clacCount;
        measure += differenceCount / OneMeasureCount(beat, beatParam) + 1;

        return measure;
    }
}

public enum GameEnvironment {
    local,
    webGL
}

public enum JudgeType {
    None = -1,
    Miss,
    Just,
    Good,
    Safe
}

public enum NotesType {
    Normal,
    Hold,
    Barrage,
    TempoChanger,
    ChartEffecter
}

public enum InputButton {
    Lane1,
    Lane2,
    Lane3,
    Lane4,
    Lane5,
    Lane6,
    Lane7,
    Submit,
    Cancel
}

public enum InputName {
    ListUp,
    ListDown,
    DifficultyUp,
    DifficultyDown,
    Submit,
    Cancel,
    Exit
}

public enum GaugeRule {
    Normal,
    Expert
}

public enum JudgeStyle {
    Normal,
    Severe
}

public enum ClearType {
    NoPlay,
    Failed,
    Clear,
    ExClear,
    FullCombo,
    Prefect
}


public enum LineMaterialType {
    Normal,
    Shiny,
    Dark
}

public enum MusicCategory {
    All,
    Temporary
}

public enum BackGroundType {
    Green,
    Blue,
    Red,
    GreenPlanet,
    BluePlanet,
    RedPlanet,
}