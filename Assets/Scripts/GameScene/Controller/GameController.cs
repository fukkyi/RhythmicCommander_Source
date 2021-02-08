using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Chart;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private string loadName = string.Empty;

    [SerializeField]
    private TimeController timeController = null;
    [SerializeField]
    private NotesController notesController = null;
    [SerializeField]
    private MeasureController measureController = null;
    [SerializeField]
    private InputController inputController = null;
    [SerializeField]
    private UIController uiCountroller = null;
    [SerializeField]
    private StarController starCountroller = null;
    [SerializeField]
    private ParticleController particleController = null;
    [SerializeField]
    private TutorialController tutorialController = null;

    private ResultController resultController = null;

    [SerializeField]
    private JudgeData[] timingRange = null;
    [SerializeField]
    private GaugeRuleData gaugeStructs = null;
    [SerializeField]
    private BackGroundData backGroundData = null;

    [SerializeField]
    private float noteSpeed = 1.0f;
    [SerializeField]
    private float maxDemoPlayTime = 60;

    [SerializeField]
    private Animator cookpitUIAnim = null;
    [SerializeField]
    private Animator readyAnim = null;
    [SerializeField]
    private Animator notesCameraAnim = null;
    [SerializeField]
    private Animator failedAnim = null;
    [SerializeField]
    private Animator growAnim = null;
    [SerializeField]
    private Animator keyInfoAnim = null;
    [SerializeField]
    private Animator tutorialAnim = null;
    [SerializeField]
    private Animator achevementAnim = null;
    [SerializeField]
    private Animator musicInfoAnim = null;

    [SerializeField]
    private AudioSource musicSource = null;

    [SerializeField]
    private Transform laneObjsParent = null;
    [SerializeField]
    private GameObject[] laneObjs = null;
    
    private MusicStruct musicStruct = null;
    private ScoreDataStruct scoreData = null;

    private UIDataStruct uiData;
    private JudgeStyle judgeStyle = JudgeStyle.Normal;

    private int chartEle = 0;
    private int gaugeRule = 0;
    private int currentBeat = -1;

    private float pushExitCount = 0;

    private bool isStarting = false;
    private bool isEnd = false;
    private bool isDeath = false;
    private bool isPlayedMusic = false;
    private bool isResult = false;
    private bool isInitFinish = false;
    private bool isPlayingAcheveAnim = false;
    private bool isPlayedAcheveAnim = false;
    private bool isDemoPlay = false;
    private bool isTutorial = false;

    // メインループ
    void Update()
    {
        if (isStarting)
        {
            RenderSettings.skybox = backGroundData.backGroundMaterials[(int)musicStruct.backGroundType];

            if (!isResult)
            {
                timeController.UpdateTime(GetPlayingChart());
            }
            // デモプレイ時に最大再生時間を超えたらタイトルに戻る
            if (isDemoPlay && (timeController.PlayTime > maxDemoPlayTime || Input.GetButtonDown("Lane3")))
            {
                FinishDemo();
            }
            // 楽曲終了時の処理
            if (timeController.PlayTimingCount >= GetPlayingChart().chartLength  && !isEnd && !isPlayingAcheveAnim)
            {
                if (isDemoPlay)
                {
                    // デモプレイの場合はタイトルに戻る
                    FinishDemo();
                }
                else if (isTutorial)
                {
                    // チュートリアルの場合は楽曲選択に戻る
                    Find.GetSceneLoadManager().LoadMusicSelectScene(true);
                }
                else 
                {
                    StartCoroutine(PlayResult());
                    isEnd = true;
                }

                return;
            }

            MightPlayMusic(timeController.PlayTime);
            MigthPlayGrowAnim();

            particleController.UpdateManage();
            // 終了したらは入力を受け付けないようにする
            if (!isEnd) {
                inputController.UpdateManage(ref uiData, gaugeStructs.gaugeRules[gaugeRule], laneObjs, timingRange[(int)judgeStyle], isStarting);
            }

            notesController.UpdateManage(noteSpeed);
            measureController.MoveMeasure(noteSpeed, GetPlayingChart());
            starCountroller.UpdateManage();

            UpdateUIData();
            uiCountroller.UpdateManage(uiData, gaugeStructs.gaugeRules[gaugeRule], isEnd);

            checkKill();
            mightPlayAcheveAnim();
        }
        else if (isInitFinish) {
            inputController.UpdateManage(ref uiData, gaugeStructs.gaugeRules[gaugeRule], laneObjs, timingRange[(int)judgeStyle], isStarting);
        }

        if (isResult) {

            RenderSettings.skybox = backGroundData.backGroundMaterials[(int)musicStruct.backGroundType];

            if (Input.GetButtonDown("Lane3")) {

                StartCoroutine(HideResult());
                isResult = false;
            }
        }
    }

    public IEnumerator TutorialStartInit()
    {
        isTutorial = true;
        yield return StartInit();
    }

    public IEnumerator DemoStartInit()
    {
        isDemoPlay = true;
        yield return StartInit();
    }

    public IEnumerator StartInit()
    {
        Application.targetFrameRate = 60;
        loadName = StaticValue.loadName;

        if (Constant.CompareEnv(GameEnvironment.local)) {
            musicStruct = ChartUtility.LoadMusicData(loadName);
        }
        else if (Constant.CompareEnv(GameEnvironment.webGL)) {

            CacheStruct cacheStruct = ChartUtility.FindCacheStruct(loadName);
            musicStruct = cacheStruct.musicStruct;
            scoreData = cacheStruct.scoreStruct;
        }

        yield return StartCoroutine(GameInit());
    }

    public IEnumerator SetStarting()
    {
        musicInfoAnim.SetTrigger("TrackAnim");

        yield return ShowTutorial();

        readyAnim.SetTrigger("Anim");
        yield return new WaitForSeconds(3.0f);

        keyInfoAnim.SetBool("KeyShow", false);

        isStarting = true;
    }

    private void UpdateUIData()
    {
        if (uiData.bpm != timeController.NowBpm) {

            growAnim.SetFloat("Speed", timeController.NowBpm / 120);
        }

        uiData.bpm = timeController.NowBpm;
        uiData.noteSpeed = noteSpeed;
    }

    private IEnumerator GameInit()
    {
        chartEle = SetChartDifficlty(StaticValue.difficulty);
        gaugeRule = (int)StaticValue.gaugeRule;
        noteSpeed = StaticValue.noteSpeed;
        judgeStyle = StaticValue.judgeStyle;
        System.Array.Resize(ref uiData.judgeCount, timingRange[(int)judgeStyle].judges.Length);

        ChartStruct chart = musicStruct.charts[chartEle];

        timeController.Init(chart);
        tutorialController.Init(isTutorial);
        // 各コントローラーの初期化処理を非同期で行う
        yield return StartCoroutine(notesController.GenerateNotes(gaugeStructs.gaugeRules[gaugeRule], chart, laneObjs));
        yield return StartCoroutine(measureController.Init(chart, laneObjsParent));
        yield return StartCoroutine(inputController.Init(laneObjs.Length));
        yield return StartCoroutine(starCountroller.Init());
        yield return StartCoroutine(particleController.Init());
        yield return StartCoroutine(LoadAudioData(musicStruct.musicFileName, loadName));

        RenderSettings.skybox = backGroundData.backGroundMaterials[(int)musicStruct.backGroundType];

        notesController.UpdateManage(noteSpeed);
        measureController.MoveMeasure(noteSpeed, chart);

        // ScoreCountの計算
        uiData.maxScoreCount = notesController.totalCombo * timingRange[(int)judgeStyle].judges[(int)JudgeType.Just].scoreCount;
        uiData.totalGaugeCount = notesController.totalGaugeCount;
        uiData.noteSpeed = noteSpeed;
        uiData.bpm = chart.bpm;
        uiData.gaugePer = gaugeStructs.gaugeRules[gaugeRule].startPar;

        growAnim.SetFloat("Speed", chart.bpm / 120);

        musicSource.volume = musicStruct.musicVolume;

        uiCountroller.Init(
            uiData: uiData,
            gaugeRule: gaugeStructs.gaugeRules[gaugeRule],
            trackCount: StaticValue.trackCount,
            musicName: musicStruct.musicName,
            artistName: musicStruct.composer,
            difficulty: chart.difficulty,
            level: chart.level,
            timeLength: Clac.TimeByTimingCount(
                count: chart.chartLength,
                oneBeatCount: Clac.OneBeatCount(chart.beat, chart.beatParam),
                musicOffset: chart.musicOffset,
                bpm: chart.bpm,
                tempoChanger: chart.tempoChangers),
            isDemo: isDemoPlay,
            isTutorial: isTutorial
        );

        yield return null;
        System.GC.Collect();
        yield return null;

        isInitFinish = true;

        cookpitUIAnim.SetBool("Show", true);

        if (isDemoPlay || isTutorial) {
            // デモプレイ、チュートリアルはすぐに始める
            cookpitUIAnim.SetTrigger("NonAnim");
            isStarting = true;
        }
        else {
            keyInfoAnim.SetBool("KeyShow", true);
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator LoadAudioData(string musicName, string loadName)
    {
        if (musicName == string.Empty) yield break;

        if (Constant.CompareEnv(GameEnvironment.local)) {

            using (UnityWebRequest resource = UnityWebRequestMultimedia.GetAudioClip("File://" + MusicPath.getMusicDataPath(loadName, musicName), Constant.musicAudioType)) {

                yield return resource.SendWebRequest();

                if (resource.isNetworkError) {

                    Debug.LogError(resource.error);
                }
                else {

                    musicSource.clip = DownloadHandlerAudioClip.GetContent(resource);
                }
            }
        }
        else if (Constant.CompareEnv(GameEnvironment.webGL)) {

            musicSource.clip = Resources.Load(Constant.audioResourceDirectory + musicName) as AudioClip;
            musicSource.clip.LoadAudioData();
        }
    }

    private void MightPlayMusic(float playTime)
    {
        if (isPlayedMusic) {

            // 音源がノーツと大幅にずれた場合は音源の再生位置を調整する
            if (Mathf.Abs(musicSource.time - playTime) >= 0.05f) {

                musicSource.time = playTime;
            }
        }
        else if (playTime >= 0) {

            musicSource.Play();
            musicSource.time = playTime;

            isPlayedMusic = true;
        }
    }

    private int SetChartDifficlty(Difficulty difficulty)
    {
        int chartDifficulty = 1;

        for (int i = 0; i < musicStruct.charts.Length; i++) {

            if (musicStruct.charts[i].difficulty == difficulty) chartDifficulty = i; 
        }

        return chartDifficulty;
    }

    private IEnumerator PlayResult()
    {
        cookpitUIAnim.SetBool("Show", false);
        uiCountroller.conboAnim.SetTrigger("Hide");

        yield return new WaitForSeconds(0.5f);

        notesCameraAnim.SetTrigger("Hide");
        starCountroller.HideStars();

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(ShowResult());
    }

    private IEnumerator ShowResult()
    {
        StaticValue.trackCount++;

        yield return Find.GetSceneLoadManager().LoadResultScene();
        yield return null;

        resultController = GameObject.FindWithTag("ResultController").GetComponent<ResultController>();

        ResultData resultData = new ResultData();

        ClearType clearType = JudgeClearType();
        Difficulty selectDif = StaticValue.difficulty;
        string musicTitle = musicStruct.musicName;
        int score = uiCountroller.targetScore;
        int bestScore = ChartUtility.GetBestScore(StaticValue.loadName, selectDif);

        resultData.musicTitle = musicTitle;
        resultData.clearType = clearType;
        resultData.composer = musicStruct.composer;
        resultData.difficulty = selectDif;
        resultData.score = uiCountroller.targetScore;
        resultData.bestScore = bestScore;
        resultData.judgeCount = uiData.judgeCount;
        resultData.level = GetPlayingChart().level;
        resultData.maxCombo = uiData.maxConbo;
        resultData.fastCount = inputController.fastCount;
        resultData.lateCount = inputController.lateCount;

        resultController.SetUIValue(resultData);
        yield return new WaitForSeconds(0.5f);

        resultController.PlayResultAnim(true);

        float loadTime = 2.5f;

        if (!StaticValue.isAuto) {
            float nowTime = Time.realtimeSinceStartup;
            yield return StartCoroutine(ChartUtility.WriteScoreDataFromResult(StaticValue.loadName, score, selectDif, clearType));
            loadTime = Mathf.Clamp(loadTime - Time.realtimeSinceStartup - nowTime, 0, 2.5f);
        }

        yield return new WaitForSeconds(loadTime);

        isResult = true;
    }

    private IEnumerator HideResult()
    {
        resultController.PlayResultAnim(false);
        cookpitUIAnim.SetTrigger("GaugeHide");

        yield return new WaitForSeconds(1.0f);
        yield return Find.GetSceneLoadManager().UnLoadResultScene();

        Find.GetSceneLoadManager().LoadMusicSelectScene();
    }

    private ClearType JudgeClearType ()
    {
        if (isDeath) return ClearType.Failed;

        ClearType clearType;

        if (notesController.totalCombo == uiData.conboCount) {

            if (uiData.judgeCount[(int)JudgeType.Just] == notesController.totalCombo) {

                clearType = ClearType.Prefect;
            }
            else {

                clearType = ClearType.FullCombo;
            }
        }
        else {

            RuleStruct selectedRule = gaugeStructs.gaugeRules[gaugeRule];

            if (uiData.gaugePer >= selectedRule.clearPar) {

                if ((int)StaticValue.gaugeRule == (int)GaugeRule.Expert) {

                    clearType = ClearType.ExClear;
                }
                else {

                    clearType = ClearType.Clear;
                }
            }
            else {

                clearType = ClearType.Failed;
            }
        }

        return clearType;
    }

    private void checkKill()
    {
        if (isEnd || isDemoPlay) return;

        // Escapeを長押ししたときは強制終了させる
        if (Input.GetButton("Cancel")) {
            pushExitCount += Time.deltaTime;

            if (pushExitCount >= 1.0f) Kill();
        }
        else {
            pushExitCount = 0;
        }

        RuleStruct playingRule = gaugeStructs.gaugeRules[gaugeRule];

        if (playingRule.death) {

            if (uiData.gaugePer <= 0) Kill();
        }
    }

    private void Kill()
    {
        isEnd = true;
        isDeath = true;
        StartCoroutine(showFailedAnim());
    }

    private IEnumerator showFailedAnim()
    {
        musicSource.Stop();
        Find.GetAudioManager().PlayOneShotSE("Failed1");

        failedAnim.gameObject.SetActive(true);
        failedAnim.SetTrigger("Show");
        uiCountroller.conboAnim.SetTrigger("Hide");

        yield return new WaitForSeconds(1.0f);

        cookpitUIAnim.SetBool("Show", false);
        notesCameraAnim.SetTrigger("Hide");
        starCountroller.HideStars();

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(ShowResult());
    }

    private void MigthPlayGrowAnim()
    {
        if (timeController.PlayTimingCount < 0 || isEnd) return;

        if (currentBeat != timeController.NowBeat) {
            growAnim.SetTrigger("Anim");
        }

        currentBeat = timeController.NowBeat;
    }

    private IEnumerator ShowTutorial()
    {
        if (!PlayerPrefs.HasKey("isPlayTutorial")) {

            tutorialAnim.SetBool("Show", true);
            yield return new WaitForSeconds(1.0f);
            // 入力待ち
            while(!Input.GetButtonDown("Lane3")) {
                yield return null;
            }

            PlayerPrefs.SetInt("isPlayTutorial", 1);
            PlayerPrefs.Save();

            tutorialAnim.SetBool("Show", false);
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void mightPlayAcheveAnim()
    {
        if (isEnd || isPlayedAcheveAnim || isTutorial) return;

        if (notesController.totalCombo == uiData.conboCount) {

            if (uiData.judgeCount[(int)JudgeType.Just] == notesController.totalCombo) {
                StartCoroutine(playAcheveAnim("Perfect"));
            }
            else {
                StartCoroutine(playAcheveAnim("FullCombo"));
            }
        }
    }

    private IEnumerator playAcheveAnim(string animName)
    {
        isPlayingAcheveAnim = true;
        isPlayedAcheveAnim = true;
        Find.GetAudioManager().PlayOneShotSE("Achevement");
        achevementAnim.gameObject.SetActive(true);
        achevementAnim.SetTrigger(animName);

        yield return new WaitForSeconds(3.2f);
        achevementAnim.gameObject.SetActive(false);

        isPlayingAcheveAnim = false;
    }

    private void FinishDemo()
    {
        Find.GetSceneLoadManager().LoadTitleScene();
    }

    private ChartStruct GetPlayingChart()
    {
        if (musicStruct.charts == null) return null;

        return musicStruct.charts[chartEle];
    }
}
