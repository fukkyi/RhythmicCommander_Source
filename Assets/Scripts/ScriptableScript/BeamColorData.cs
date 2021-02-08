using UnityEngine;

[CreateAssetMenu(fileName = "BeamColorData", menuName = "ScriptableObject/BeamColorData")]
public class BeamColorData : ScriptableObject
{
    public Color[] beamColors;
    public Color noneColor;
}
