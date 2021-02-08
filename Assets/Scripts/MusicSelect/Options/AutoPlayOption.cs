using UnityEngine;
using TMPro;

public class AutoPlayOption : Option
{
    private bool isAuto;

    [SerializeField]
    private TextMeshProUGUI valueNameText = null;

    public override void Init()
    {
        isAuto = StaticValue.isAuto;
        SetOptionText();
    }

    public override void Selected()
    {
        detailParent.gameObject.SetActive(true);
    }

    public override void UnSelected()
    {
        detailParent.gameObject.SetActive(false);
    }

    public override void ChangeValue(bool up, bool down, bool submit)
    {
        if (up || down) {

            bool isNextAuto = false;
            if (up) isNextAuto = true;
            //if (down) isNextAuto = false;

            if (isNextAuto != isAuto) Find.GetAudioManager().PlayOneShotSE("ValueMove");
            isAuto = isNextAuto;

            SetOptionText();
            StaticValue.isAuto = isAuto;
        }
    }

    private void SetOptionText()
    {
        if (isAuto) {

            valueText.SetText("Enable");
            valueNameText.SetText("Enable");
        }
        else {
            valueText.SetText("Disable");
            valueNameText.SetText("Disable");
        }
    }
}
