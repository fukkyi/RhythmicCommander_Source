using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChartInfoUIController : MonoBehaviour
{
    [SerializeField]
    private MusicDataController musicData = null;

    [SerializeField]
    private RankData rankData = null;

    [SerializeField]
    private TextMeshProUGUI[] levelTexts = null;
    [SerializeField]
    private TextMeshProUGUI bpmText = null;
    [SerializeField]
    private TextMeshProUGUI hiScoreText = null;
    [SerializeField]
    private TextMeshProUGUI rankText = null;
    [SerializeField]
    private TextMeshProUGUI clearRateText = null;
    [SerializeField]
    private TextMeshProUGUI clearTypeText = null;

    [SerializeField]
    private RectTransform selectFlame = null;

    public void Init()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        SetLevelText();
        SetBpmText();
        SetScoreText();
        MoveSelectFlame();
    }

    public void SetLevelText()
    {
        float?[] levels = new float?[5];

        if (musicData.selectMusicElement > 0) {

            levels = musicData.queryMusics[musicData.selectMusicElement - 1].levels;
        }

        for (int i = 0; i < levelTexts.Length; i++) {

            if (levels[i] == null) {
                levelTexts[i].SetText("-.--");
            }
            else {
                
                levelTexts[i].SetText("{0:2}", (float)levels[i]);
            }
        }
    }

    public void SetBpmText()
    {
        float[] bpms = null;

        if (musicData.selectMusicElement > 0) {

            bpms = musicData.queryMusics[musicData.selectMusicElement - 1].bpms[(int)musicData.selectingDif];
        }        

        if (bpms == null) {

            bpmText.SetText("BPM ---");
        }
        else { 

            if (bpms.Length == 1) {

                bpmText.SetText("BPM {0:0}", bpms[0]);
            }
            else if (bpms.Length == 2) {

                bpmText.SetText("BPM {0:0}-{1:0}", bpms[0], bpms[1]);
            }
        }
    }

    public void SetScoreText()
    {
        if (musicData.selectMusicElement == 0) {

            hiScoreText.SetText("-------");
            clearRateText.SetText("--.-%");
            clearTypeText.SetText(string.Empty);
            rankText.SetText(string.Empty);
        }
        else {
            MusicInfoStruct musicInfo = musicData.GetSelectingMusicStruct();

            int bestScore = musicInfo.score[(int)musicData.selectingDif];

            hiScoreText.text = bestScore.ToString("D7");

            ClearTypeStruct clearType = musicData.clearTypeData.clearTypes[musicInfo.clearType[(int)musicData.selectingDif]];
            clearTypeText.SetText(clearType.clearName);
            clearTypeText.colorGradientPreset = clearType.colorGradient;
            clearTypeText.color = clearType.clearColor;

            if (musicInfo.clearRate[(int)musicData.selectingDif] == null) {

                clearRateText.SetText("--.-%");
                rankText.SetText(string.Empty);
            }
            else {

                clearRateText.SetText("{0:1}%", (float)musicInfo.clearRate[(int)musicData.selectingDif]);

                RankStruct rank = rankData.findRank(bestScore);
                rankText.SetText(rank.rankAlphabet);
                rankText.colorGradientPreset = rank.colorGradient;
                rankText.color = rank.rankColor;
            }
        }
    }

    public void MoveSelectFlame()
    {
        float posY = levelTexts[(int)musicData.selectingDif].transform.parent.localPosition.y;
        selectFlame.localPosition = new Vector2(selectFlame.localPosition.x, posY);
    }
}
