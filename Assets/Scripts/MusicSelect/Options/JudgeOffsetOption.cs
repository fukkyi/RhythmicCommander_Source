using UnityEngine;
using TMPro;

public class JudgeOffsetOption : Option
{
    private int offset;

    [SerializeField]
    private string fastDescription = string.Empty;
    [SerializeField]
    private string lateDescription = string.Empty;
    [SerializeField]
    private string normalDescription = string.Empty;

    [SerializeField]
    private TMP_ColorGradient fastColor = null;
    [SerializeField]
    private TMP_ColorGradient lateColor = null;
    [SerializeField]
    private TextMeshProUGUI valueDetailText = null;
    [SerializeField]
    private TextMeshProUGUI descriprionText = null;

    public override void Init()
    {
        offset = StaticValue.judgeOffset;
        string format = formatValueText();
        valueText.SetText(format, offset);
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

            if (up) offset++;
            if (down) offset--;

            int clampOffset = Mathf.Clamp(offset, -20, 20);
            if (clampOffset == offset) Find.GetAudioManager().PlayOneShotSE("ValueMove");
            offset = clampOffset;

            string format = formatValueText();

            StaticValue.judgeOffset = offset;

            valueText.SetText(format, offset);
            ChangeDetail();
        }
    }

    public void ChangeDetail()
    {
        string format = formatValueText();
        valueDetailText.SetText(format, offset);

        if (offset > 0) {

            valueDetailText.colorGradientPreset = fastColor;
            descriprionText.SetText(fastDescription);
        }
        else if (offset < 0) {

            valueDetailText.colorGradientPreset = lateColor;
            descriprionText.SetText(lateDescription);
        }
        else {

            valueDetailText.colorGradientPreset = null;
            descriprionText.SetText(normalDescription);
        }
    }

    public string formatValueText()
    {
        string format;

        if (offset > 0) {

            format = "+{0}";
        }
        else if (offset < 0) {

            format = "{0}";
        }
        else {

            offset = 0;
            format = "{0}";
        }

        return format;
    }
}
