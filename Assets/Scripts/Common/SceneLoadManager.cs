using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private bool isWorking = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool LoadGameScene()
    {
        if (!CanLoadScene()) return false;
        StartCoroutine(LoadGameSceneAsync());

        return true;
    }

    public bool LoadMusicSelectScene(bool isNonStar = false)
    {
        if (!CanLoadScene()) return false;

        if (isNonStar) {
            StartCoroutine(LoadSceneNonStar("MusicSelectScene"));
        }
        else {
            StartCoroutine(LoadMusicSelectSceneAsync());
        }

        return true;
    }

    public bool LoadCreateScene()
    {
        if (!CanLoadScene()) return false;
        StartCoroutine(LoadSceneNonStar("CreateScene"));

        return true;
    }

    public bool LoadTitleScene()
    {
        if (!CanLoadScene()) return false;
        StartCoroutine(LoadSceneNonStar("TitleScene"));

        return true;
    }

    public bool LoadDemo()
    {
        if (!CanLoadScene()) return false;

        string[] allMusic = Chart.ChartUtility.GetAllFileNames();
        // 楽曲がない場合は再生しない
        if (allMusic.Length <= 0) return false;

        int playRand = Random.Range(0, allMusic.Length);
        string playName = allMusic[playRand];

        MusicStruct playMusic = Chart.ChartUtility.LoadMusicData(playName);
        Difficulty[] lowDifs = playMusic.GetLowDifficult(Difficulty.Another);
        // 低難度譜面がない場合は再生しない
        if (lowDifs.Length <= 0 || !playMusic.isShowMusicList) return false;

        int difRand = Random.Range(0, lowDifs.Length);

        StaticValue.loadName = playName;
        StaticValue.difficulty = lowDifs[difRand];
        StaticValue.isAuto = true;

        StartCoroutine(LoadDemoAsync());

        return true;
    }

    public bool LoadTutorial()
    {
        if (!CanLoadScene()) return false;

        StaticValue.loadName = "Tutorial";
        StaticValue.difficulty = Difficulty.Beginner;
        StaticValue.isAuto = true;

        StartCoroutine(LoadTutorialAsync());

        return true;
    }

    public bool CanLoadScene()
    {
        return !isWorking;
    }

    public IEnumerator LoadResultScene()
    {
        yield return SceneManager.LoadSceneAsync("ResultScene", LoadSceneMode.Additive);
    }

    public IEnumerator UnLoadResultScene()
    {
        yield return SceneManager.UnloadSceneAsync("ResultScene");
    }

    private IEnumerator LoadGameSceneAsync()
    {
        yield return BeginLoadScene("GameScene");

        yield return StartCoroutine(Find.GetGameController().StartInit());
        yield return StartCoroutine(Find.GetLoadSceneController().WaitAnimStars(false));

        yield return StartCoroutine(Find.GetGameController().SetStarting());

        yield return SceneManager.UnloadSceneAsync("LoadScene");

        isWorking = false;
    }

    private IEnumerator LoadMusicSelectSceneAsync()
    {
        yield return BeginLoadScene("MusicSelectScene");
        yield return FinishLoadScene();
    }

    private IEnumerator LoadSceneNonStar(string sceneName)
    {
        yield return BeginLoadScene(sceneName, false);
        yield return FinishLoadScene(false);
    }

    private IEnumerator LoadTutorialAsync()
    {
        yield return BeginLoadScene("GameScene", false);
        yield return StartCoroutine(Find.GetGameController().TutorialStartInit());
        yield return FinishLoadScene(false);
    }

    private IEnumerator LoadDemoAsync()
    {
        yield return BeginLoadScene("GameScene", false);
        yield return StartCoroutine(Find.GetGameController().DemoStartInit());
        yield return FinishLoadScene(false);
    }

    private IEnumerator WaitSceneAsync(AsyncOperation asyncScene)
    {
        while(asyncScene.progress < 0.9f) {
            yield return null;
        }

        asyncScene.allowSceneActivation = true;
        yield return null;
    }

    private IEnumerator BeginLoadScene(string sceneName, bool isShowStar = true)
    {
        isWorking = true;

        Scene currentScene = SceneManager.GetActiveScene();
        yield return SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);
        // しばらく待たないとヌルぽが発生する
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(Find.GetLoadSceneController().WaitAnimStars(true, isShowStar));

        if (isShowStar) {
            Find.GetLoadSceneController().SetActiveCookpit(true);
        }

        yield return SceneManager.UnloadSceneAsync(currentScene);

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // しばらく待たないとヌルぽが発生する
        yield return new WaitForSeconds(0.1f);

        if (isShowStar)
        {
            Find.GetLoadSceneController().SetActiveCookpit(false);
        }
    }

    private IEnumerator FinishLoadScene(bool isShowStar = true)
    {
        yield return StartCoroutine(Find.GetLoadSceneController().WaitAnimStars(false, isShowStar));
        yield return new WaitForSeconds(1.0f);

        yield return SceneManager.UnloadSceneAsync("LoadScene");

        isWorking = false;
    }
}
