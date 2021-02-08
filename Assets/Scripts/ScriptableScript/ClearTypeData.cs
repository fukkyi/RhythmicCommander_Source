using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "ClearTypeData", menuName = "ScriptableObject/ClearTypeData")]
public class ClearTypeData : ScriptableObject
{
    public ClearTypeStruct[] clearTypes;

    public void setColor(ref TextMeshProUGUI text, ClearType clearType)
    {
        text.color = clearTypes[(int)clearType].clearColor;
        text.colorGradientPreset = clearTypes[(int)clearType].colorGradient;
    }
}

[System.Serializable]
public struct ClearTypeStruct {

    public string clearName;
    public string displayName;
    public Color clearColor;
    public TMP_ColorGradient colorGradient;
}