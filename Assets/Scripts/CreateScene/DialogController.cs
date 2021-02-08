using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [SerializeField]
    private RectTransform exitDialog = null;

    private bool isBackedTitle = false;

    public void UpdateMange()
    {
        if (Input.GetButtonDown("Cancel")) {

            if (exitDialog.gameObject.activeSelf) return;
            ShowExitDialog(true);
        }
    }

    public void ShowExitDialog(bool show)
    {
        exitDialog.gameObject.SetActive(show);
    }

    public void BackTitleScene()
    {
        if (isBackedTitle) return;
        bool isWork = Find.GetSceneLoadManager().LoadTitleScene();

        if (!isWork) return;

        Find.GetAudioManager().PlayOneShotSE("Confrim_2");
        ShowExitDialog(false);
        isBackedTitle = true;
    }
}
