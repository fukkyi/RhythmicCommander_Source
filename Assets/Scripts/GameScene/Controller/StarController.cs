using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    [SerializeField]
    private TimeController timeController = null;

    [SerializeField]
    private Transform[] parentTrans = null;
    private Transform[] starTrans = null;

    [SerializeField]
    private int starGenerateCount = 3;
    [SerializeField]
    private float starInterval = 150;
    [SerializeField]
    private float starSpeed = 10;

    [SerializeField]
    private GameObject starObj = null;

    public IEnumerator Init()
    {
        float nextFlameTime = Clac.NextFlameTime();

        System.Array.Resize(ref starTrans, starGenerateCount * parentTrans.Length);

        int generateCount = 0;
        for (int i = 0; i < parentTrans.Length; i++) {

            for (int j = 0; j < starGenerateCount; j++) {

                // 処理時間がフレームレートの80%を超えたら次のフレームへ
                if (Time.realtimeSinceStartup >= nextFlameTime) {

                    yield return null;
                    nextFlameTime = Clac.NextFlameTime();
                }

                Vector3 genetatePos = new Vector3(0, 
                    starInterval * j, 0);

                starTrans[generateCount] = Instantiate(starObj, parentTrans[i]).transform;
                starTrans[generateCount].localPosition = genetatePos;

                generateCount++;
            }
        }
    }

    public void UpdateManage()
    {
        float bpm = timeController.NowBpm;

        for (int i = 0; i < starTrans.Length; i++) {

            starTrans[i].localPosition += Vector3.down * bpm * starSpeed / (Application.targetFrameRate / 2);

            if (starTrans[i].localPosition.y > -starInterval) continue;

            float posY = starTrans[i].localPosition.y + starInterval;

            starTrans[i].localPosition = (Vector3.up * starInterval * (starGenerateCount - 1)) + Vector3.up * posY;
            starTrans[i].localRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        }
    }

    public void HideStars()
    {
        foreach (Transform star in starTrans) {

            star.gameObject.GetComponent<Animator>().SetTrigger("Hide");
        }
    }
}
