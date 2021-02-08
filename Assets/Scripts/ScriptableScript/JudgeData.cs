using UnityEngine;

[CreateAssetMenu(fileName = "JudgeData", menuName = "ScriptableObject/JudgeData")]
public class JudgeData : ScriptableObject {

    public JudgeStruct[] judges;
}

[System.Serializable]
public struct JudgeStruct {

    public Sprite sprite;
    public int flameRange;
    public int scoreCount;
}