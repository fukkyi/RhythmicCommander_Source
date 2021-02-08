using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingController : MonoBehaviour
{
    [SerializeField]
    private float contentIndent = 70f;
    [SerializeField]
    private float moveTime = 0.15f;

    [SerializeField]
    private Vector3 generateStartPosY = new Vector3(45f, -100f, 0);

    [SerializeField]
    private SelectInputController inputController = null;
    [SerializeField]
    private MusicDataController musicData = null;

    [SerializeField]
    private RankingContent generateContent = null;
    [SerializeField]
    private RectTransform contentTrans = null;
    [SerializeField]
    private RectTransform noDataText = null;

    [SerializeField]
    private Animator rankingAnim = null;

    private RankingContent[] contents = null;

    public bool isEneble { get; private set; }

    private bool isMoveing = false;
    private int currentContent = 0;
    private int contentLength = 0;
    private int showContentLength = 5;
    private float currentMoveTime = 0;
    private float currentContentPosY = 0;

    public void Init()
    {
        int generateCount = Constant.rankingLength;
        System.Array.Resize(ref contents, generateCount);
        contentTrans.sizeDelta = new Vector2(0, generateCount * contentIndent);

        for (int i = 0; i < generateCount; i++) {

            Vector3 insPos = generateStartPosY + (Vector3.down * contentIndent * i);
            contents[i] = Instantiate(generateContent, contentTrans, false);
            contents[i].transform.localPosition = insPos;
        }
    }

    public void ShowRankingUI()
    {
        isEneble = true;

        RankingStruct[] rankingStructs = musicData.GetSelectingMusicStruct().rankingStructs[(int)musicData.selectingDif];
        SetRanking(rankingStructs);

        rankingAnim.SetBool("Show", true);
        Find.GetAudioManager().PlayOneShotSE("Confrim_1");
    }

    public void UpdateManage()
    {
        if (!isEneble) return;

        bool cancel = inputController.GetInputDown(InputName.Cancel);

        if (cancel) {

            rankingAnim.SetBool("Show", false);
            isEneble = false;
            Find.GetAudioManager().PlayOneShotSE("Cancel");
        }

        MoveContents();
    }

    public void SetRanking(RankingStruct[] rankings)
    {
        foreach (RankingContent content in contents) {

            content.gameObject.SetActive(false);
        }

        contentLength = Mathf.Clamp(rankings.Length, 0, Constant.rankingLength);
        contentTrans.sizeDelta = new Vector2(0, contentLength * contentIndent);
        contentTrans.anchoredPosition = Vector2.zero;
        currentContent = 0;

        if (rankings.Length == 0) {

            contentLength = 0;
            noDataText.gameObject.SetActive(true);
            return;
        }
        else if (noDataText.gameObject.activeSelf) {

            noDataText.gameObject.SetActive(false);
        }

        for (int i = 0; i < contentLength; i++) {

            contents[i].SetContent(i + 1, rankings[i].name, rankings[i].score, rankings[i].clearType, musicData.clearTypeData);
            contents[i].gameObject.SetActive(true);
        }
    }

    private void MoveContents()
    {
        if (contentLength <= showContentLength) return;

        if (isMoveing) {

            currentMoveTime += Time.deltaTime;
            float movePosY = ((currentContent * contentIndent) - currentContentPosY) * (currentMoveTime / moveTime);

            if (currentMoveTime >= moveTime) {

                currentMoveTime = 0;
                contentTrans.anchoredPosition = Vector2.up * contentIndent * currentContent;
                isMoveing = false;
            }
            else {

                contentTrans.anchoredPosition = Vector2.up * (currentContentPosY + movePosY);
            }

            return;
        }

        bool selectUp = inputController.GetInput(InputName.ListUp);
        bool selectDown = inputController.GetInput(InputName.ListDown);

        if (selectUp || selectDown) {

            isMoveing = true;
            currentContentPosY = contentIndent * currentContent;

            if (selectDown && currentContent < contentLength - showContentLength) {

                Find.GetAudioManager().PlayOneShotSE("ListMove");
                currentContent++;
            }
            else if (selectUp && currentContent > 0) {

                Find.GetAudioManager().PlayOneShotSE("ListMove");
                currentContent--;
            }
        }
    }
}
