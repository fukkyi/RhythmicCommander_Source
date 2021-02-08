[System.Serializable]
public class MusicStruct
{
    /// <summary>
    /// 曲名
    /// </summary>
    public string musicName = "music";

    /// <summary>
    /// 作曲者
    /// </summary>
    public string composer = "composer";

    /// <summary>
    /// 音源の名前
    /// </summary>
    public string musicFileName = string.Empty;

    /// <summary>
    /// 曲のプレビュー開始時間
    /// </summary>
    public float previewTime = 0;

    /// <summary>
    /// 音源のボリューム
    /// </summary>
    public float musicVolume = 1.0f;

    /// <summary>
    /// 背景のSkybox
    /// </summary>
    public BackGroundType backGroundType = BackGroundType.Green;

    /// <summary>
    /// 楽曲一覧に表示させるか
    /// </summary>
    public bool isShowMusicList = true;

    /// <summary>
    /// この曲の譜面
    /// </summary>
    public ChartStruct[] charts = null;

    /// <summary>
    /// 楽曲に含まれている譜面難易度を全て取得する
    /// </summary>
    /// <returns></returns>
    public Difficulty[] GetAllDifficult() {

        Difficulty[] difficulties = new Difficulty[charts.Length];

        for(int i = 0; i < charts.Length; i++) {
            difficulties[i] = charts[i].difficulty;
        }

        return difficulties;
    }

    /// <summary>
    /// 楽曲に含まれている譜面レベルを全て取得する
    /// </summary>
    /// <returns></returns>
    public float[] GetAllLevel()
    {
        float[] levels = new float[charts.Length];

        for(int i = 0; i < charts.Length; i++)
        {
            levels[i] = charts[i].level;
        }

        return levels;
    }

    /// <summary>
    /// 指定した難易度以下の難易度をすべて取得する
    /// </summary>
    /// <returns></returns>
    public Difficulty[] GetLowDifficult(Difficulty splitDif = Difficulty.Standard, float splitLevel = 5) {

        Difficulty[] allDifs = GetAllDifficult();
        float[] allLevels = GetAllLevel();
        Difficulty[] lowDifs = new Difficulty[0];

        for(int i = 0; i < allDifs.Length; i++)
        {
            if (allDifs[i] > splitDif) continue;
            if (allLevels[i] >= splitLevel) continue;

            System.Array.Resize(ref lowDifs, lowDifs.Length + 1);
            lowDifs[lowDifs.Length - 1] = allDifs[i];
        }

        return lowDifs;
    }
}
