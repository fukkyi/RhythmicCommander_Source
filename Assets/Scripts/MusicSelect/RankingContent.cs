using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingContent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI rankText = null;
    [SerializeField]
    private TextMeshProUGUI nameText = null;
    [SerializeField]
    private TextMeshProUGUI scoreText = null;
    [SerializeField]
    private TextMeshProUGUI clearText = null;

    public void SetContent(int rank, string name, int score, ClearType clearType, ClearTypeData clearTypeData)
    {
        rankText.text = rank.ToString();
        nameText.SetText(name);
        scoreText.text = score.ToString("D7");
        clearText.text = clearTypeData.clearTypes[(int)clearType].clearName;
        clearText.colorGradientPreset = clearTypeData.clearTypes[(int)clearType].colorGradient;
    }
}
