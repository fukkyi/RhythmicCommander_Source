using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyData", menuName = "ScriptableObject/DifficultyData")]
public class DifficultyData : ScriptableObject
{
    public DifficltyStruct[] difficlties;
}

[System.Serializable]
public struct DifficltyStruct {

    public Color color;
    public string abbreviation;
}

public enum Difficulty {
    Beginner,
    Standard,
    Advanced,
    Extreme,
    Another,
}
