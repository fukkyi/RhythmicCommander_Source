[System.Serializable]
public class NotesStruct
{
    /// <summary>
    /// ノーツのタイミング
    /// </summary>
    public int timing = 0;

    /// <summary>
    /// 降らせるレーン
    /// </summary>
    public int lane = 0;

    /// <summary>
    /// ノーツの種類
    /// </summary>
    public NotesType type = 0;

    /// <summary>
    /// ロングノーツの長さ
    /// </summary>
    public int length = 0;

    /// <summary>
    /// 連打可能数
    /// </summary>
    public int count = 0;

    /// <summary>
    /// ノーツのスピード
    /// </summary>
    public float speed = 1.0f;

    /// <summary>
    /// 輝くノーツか
    /// </summary>
    public bool shiny = false;
}
