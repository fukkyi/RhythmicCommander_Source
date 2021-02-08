using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectInputController : MonoBehaviour
{
    [SerializeField]
    private float returnTitleTime = 60f;
    [SerializeField]
    private InputStruct[] inputStructs = null;

    private string[][] inputButtonStrings = null;

    private bool[] isPushButtons = null;
    private bool[] isPushButtonsDown = null;

    private float currentWaitTime = 0;

    public void Init()
    {
        Array.Resize(ref inputButtonStrings, inputStructs.Length);
        Array.Resize(ref isPushButtons, inputStructs.Length);
        Array.Resize(ref isPushButtonsDown, inputStructs.Length);

        for (int i = 0; i < inputButtonStrings.Length; i++) {

            Array.Resize(ref inputButtonStrings[i], inputStructs[i].buttons.Length);
            for (int j = 0; j < inputButtonStrings[i].Length; j++) {

                inputButtonStrings[i][j] = Enum.GetName(typeof(InputButton), inputStructs[i].buttons[j]);
            }
        }
    }

    public void UpdateInput()
    {
        for (int i = 0; i < inputButtonStrings.Length; i++)
        {
            isPushButtons[i] = CheckInput(inputButtonStrings[i]);
            isPushButtonsDown[i] = CheckInputDown(inputButtonStrings[i]);
        }

        currentWaitTime += Time.deltaTime;
        if (currentWaitTime >= returnTitleTime)
        {
            Find.GetSceneLoadManager().LoadTitleScene();
            currentWaitTime = 0;
        }
    }

    public bool GetInput(InputName inputName)
    {
        bool isPush = false;

        for (int i = 0; i < inputStructs.Length; i++) {

            if (inputName == inputStructs[i].inputName) {

                isPush = isPushButtons[i];
            }
        }

        return isPush;
    }

    public bool GetInputDown(InputName inputName)
    {
        bool isPush = false;

        for (int i = 0; i < inputStructs.Length; i++) {

            if (inputName == inputStructs[i].inputName) {

                isPush = isPushButtonsDown[i];
            }
        }

        return isPush;
    }

    private bool CheckInput(string[] inputNames)
    {
        bool isPush = false;

        for (int i = 0; i < inputNames.Length; i++) {

            isPush = Input.GetButton(inputNames[i]);
            if (isPush)
            {
                currentWaitTime = 0;
                break;
            }
        }

        return isPush;
    }

    private bool CheckInputDown(string[] inputNames)
    {
        bool isPush = false;

        for (int i = 0; i < inputNames.Length; i++) {

            isPush = Input.GetButtonDown(inputNames[i]);
            if (isPush) break;
        }

        return isPush;
    }

    [Serializable]
    public struct InputStruct {

        public InputName inputName;
        public InputButton[] buttons;
    }    
}
