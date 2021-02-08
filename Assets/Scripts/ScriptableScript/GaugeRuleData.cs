using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GaugeRuleStruct", menuName = "ScriptableObject/GaugeRuleStruct")]
public class GaugeRuleData : ScriptableObject {

    public RuleStruct[] gaugeRules;
}

[System.Serializable]
public struct RuleStruct {
    
    // 配列数は判定の数と同じでなければならない 要素は判定のenumとリンクさせる
    public int[] gaugeCount;

    public float startPar;
    public float clearPar;
    public float missPenalty;
    // ホールドノーツの増分
    public float holdParRate;
    // 何%分貰えるか
    public float parRate;
    // 瀕死時のレート
    public float dyingRate;

    public Color nonClearColor;
    public Color clearColor;
    public Color quoteColor;

    public bool death;
}
