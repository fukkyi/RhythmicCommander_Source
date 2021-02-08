using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOptionController : MonoBehaviour
{
    [SerializeField]
    private SelectInputController inputController = null;
    [SerializeField]
    private ControllerInputUI controllerUI = null;

    [SerializeField]
    private TextMeshProUGUI SelectTitleText = null;
    [SerializeField]
    private TextMeshProUGUI SelectComposerText = null;
    [SerializeField]
    private TextMeshProUGUI descriptionText = null;

    [SerializeField]
    private Option[] options = null;
    [SerializeField]
    private Animator optionUIAnim = null;
    [SerializeField]
    private Animator selectFlameAnim = null;
    [SerializeField]
    private RectTransform selectFlame = null;

    [SerializeField]
    private float maxChangeInterval = 0.3f;
    [SerializeField]
    private float minChangeInterval = 0.05f;

    public bool isEneble { get; private set; }

    private int selectOptionEle = 0;

    private float changeInterval = 0;
    private float pushingTime = 0;

    public void SetMusicInfoUI(MusicInfoStruct musicInfo)
    {
        SelectTitleText.SetText(musicInfo.title);
        SelectComposerText.SetText(musicInfo.composer);
    }

    public void ShowOptionUI()
    {
        isEneble = true;

        foreach(Option option in options) {
            option.Init();
        }

        optionUIAnim.SetBool("Show", true);
        selectFlameAnim.enabled = true;
        options[selectOptionEle].UnSelected();
        selectOptionEle = 0;
        options[selectOptionEle].Selected();
        RefreshUI();

        controllerUI.SetOptionTexts();

        Find.GetAudioManager().PlayOneShotSE("Confrim_1");
    }

    public void UpdateManage()
    {
        if (!isEneble) return;
        
        bool pushUp = inputController.GetInput(InputName.DifficultyUp);
        bool pushDown = inputController.GetInput(InputName.DifficultyDown);
        bool submit = inputController.GetInput(InputName.Submit);
        bool cancel = inputController.GetInputDown(InputName.Cancel);
        bool valueUp = false;
        bool valueDown = false;

        ChangeElement();

        if (pushUp || pushDown) {

            pushingTime -= Time.deltaTime;

            if (pushingTime < 0) {

                valueUp = pushUp;
                valueDown = pushDown;
                pushingTime = changeInterval;

                changeInterval = Mathf.Clamp(changeInterval - 0.05f, minChangeInterval, maxChangeInterval);
            }
        }
        else {

            pushingTime = 0;
            changeInterval = maxChangeInterval;
        }

        options[selectOptionEle].ChangeValue(valueUp, valueDown, submit);

        if (cancel) {

            optionUIAnim.SetBool("Show", false);
            selectFlameAnim.enabled = false;
            isEneble = false;

            controllerUI.SetConfrimTexts();

            Find.GetAudioManager().PlayOneShotSE("Cancel");
        }
    }

    public void ChangeElement()
    {
        bool selectUp = inputController.GetInputDown(InputName.ListUp);
        bool selectDown = inputController.GetInputDown(InputName.ListDown);

        if (selectUp || selectDown) {

            options[selectOptionEle].UnSelected();

            if (selectUp) selectOptionEle--;
            if (selectDown) selectOptionEle++;

            int clampSelectOptionEle = Mathf.Clamp(selectOptionEle, 0, options.Length - 1);
            if (clampSelectOptionEle == selectOptionEle) Find.GetAudioManager().PlayOneShotSE("ListMove");
            selectOptionEle = clampSelectOptionEle;

            options[selectOptionEle].Selected();
            RefreshUI();            
        }
    }

    public void RefreshUI()
    {
        float posY = options[selectOptionEle].transform.localPosition.y;
        selectFlame.anchoredPosition = new Vector2(selectFlame.localPosition.x, posY);

        descriptionText.SetText(options[selectOptionEle].GetDescription());
    }
}
