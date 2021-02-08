using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    private Animator startCameraAnim = null;
    [SerializeField]
    private Animator canvasAnim = null;
    [SerializeField]
    private Animator tutorialDialogAnim = null;

    [SerializeField]
    private GameObject keyboardInfoContent = null;
    [SerializeField]
    private GameObject controllerInfoContent = null;

    [SerializeField]
    private RectTransform editBanner = null;
    [SerializeField]
    private TextMeshProUGUI verText = null;
    [SerializeField]
    private TextMeshProUGUI envText = null;
    [SerializeField]
    private TMP_InputField playerNameIF = null;

    [SerializeField]
    private float startDemoTime = 60;

    private bool isSelected = false;
    private bool isController = false;
    private bool isShowTutorialDialog = false;

    private float currentDemoTime = 0;

    void Awake()
    {
        if (Constant.CompareEnv(GameEnvironment.webGL)) {
            editBanner.gameObject.SetActive(false);
        }

        isController = Constant.IsConnectingController();
        if (isController) 
        {
            keyboardInfoContent.SetActive(false);
            controllerInfoContent.SetActive(true);
        }
        else
        {
            keyboardInfoContent.SetActive(true);
            controllerInfoContent.SetActive(false);
        }

        verText.SetText("Ver: " + Constant.version);
        envText.SetText("Env: " + Constant.env);
    }

    // Start is called before the first frame update
    void Start()
    {
        Find.GetAudioManager().PlayBGM("Mystic Edge");
        StaticValue.trackCount = 1;

        if (PlayerPrefs.HasKey("PlayerName"))
        {
            StaticValue.playerName = PlayerPrefs.GetString("PlayerName");
        }
        playerNameIF.text = StaticValue.playerName;
    }

    // Update is called once per frame
    void Update()
    {
        if (isController)
        {
            if (tutorialDialogAnim.GetBool("Show"))
            {
                MoveToMusicSelectScene();
            }
            else if (Input.GetButtonDown("Lane3"))
            {
                StartCoroutine(ShowtutorialDialog());
            }
        }

        if (currentDemoTime > startDemoTime)
        {
            if (Find.GetSceneLoadManager().LoadDemo())
            {
                currentDemoTime = 0;
            }
        }
        else
        {
            currentDemoTime += Time.deltaTime;
        }
    }

    public IEnumerator ShowtutorialDialog()
    {
        if (tutorialDialogAnim.GetBool("Show")) yield break;

        Find.GetAudioManager().PlaySE("Confrim_2");
        tutorialDialogAnim.SetBool("Show", true);
        yield return new WaitForSeconds(0.3f);

        isShowTutorialDialog = true;
    }

    public void SelectTutorialDialog()
    {
        if (Input.GetButtonDown("Lane3"))
        {
            MoveToTutorial();
        }
        if (Input.GetButtonDown("Submit"))
        {
            MoveToMusicSelectScene();
        }
    }

    public void MoveToMusicSelectScene()
    {
        if (isSelected) return;
        bool isWork = Find.GetSceneLoadManager().LoadMusicSelectScene(true);

        if (!isWork) return;
        PlaySelectAnim();
    }

    public void MoveToCreateScene()
    {
        if (isSelected) return;
        bool isWork = Find.GetSceneLoadManager().LoadCreateScene();

        if (!isWork) return;
        PlaySelectAnim();
    }

    public void MoveToTutorial()
    {
        if (isSelected) return;
        bool isWork = Find.GetSceneLoadManager().LoadTutorial();

        if (!isWork) return;
        PlaySelectAnim();
    }

    public void PlaySelectAnim()
    {
        Find.GetAudioManager().PlaySE("Confrim_2");
        startCameraAnim.SetTrigger("Start");
        canvasAnim.SetBool("Fade", true);
        tutorialDialogAnim.SetBool("Show", false);

        isSelected = true;
    }

    public void SetPlayerName(string name)
    {
        StaticValue.playerName = name;
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
    }
}
