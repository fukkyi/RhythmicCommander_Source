[System.Serializable]
public class TempoChangerStruct
{
    /// <summary>
    /// 変化させるタイミング
    /// </summary>
    public int timing;

    /// <summary>
    /// 変化後のBPM
    /// </summary>
    public float bpm;

    /// <summary>
    /// 変化後の親拍子
    /// </summary>
    public int beatParam;

    /// <summary>
    /// 変化後の子拍子
    /// </summary>
    public int beat;
}
