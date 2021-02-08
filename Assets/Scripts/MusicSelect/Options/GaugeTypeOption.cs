using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GaugeTypeOption : Option {

    private GaugeRule gauge;
    
    [SerializeField]
    private GaugeRuleData gaugeRule;
    [SerializeField]
    private TextMeshProUGUI valueNameText = null;
    [SerializeField]
    private Image nonClearGauge = null;
    [SerializeField]
    private Image clearGauge = null;

    public override void Init()
    {
        gauge = StaticValue.gaugeRule;
        valueText.SetText(gauge.ToString());
    }

    public override void ChangeValue(bool up, bool down, bool submit)
    {
        if (up || down) {

            int gaugeLength = Enum.GetNames(typeof(GaugeRule)).Length - 1;

            if (up && (int)gauge < gaugeLength) {

                gauge++;
                Find.GetAudioManager().PlayOneShotSE("ValueMove");
            }
            if (down && gauge > 0) {

                gauge--;
                Find.GetAudioManager().PlaySE("ValueMove");
            }

            StaticValue.gaugeRule = gauge;

            valueText.SetText(gauge.ToString());
            ChangeGauge();
        }
    }

    public override void Selected()
    {
        detailParent.gameObject.SetActive(true);
        ChangeGauge();
    }

    public override void UnSelected()
    {
        detailParent.gameObject.SetActive(false);
    }

    public void ChangeGauge()
    {
        RuleStruct rule = gaugeRule.gaugeRules[(int)gauge];

        nonClearGauge.color = rule.nonClearColor;
        clearGauge.color = rule.clearColor;
        nonClearGauge.fillAmount = rule.clearPar;

        valueNameText.SetText(Enum.GetName(typeof(GaugeRule), gauge));
    }
}