using UnityEngine;

public class HoldNotes : LongNotes
{
    private int pushFlameCount = 0;
    private int totalFlameCount = 0;
    private int totalJudgeCount = 0;
    private int judgeCount = 0;
    private bool isFirstjudge = true;

    public bool IsPushed { set; get; } = false;
    public bool IsPushedJudge { set; get; } = false;

    protected new void Awake()
    {
        base.Awake();
        Type = NotesType.Hold;
        lineTrans.gameObject.AddComponent<BoxCollider>().isTrigger = true;
    }

    public override void Init(NotesStruct notesStruct, ChartStruct chartStruct, Material shinyMaterial = null)
    {
        base.Init(notesStruct, chartStruct, shinyMaterial);
        totalJudgeCount = (EndTimingFlame - TimingFlame) / Constant.holdJudgeFlame;
    }

    public JudgeType HoldManage(int deltaFlame, bool isPushing, JudgeStruct[] judges)
    {
        // -1 判定無し
        JudgeType judgeType = JudgeType.None;

        if (judgeCount >= totalJudgeCount) return judgeType;

        totalFlameCount += deltaFlame;

        if (isPushing && IsPushed) {

            pushFlameCount += deltaFlame;
        }

        if (totalFlameCount >= Constant.holdJudgeFlame) {

            judgeType = JudgeType.Miss;

            // 最初の判定のみ、1フレームでも押されている場合はJustにする
            if (isFirstjudge) {

                if (pushFlameCount >= 1) judgeType = JudgeType.Just;
                isFirstjudge = false;
            }
            else {

                for (int i = (int)JudgeType.Just; i < judges.Length; i++) {

                    if (totalFlameCount - pushFlameCount <= judges[i].flameRange) {

                        judgeType = (JudgeType)System.Enum.ToObject(typeof(JudgeType), i);
                        break;
                    }
                }
            }

            judgeCount++;
            totalFlameCount = Mathf.Clamp(totalFlameCount - Constant.holdJudgeFlame, 0, int.MaxValue);
            pushFlameCount = Mathf.Clamp(pushFlameCount - Constant.holdJudgeFlame, 0, int.MaxValue);                       
        }

        return judgeType;
    }

    public override int ClacComboCount()
    {
        int holdCount = (EndTimingFlame - TimingFlame) / Constant.holdJudgeFlame + 1;
        return holdCount;
    }

    public override int ClacGaugeCount(RuleStruct gaugeRule)
    {
        int comboCount = ClacComboCount();
        return (int)(gaugeRule.gaugeCount[(int)JudgeType.Just] * gaugeRule.holdParRate * comboCount);
    }
}
