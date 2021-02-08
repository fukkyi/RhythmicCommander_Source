using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ControllerInputUI : MonoBehaviour
{
    [SerializeField]
    private GameObject controllerUI = null;
    [SerializeField]
    private GameObject keyboardUI = null;

    [SerializeField]
    private TextMeshProUGUI[] buttonTexts = null;
    [SerializeField]
    private Animator[] buttonAnims = null;

    private string[] buttonNames = null;

    private void Start()
    {
        if (Constant.IsConnectingController())
        {
            controllerUI.SetActive(true);
            keyboardUI.SetActive(false);
        }
        else
        {
            controllerUI.SetActive(false);
            keyboardUI.SetActive(true);
        }

        buttonNames = Enum.GetNames(typeof(InputButton));
    }

    private void Update()
    {
        if (!controllerUI.activeSelf) return;

        for(int i = 0; i < buttonAnims.Length; i++)
        {
            bool isPush = Input.GetButton(buttonNames[i]);
            if (buttonAnims[i].GetBool("Push") == isPush) continue;

            buttonAnims[i].SetBool("Push", isPush);
        }
    }

    public void SetMusicSelectTexts()
    {
        if (!controllerUI.activeSelf) return;

        buttonTexts[(int)ButtonInfoText.BlueLeft].SetText("楽曲 ↓");
        buttonTexts[(int)ButtonInfoText.BlueRight].SetText("楽曲 ↑");
        buttonTexts[(int)ButtonInfoText.WhiteLeft].SetText("レベル ↓");
        buttonTexts[(int)ButtonInfoText.WhiteRight].SetText("レベル ↑");
        buttonTexts[(int)ButtonInfoText.Red].SetText("決定");
        buttonTexts[(int)ButtonInfoText.Green].SetText("");
    }

    public void SetConfrimTexts()
    {
        if (!controllerUI.activeSelf) return;

        buttonTexts[(int)ButtonInfoText.BlueLeft].SetText("選択 ↓");
        buttonTexts[(int)ButtonInfoText.BlueRight].SetText("選択 ↑");
        buttonTexts[(int)ButtonInfoText.WhiteLeft].SetText("レベル ↓");
        buttonTexts[(int)ButtonInfoText.WhiteRight].SetText("レベル ↑");
        buttonTexts[(int)ButtonInfoText.Red].SetText("決定");
        buttonTexts[(int)ButtonInfoText.Green].SetText("戻る");
    }

    public void SetOptionTexts()
    {
        if (!controllerUI.activeSelf) return;

        buttonTexts[(int)ButtonInfoText.BlueLeft].SetText("選択 ↓");
        buttonTexts[(int)ButtonInfoText.BlueRight].SetText("選択 ↑");
        buttonTexts[(int)ButtonInfoText.WhiteLeft].SetText("値 ↓");
        buttonTexts[(int)ButtonInfoText.WhiteRight].SetText("値 ↑");
        buttonTexts[(int)ButtonInfoText.Red].SetText("");
        buttonTexts[(int)ButtonInfoText.Green].SetText("戻る");
    }

    private enum ButtonInfoText
    {
        BlueLeft,
        BlueRight,
        WhiteLeft,
        WhiteRight,
        Red,
        Green
    }
}