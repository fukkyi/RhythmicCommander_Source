using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

//ノーツの親クラス
public class NormalNotes : NotesObject
{
    public bool IsShiny { get; protected set; } = false;
    public int TimingFlame { get; protected set; } = 0;
    public NotesType Type { get; protected set; } = NotesType.Normal;

    protected int lane = 1;
    /// <summary>
    /// ノーツを叩いたときに増えるコンボ数
    /// </summary>
    protected int comboCount = 1;
    protected bool isJudge = false;
    protected bool isPlayedClap = false;

    protected SpriteRenderer spriteRenderer = null;

    protected new void Awake()
    {
        base.Awake();
        Type = NotesType.Normal;
        notesTrans = gameObject.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// ノーツを移動させる
    /// </summary>
    /// <param name="timing"></param>
    /// <param name="speed"></param>
    public override void MovePos(int timing, float notesSpeed, bool enableShowCount = true)
    {
        if (isJudge) return;

        base.MovePos(timing, notesSpeed, enableShowCount);
    }

    public virtual void Init(NotesStruct notesStruct, ChartStruct chartStruct, Material shinyMaterial = null, int noteNumber = 0)
    {
        Init(notesStruct, chartStruct, shinyMaterial);
    }

    public virtual void Init(NotesStruct notesStruct, ChartStruct chartStruct, Material shinyMaterial = null)
    {
        timingCount = notesStruct.timing;
        showTimingCount = Clac.ShowTimingCount(timingCount, chartStruct.chartEffecters);
        TimingFlame = Clac.TimingFlame(timingCount, chartStruct);

        if (notesStruct.lane > 0)
        {
            lane = notesStruct.lane;
        }

        mySpeed = notesStruct.speed;
        IsShiny = notesStruct.shiny;

        if (shinyMaterial != null) spriteRenderer.material = shinyMaterial;
    }

    /// <summary>
    /// ノーツのマテリアルを設定する
    /// </summary>
    /// <param name="material"></param>
    public void SetMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    public virtual void JudgeHide()
    {
        isJudge = true;
        transform.localPosition = inVisiblePos;
    }

    public void ResetPlayedClap()
    {
        isPlayedClap = false;
    }

    public virtual void MightPlayClap(int flame, int playTiming = 0)
    {
        if (isPlayedClap || !isVisible) return;

        if (flame >= TimingFlame && timingCount >= playTiming)
        {
            CriAudioManager.Instance.PlaySE("Clap_1");
            isPlayedClap = true;
        }
    }

    public virtual NotesStruct GetNoteData()
    {
        NotesStruct noteData = new NotesStruct
        {
            timing = timingCount,
            type = Type,
            lane = lane,
            shiny = IsShiny,
            speed = mySpeed
        };

        return noteData;
    }

    public virtual int ClacComboCount()
    {
        return comboCount;
    }

    public virtual int ClacGaugeCount(RuleStruct gaugeRule)
    {
        return gaugeRule.gaugeCount[(int)JudgeType.Just];
    }
}
