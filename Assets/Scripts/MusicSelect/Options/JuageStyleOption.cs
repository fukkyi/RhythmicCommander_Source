using UnityEngine;
using System;
using TMPro;

public class JuageStyleOption : Option
{
    private JudgeStyle judge;

    [SerializeField]
    private JudgeData[] judgeDatas = null;
    [SerializeField]
    private TextMeshProUGUI justMSecText = null;
    [SerializeField]
    private TextMeshProUGUI goodMSecText = null;
    [SerializeField]
    private TextMeshProUGUI valueNameText = null;

    public override void Init()
    {
        judge = StaticValue.judgeStyle;
        valueText.SetText(judge.ToString());
    }

    public override void Selected()
    {
        detailParent.gameObject.SetActive(true);
        ChangeDetail();
    }

    public override void UnSelected()
    {
        detailParent.gameObject.SetActive(false);
    }

    public override void ChangeValue(bool up, bool down, bool submit)
    {
        if (up || down) {

            int judgeLength = Enum.GetNames(typeof(JudgeStyle)).Length - 1;

            if (up && (int)judge < judgeLength) {

                judge++;
                Find.GetAudioManager().PlaySE("ValueMove");
            }
            if (down && judge > 0) {
                judge--;
                Find.GetAudioManager().PlayOneShotSE("ValueMove");
            }

            StaticValue.judgeStyle = judge;

            valueText.SetText(judge.ToString());
            ChangeDetail();
        }
    }

    public void ChangeDetail()
    {
        JudgeData judgeData = judgeDatas[(int)judge];

        float justMSec = (judgeData.judges[(int)JudgeType.Just].flameRange + 1) * 8.3f;
        float goodMSec = (judgeData.judges[(int)JudgeType.Good].flameRange + 1) * 8.3f;

        justMSecText.SetText("±{0:1} ms", justMSec);
        goodMSecText.SetText("±{0:1} ms", goodMSec);

        valueNameText.SetText(Enum.GetName(typeof(JudgeStyle), judge));
    }
}
