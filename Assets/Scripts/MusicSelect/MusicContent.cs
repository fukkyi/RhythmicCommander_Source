using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MusicContent : MonoBehaviour
{
    [SerializeField]
    private Animator selectAnim = null;
    [SerializeField]
    private int _contentNum = 0;
    [SerializeField]
    private int _musicNum = 0;
    private float _nextPos = 0;
    private float _basePos = 0;

    [SerializeField]
    TextMeshProUGUI titleText = null;
    [SerializeField]
    TextMeshProUGUI composerText = null;
    [SerializeField]
    TextMeshProUGUI levelText = null;
    [SerializeField]
    private Image lineImage = null;

    public int contentNum { set { if (value >= 0) _contentNum = value; } get { return _contentNum; } }
    public int musicNum { set { if (value >= 0) _musicNum = value; } get { return _musicNum; } }
    public float nextPos { set { _nextPos = value; } get { return _nextPos; } }

    public float basePos { set { _basePos = value; } get { return _basePos; } }

    public void SetNextContentNum(bool up, bool down, int contentLength)
    {
        if (down) {

            if (contentNum == 0) {

                contentNum = contentLength - 1;
                transform.localPosition = Vector3.up * -10;
            }
            else {

                contentNum--;
            }            
        }
        else if (up) {

            if (contentNum == contentLength - 1) {

                contentNum = 0;
                transform.localPosition = Vector3.up * 10;
            }
            else {

                contentNum++;
            }
        }

        SetSelectingAnim(contentLength);
    }

    public void ClacNextPos(int contentLength, float contentInterval)
    {
        int halfLength = (contentLength - 1) / 2;

        nextPos = (contentNum - halfLength) * -contentInterval;
        basePos = transform.localPosition.y;
    }

    public void MovePosFromY(float posY)
    {
        transform.localPosition = Vector3.up * posY;
    }

    public void SetContentUI(MusicInfoStruct musicInfoStruct, Difficulty difficulty, Color levelColor)
    {
        titleText.SetText(musicInfoStruct.title);
        lineImage.gameObject.SetActive(false);
        composerText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        composerText.SetText(musicInfoStruct.composer);
        levelText.SetText(GetLevelString(musicInfoStruct.levels[(int)difficulty]));
        levelText.color = levelColor;
    }

    public void SetContentUIToCategory()
    {
        titleText.SetText("All Music");
        lineImage.gameObject.SetActive(true);
        composerText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
    }

    public string GetLevelString(float? level)
    {
        string levelString = string.Empty;

        if (level == null) {

            levelString = "-.--";
        }
        else {

            levelString = ((float)level).ToString("F2");
        }

        return levelString;
    }

    public void SetSelectingAnim(int contentLength) {

        if (contentNum == (contentLength - 1) / 2) {

            selectAnim.SetBool("Selecting", true);
        }
        else {

            selectAnim.SetBool("Selecting", false);
        }
    }

    public void PlayHideAnim(bool hide)
    {
        selectAnim.SetBool("Hide", hide);
    }
}
