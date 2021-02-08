using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSelectController : MonoBehaviour
{
    [SerializeField]
    private SelectInputController selectInputController = null;
    [SerializeField]
    private MusicDataController musicDataController = null;
    [SerializeField]
    private MusicListController musicListController = null;
    [SerializeField]
    private ChartInfoUIController chartInfoUIController = null;
    [SerializeField]
    private GameOptionController optionController = null;
    [SerializeField]
    private MusicAudioController musicAudioController = null;
    [SerializeField]
    private RankingController rankingController = null;

    [SerializeField]
    private Animator startCameraAnim = null;
    [SerializeField]
    private Animator canvasAnim = null;

    private bool isConfrim = false;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        StartCoroutine(InitManage());
    }

    void Update()
    {
        if (isConfrim) return;

        selectInputController.UpdateInput();
        musicListController.UpdateManage(optionController.isEneble);
        musicAudioController.UpdateManage();
        optionController.UpdateManage();
        rankingController.UpdateManage();
    }

    public void ConfrimMusic()
    {
        if (isConfrim) return;

        isConfrim = true;
        StaticValue.loadName = musicDataController.GetSelectingMusicStruct().folderName;
        StaticValue.difficulty = musicDataController.selectingDif;

        Find.GetAudioManager().PlayOneShotSE("Confrim_2");
        musicAudioController.StopMusic();
        StartCoroutine(PlayStartAnim());
    }

    private IEnumerator PlayStartAnim()
    {
        canvasAnim.SetBool("Show", false);
        yield return new WaitForSeconds(0.5f);

        startCameraAnim.SetTrigger("Start");
        yield return new WaitForSeconds(1.0f);


       Find.GetSceneLoadManager().LoadGameScene();
    }

    private IEnumerator InitManage()
    {
        selectInputController.Init();

        float loadTime = 1.0f;
        float startTime = Time.realtimeSinceStartup;
        yield return StartCoroutine(musicDataController.Init());
        loadTime = Mathf.Clamp(Time.realtimeSinceStartup - startTime, 0, 1.0f);

        musicListController.Init();
        chartInfoUIController.Init();
        rankingController.Init();
        StartCoroutine(musicAudioController.Init());

        yield return new WaitForSeconds(loadTime);
        canvasAnim.SetBool("Show", true);
    }
}
