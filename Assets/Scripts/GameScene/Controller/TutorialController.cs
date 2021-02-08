using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private TutorialDetail[] tutorialDetails = null;
    [SerializeField]
    private TimeController timeController = null;
    [SerializeField]
    private InputController inputController = null;
    [SerializeField]
    private Animator windowAnim = null;
    [SerializeField]
    private Animator arrowAnim = null;
    [SerializeField]
    private TextMeshProUGUI messageText = null;

    private int showElement = 0;

    private bool isShowText = false;

    // Update is called once per frame
    void Update()
    {
        if (showElement >= tutorialDetails.Length) return;

        if (isShowText)
        {
            if (timeController.PlayTimingCount >= tutorialDetails[showElement].hideTimingCount)
            {
                HideText();
                showElement++;
            }
        }
        else
        {
            if (timeController.PlayTimingCount >= tutorialDetails[showElement].showTimingCount)
            {
                ShowText();
            }
        }
    }

    public void Init(bool isTutorial)
    {
        GetComponent<Canvas>().enabled = isTutorial;
        gameObject.SetActive(isTutorial);

        if (isTutorial)
        {
            // オート連打を遅くする
            inputController.SetAutoBarrageInterval(0.1f);
        }
    }

    public void ShowText()
    {
        messageText.SetText(tutorialDetails[showElement].message);

        if (!windowAnim.GetBool("WindowShow"))
        {
            Find.GetAudioManager().PlaySE("Confrim_3");
        }

        windowAnim.SetBool("WindowShow", true);
        windowAnim.SetBool("TextShow", true);

        if (ShouldPlayArrowAnim(showElement))
        {
            arrowAnim.gameObject.SetActive(true);
            arrowAnim.SetBool("Show", true);
            arrowAnim.SetTrigger(tutorialDetails[showElement].arrowAnimName);
        }

        StaticValue.isAuto = tutorialDetails[showElement].isAutoPlay;
        isShowText = true;
    }

    public void HideText()
    {
        windowAnim.SetBool("TextShow", false);
        if (tutorialDetails[showElement].isWindowHide)
        {
            windowAnim.SetBool("WindowShow", false);
        }

        if (ShouldPlayArrowAnim(showElement) && !ShouldPlayArrowAnim(showElement + 1))
        {
            arrowAnim.SetBool("Show", false);
            arrowAnim.gameObject.SetActive(false);
        }

        isShowText = false;
    }

    private bool ShouldPlayArrowAnim(int element)
    {
        if (showElement >= tutorialDetails.Length) return false;

        return tutorialDetails[element].arrowAnimName != string.Empty;
    }

    [System.Serializable]
    public struct TutorialDetail
    {
        [TextArea()]
        public string message;
        public string arrowAnimName;
        public int showTimingCount;
        public int hideTimingCount;
        public bool isWindowHide;
        public bool isAutoPlay;
    }
}
