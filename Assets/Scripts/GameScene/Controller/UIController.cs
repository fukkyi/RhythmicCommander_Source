using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public int targetScore = 0;
    public int comboAnimCount = 100;

    [SerializeField]
    private float scoreAnimTime = 1.0f;
    [SerializeField]
    private DifficultyData difficultyData = null;
    [SerializeField]
    private TimeController timeController = null;

    [SerializeField]
    private Canvas comboCanvas = null;
    [SerializeField]
    private Canvas demoCanvas = null;
    [SerializeField]
    private Transform timePositionTrans = null;

    [SerializeField]
    private Image nonClearGauge = null;
    [SerializeField]
    private Image clearBackGauge = null;
    [SerializeField]
    private Image clearGauge = null;

    [SerializeField]
    private TextMeshProUGUI comboCountText = null;
    [SerializeField]
    private TextMeshProUGUI comboAnimText = null;
    [SerializeField]
    private TextMeshProUGUI barrageCountText = null;
    [SerializeField]
    private TextMeshProUGUI percentageText = null;

    [SerializeField]
    private TextMeshPro[] judgeCountText = null;
    [SerializeField]
    private TextMeshPro scoreText = null;
    [SerializeField]
    private TextMeshPro trackText = null;
    [SerializeField]
    private TextMeshPro musicText = null;
    [SerializeField]
    private TextMeshPro artistText = null;
    [SerializeField]
    private TextMeshPro difficultyText = null;
    [SerializeField]
    private TextMeshPro levelText = null;
    [SerializeField]
    private TextMeshPro bpmText = null;
    [SerializeField]
    private TextMeshPro speedText = null;
    [SerializeField]
    private TextMeshPro totalSpeedText = null;
    [SerializeField]
    private TextMeshPro maxConboText = null;
    [SerializeField]
    private TextMeshPro autoPlayText = null;

    public Animator conboAnim = null;

    [SerializeField]
    private SpriteRenderer growSprite = null;

    [SerializeField]
    private Animator barrageCountAnim = null;
    [SerializeField]
    private Animator parcentageTextAnim = null;
    [SerializeField]
    private Animator countComboAnim = null;

    [SerializeField]
    private AnimationCurve scoreAnimCurve = null;

    private UIDataStruct beforeUIData;

    private bool isAnimScore = false;
    private int showScore = 0;
    private int baseScore = 0;
    private float chartTimeLength = 0;
    private float currentAnimTime = 0;
    private float timeBarLength = 8.8f;

    //要素数はスコアの桁数
    private char[] scoreChars = new char[7];

    public void Init(UIDataStruct uiData, RuleStruct gaugeRule, int trackCount, string musicName, string artistName, Difficulty difficulty, float level, float timeLength, bool isDemo, bool isTutorial)
    {
        SetComboText(0);
        SetMaxConboText(0);

        chartTimeLength = timeLength;

        System.Array.Resize(ref beforeUIData.judgeCount, uiData.judgeCount.Length);

        for (int i = 0; i < uiData.judgeCount.Length; i++) {

            SetJudgeCountText(0, i);
        }

        SetBPMText(uiData.bpm, uiData.noteSpeed);

        // 表示が変わらないUIはここで値を代入
        trackText.SetText("Track {0}", trackCount);
        musicText.SetText(musicName);
        artistText.SetText(artistName);
        difficultyText.SetText(difficulty.ToString());
        difficultyText.color = new Color(
            r: difficultyData.difficlties[(int)difficulty].color.r,
            g: difficultyData.difficlties[(int)difficulty].color.g,
            b: difficultyData.difficlties[(int)difficulty].color.b,
            a: difficultyText.color.a
        );
        levelText.SetText("{0:2}", level);

        demoCanvas.enabled = isDemo;
        // 通常のゲームでAutoPlayの場合は、専用テキストを表示させる
        if (!isTutorial && !isDemo && StaticValue.isAuto) autoPlayText.gameObject.SetActive(true);

        InitGauge(gaugeRule);
        SetTimeGauge();
    }

    public void UpdateManage(UIDataStruct uiData, RuleStruct gaugeRule, bool isEnd)
    {
        if (uiData.conboCount != beforeUIData.conboCount) {

            if (uiData.conboCount < beforeUIData.conboCount) {
                //コンボが途切れた場合はグラデーションを切る
                if (comboCountText.enableVertexGradient) comboCountText.enableVertexGradient = false;
            }

            if ((uiData.conboCount / comboAnimCount) != (beforeUIData.conboCount / comboAnimCount) && uiData.conboCount != 0) {

                comboAnimText.SetText("{0}", uiData.conboCount / comboAnimCount * comboAnimCount);
                countComboAnim.SetTrigger("Show");
            }

            SetComboText(uiData.conboCount);
        }

        if (uiData.barrageCount != beforeUIData.barrageCount) {

            SetBarrageText(uiData.barrageCount);
        }

        if (uiData.maxConbo != beforeUIData.maxConbo) {

            SetMaxConboText(uiData.maxConbo);
        }

        for (int i = 0; i < uiData.judgeCount.Length; i++) {

            if (uiData.judgeCount[i] != beforeUIData.judgeCount[i]) {

                SetJudgeCountText(uiData.judgeCount[i], i);
            }
        }

        if (uiData.scoreCount != beforeUIData.scoreCount) {

            InitScoreAnim(uiData.maxScoreCount, uiData.scoreCount);
        }

        if (isAnimScore) {

            AnimScoreText();
        }

        if ((uiData.bpm != beforeUIData.bpm) || (uiData.noteSpeed != beforeUIData.noteSpeed)) {

            SetBPMText(uiData.bpm, uiData.noteSpeed);
        }

        if (uiData.gaugePer != beforeUIData.gaugePer) {

            SetGauge(uiData.gaugePer, gaugeRule.clearPar);
        }

        SetTimeGauge();

        if (gaugeRule.death) {

            if (isEnd) {
                parcentageTextAnim.SetBool("Dying", false);
            }
            else if (uiData.gaugePer <= gaugeRule.dyingRate) {

                if (!parcentageTextAnim.GetBool("Dying")) {
                    parcentageTextAnim.SetBool("Dying", true);
                    growSprite.color = Color.red;
                }
            }
            else {

                if (parcentageTextAnim.GetBool("Dying")) {
                    parcentageTextAnim.SetBool("Dying", false);
                    growSprite.color = Color.white;
                }
            }
        }

        beforeUIData.SetParam(uiData);
    }

    public void SetComboText(int combo)
    {
        if (combo > 1) {

            comboCountText.SetText("{0}", combo);

            if (!comboCanvas.enabled) {

                comboCanvas.enabled = true;
                conboAnim.enabled = true;
            }

            conboAnim.SetTrigger("Play");
        }
        else {

            if (!comboCanvas.enabled) return;
            conboAnim.enabled = false;
            comboCanvas.enabled = false;
        }
    }

    public void SetBarrageText(int count)
    {
        if (count > 0) {

            if (!barrageCountAnim.GetBool("isShow")) {

                barrageCountAnim.SetBool("isShow", true);
                barrageCountAnim.SetTrigger("Show");
            }

            barrageCountText.SetText("{0}", count);
        }
        else {

            if (!barrageCountAnim.GetBool("isShow")) return;

            if (count == 0) {

                barrageCountText.SetText("{0}", count);
                barrageCountAnim.SetTrigger("Success");
            }
            else {

                barrageCountAnim.SetTrigger("Failed");
            }

            barrageCountAnim.SetBool("isShow", false);
        }
    }

    private void SetMaxConboText(int maxConbo)
    {
        maxConboText.SetText("{0}", maxConbo);
    }

    public void SetJudgeCountText(int judgeCount, int judgeType)
    {
        judgeCountText[judgeType].SetText("{0}", judgeCount);
    }

    private void InitScoreAnim(int maxScoreCount, int scoreCount)
    {
        currentAnimTime = 0;
        baseScore = showScore;
        targetScore = (int)(scoreCount / (float)maxScoreCount * Constant.maxScore);
        isAnimScore = true;
    }

    private void AnimScoreText()
    {
        currentAnimTime += Time.deltaTime;

        showScore = baseScore + (int)(scoreAnimCurve.Evaluate(currentAnimTime / scoreAnimTime) * (targetScore - baseScore));

        if (currentAnimTime >= scoreAnimTime) {

            showScore = targetScore;
            isAnimScore = false;
        }

        SetCharScoreText(showScore);
    }

    private void SetCharScoreText(int score)
    {
        char zeroChar = '0';        

        for (int i = scoreChars.Length - 1; i >= 0; i--) {

            scoreChars[i] = (char)((score % 10) + zeroChar);
            score /= 10;
        }

        scoreText.SetCharArray(scoreChars);
    }

    private void SetBPMText(float bpm, float noteSpeed)
    {
        float totalSpeed = bpm * noteSpeed;

        bpmText.SetText("{0:0}", bpm);
        speedText.SetText("{0:2}", noteSpeed);
        totalSpeedText.SetText("{0:0}",totalSpeed);
    }

    private void InitGauge(RuleStruct gaugeRule)
    {
        clearGauge.color = gaugeRule.clearColor;
        nonClearGauge.color = gaugeRule.nonClearColor;
        clearBackGauge.color = gaugeRule.quoteColor;

        clearBackGauge.fillAmount = 1 - gaugeRule.clearPar;

        SetGauge(gaugeRule.startPar, gaugeRule.clearPar);
    }

    private void SetGauge(float gaugePar, float clearPar)
    {
        nonClearGauge.fillAmount = Mathf.Clamp(gaugePar, 0, clearPar);
        clearGauge.fillAmount = Mathf.Clamp01(gaugePar);

        percentageText.SetText("{0}%", Mathf.FloorToInt(gaugePar * 100));
    }

    private void SetTimeGauge()
    {
        float timePosition = Mathf.Clamp01((timeController.PlayTime - timeController.StartMusicOffset) / (chartTimeLength - timeController.StartMusicOffset));

        timePositionTrans.localPosition = Vector3.right * ((timePosition / 1 * timeBarLength) - (timeBarLength / 2));
    }
}

public struct UIDataStruct {

    public int maxScoreCount;
    public int totalGaugeCount;
    public int scoreCount;
    public int conboCount;
    public int barrageCount;
    public int maxConbo;
    public int[] judgeCount;
    public float bpm;
    public float noteSpeed;
    public float gaugePer;

    public void SetParam(UIDataStruct uiData)
    {
        conboCount = uiData.conboCount;
        barrageCount = uiData.barrageCount;

        for (int i = 0; i < uiData.judgeCount.Length; i++) {
            judgeCount[i] = uiData.judgeCount[i];
        }

        totalGaugeCount = uiData.totalGaugeCount;
        maxScoreCount = uiData.maxScoreCount;
        maxConbo = uiData.maxConbo;
        scoreCount = uiData.scoreCount;
        bpm = uiData.bpm;
        noteSpeed = uiData.noteSpeed;
        gaugePer = uiData.gaugePer;
    }
}
