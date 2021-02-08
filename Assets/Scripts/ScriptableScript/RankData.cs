using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "RankData", menuName = "ScriptableObject/RankData")]
public class RankData : ScriptableObject
{
    public RankStruct[] ranks;

    public RankStruct findRank(int score)
    {
        foreach(RankStruct rank in ranks) {

            if (rank.scoreBorder <= score) {

                return rank;
            }
        }

        return ranks[ranks.Length - 1];
    }

    public void SetRankText(ref TextMeshProUGUI textMesh, RankStruct rankStruct)
    {
        textMesh.SetText(rankStruct.rankAlphabet);
        textMesh.colorGradientPreset = rankStruct.colorGradient;
        textMesh.color = rankStruct.rankColor;
    }

    public void SetRankText(ref TextMeshProUGUI textMesh, int score)
    {
        RankStruct rankStruct = findRank(score);
        SetRankText(ref textMesh, rankStruct);
    }
}

[System.Serializable]
public struct RankStruct {

    public string rankAlphabet;
    public int scoreBorder;
    public Color rankColor;
    public TMP_ColorGradient colorGradient;
}
