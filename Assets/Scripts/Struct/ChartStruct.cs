[System.Serializable]
public class ChartStruct
{
    /// <summary>
    /// 譜面の難易度
    /// </summary>
    public Difficulty difficulty = 0;

    /// <summary>
    /// 曲のBPM
    /// </summary>
    public float bpm = 150;

    /// <summary>
    /// 拍子(分母)
    /// </summary>
    public int beatParam = 4;

    /// <summary>
    /// 拍子(分子)
    /// </summary>
    public int beat = 4;

    /// <summary>
    /// 譜面のレベル
    /// </summary>
    public float level = 0f;

    /// <summary>
    /// 曲のオフセット
    /// </summary>
    public float musicOffset = 0f;

    /// <summary>
    /// 譜面の長さ(Count形式)
    /// </summary>
    public int chartLength = 9600;

    /// <summary>
    /// 譜面製作者
    /// </summary>
    public string notesDesigner = string.Empty;

    /// <summary>
    /// この難易度の音楽データパス ( "" だとそのまま )
    /// </summary>
    public string uniqueMusicPath = string.Empty;

    /// <summary>
    /// 譜面構造体
    /// </summary>
    public NotesStruct[] notes = null;

    /// <summary>
    /// 途中でテンポが変化する内容の構造体
    /// </summary>
    public TempoChangerStruct[] tempoChangers = null;

    /// <summary>
    /// 譜面全体の速度変化などのエフェクト用構造体
    /// </summary>
    public ChartEffecterStruct[] chartEffecters = null;
}
