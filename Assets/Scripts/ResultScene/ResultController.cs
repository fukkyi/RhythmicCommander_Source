using UnityEngine;
using TMPro;

public class ResultController : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI scoreText = null;
    [SerializeField]
    private TextMeshProUGUI bestScoreText = null;
    [SerializeField]
    private TextMeshProUGUI differenceScoreText = null;
    [SerializeField]
    private TextMeshProUGUI clearText = null;
    [SerializeField]
    private TextMeshProUGUI rankText = null;
    [SerializeField]
    private TextMeshProUGUI[] judgeCountTexts = null;
    [SerializeField]
    private TextMeshProUGUI maxComboText = null;
    [SerializeField]
    private TextMeshProUGUI musicTitleText = null;
    [SerializeField]
    private TextMeshProUGUI composerText = null;
    [SerializeField]
    private TextMeshProUGUI difficultText = null;
    [SerializeField]
    private TextMeshProUGUI levelText = null;
    [SerializeField]
    private TextMeshProUGUI fastCountText = null;
    [SerializeField]
    private TextMeshProUGUI lateCountText = null;
    [SerializeField]
    private TextMeshProUGUI newRecordText = null;

    [SerializeField]
    private RectTransform tweetButton = null;

    [SerializeField]
    private Animator resultAnim = null;

    [SerializeField]
    private RankData rankData = null;
    [SerializeField]
    private ClearTypeData clearType = null;
    [SerializeField]
    private DifficultyData difficultyData = null;

    private ResultData resultStruct = new ResultData();

    public void SetUIValue(ResultData resultData)
    {
        if (Constant.CompareEnv(GameEnvironment.webGL) && !StaticValue.isAuto) {
            tweetButton.gameObject.SetActive(true);
        }

        resultStruct = resultData;

        scoreText.text = resultData.score.ToString("D7");

        bestScoreText.text = resultData.bestScore.ToString("D7");

        SetDifferenceScore(resultData.score, resultData.bestScore);

        rankData.SetRankText(ref rankText, resultData.score);

        maxComboText.text = resultData.maxCombo.ToString();

        // 判定数のテキスト代入
        for (int i = 0; i < judgeCountTexts.Length; i++) {

            judgeCountTexts[i].text = resultData.judgeCount[i].ToString();
        }

        clearText.SetText(clearType.clearTypes[(int)resultData.clearType].displayName);
        clearType.setColor(ref clearText, resultData.clearType);

        musicTitleText.SetText(resultData.musicTitle);
        composerText.SetText(resultData.composer);

        difficultText.SetText(resultData.difficulty.ToString());
        difficultText.color = difficultyData.difficlties[(int)resultData.difficulty].color;
        levelText.SetText("{0:2}", resultData.level);

        fastCountText.SetText(resultData.fastCount.ToString());
        lateCountText.SetText(resultData.lateCount.ToString());
    }

    private void SetDifferenceScore(int score, int bestScore)
    {
        int differenceScore = score - bestScore;

        if (score > bestScore) {
            differenceScoreText.text = "+" + differenceScore.ToString("D7");
            newRecordText.gameObject.SetActive(true);
        }
        else {
            differenceScoreText.text = differenceScore.ToString("D7");
            newRecordText.gameObject.SetActive(false);
        }
    }

    public void PlayResultAnim(bool show)
    {
        resultAnim.SetBool("Show", show);
    }

    public void TweetManage()
    {
        string clearText = string.Empty;
        string rankText = rankData.findRank(resultStruct.score).rankAlphabet;

        switch (resultStruct.clearType) {

            case ClearType.Failed:
                clearText = "クリア出来なかったよ...";
                break;
            case ClearType.Clear:
                clearText = "クリアしたよ！";
                break;
            case ClearType.ExClear:
                clearText = "Exクリアしたよ！";
                break;
            case ClearType.FullCombo:
                clearText = "フルコンボしたよ！！";
                break;
            case ClearType.Prefect:
                clearText = "パーフェクトしたよ！！！";
                break;
        }

        string sentence = "「{0}」の難易度「{1}」をランク「{2}」で{3} ";
        string formatSentence = string.Format(sentence, resultStruct.musicTitle, resultStruct.difficulty, rankText, clearText);

        StartCoroutine(TweetWithScreenShot.TweetManager.TweetWithScreenShot(formatSentence));
    }
}

public struct ResultData {

    public string musicTitle;
    public string composer;
    public Difficulty difficulty;
    public float level;
    public int score;
    public int bestScore;
    public int maxCombo;
    public ClearType clearType;
    public int[] judgeCount;
    public int fastCount;
    public int lateCount;
}
