[System.Serializable]
public class ScoreDataStruct
{
    public ScoreStruct[] scores = new ScoreStruct[5];
}

[System.Serializable]
public class ScoreStruct 
{
    /// <summary>
    /// ベストスコア
    /// </summary>
    public int bestScore = 0;

    /// <summary>
    /// クリア状況
    /// </summary>
    public ClearType clearType = 0;

    /// <summary>
    /// プレイ回数
    /// </summary>
    public int playCount = 0;

    /// <summary>
    /// クリア回数
    /// </summary>
    public int clearCount = 0;

    /// <summary>
    /// ランキング
    /// </summary>
    public RankingStruct[] scoreRanking = null;
}

[System.Serializable]
public class RankingStruct
{
    public string name = "Guest";
    public int score = 0;
    public ClearType clearType = ClearType.NoPlay;

    public RankingStruct(string _name, int _score, ClearType _clearType)
    {
        name = _name;
        score = _score;
        clearType = _clearType;
    }
}
