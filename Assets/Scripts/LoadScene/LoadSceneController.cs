using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneController : MonoBehaviour
{
    [SerializeField]
    private GameObject cookpitCamera = null;
    [SerializeField]
    private GameObject cookpit = null;

    [SerializeField]
    private GameObject starObj = null;
    [SerializeField]
    private Transform insTrans = null;
    [SerializeField]
    private Animator loadAnim = null;

    [SerializeField]
    private int starCount = 4;
    [SerializeField]
    private float starInterval = 15;
    [SerializeField]
    private float starSpeed = 1;

    private Transform[] stars = null;
    private SpriteRenderer[][] starRenderers = null;

    public Color starColor;

    void Start()
    {
        Init();
    }

    void Update()
    {
        MoveStars();
    }

    private void Init()
    {
        System.Array.Resize(ref stars, starCount);
        System.Array.Resize(ref starRenderers, starCount);

        for (int i = 0; i < starCount; i++) {

            stars[i] = Instantiate(starObj, insTrans).transform;

            starRenderers[i] = stars[i].GetComponentsInChildren<SpriteRenderer>();

            stars[i].localPosition = Vector3.forward * 15 * (i - 1);
            stars[i].localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        SetActiveCookpit(false);
    }

    private void MoveStars()
    {
        for (int i = 0; i < stars.Length; i++) {

            stars[i].localPosition -= Vector3.forward * Time.deltaTime * starSpeed;

            for (int j = 0; j < starRenderers[i].Length; j++) {
                starRenderers[i][j].color = starColor;
            }

            if (stars[i].localPosition.z < -15) {

                stars[i].localPosition = Vector3.forward * 15 * (starCount - 1);
                stars[i].localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            }
        }
    }

    public void AnimStars(bool show)
    {
        loadAnim.SetBool("Show", show);
    }

    public IEnumerator WaitAnimStars(bool show, bool isShowStar = true)
    {
        if (!isShowStar && show) {
            loadAnim.SetTrigger("NonStar");
        }

        loadAnim.SetBool("Show", show);


        yield return null;

        while (loadAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) {

            yield return null;
        }
    }

    public void SetActiveCookpit(bool active)
    {
        cookpitCamera.SetActive(active);
        cookpit.SetActive(active);
    }
}
