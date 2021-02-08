using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeText : MonoBehaviour
{
    private Animator showAnim = null;
    private SpriteRenderer spriteRenderer = null;

    private const string playAnimName = "Show";

    public bool isShow = false;

    void Awake()
    {
        showAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateManage()
    {
        if (!IsPlaying())
        {
            transform.localPosition = Vector3.zero;
            isShow = false;
        }
    }

    public bool IsPlaying()
    { 
        AnimatorStateInfo animState = showAnim.GetCurrentAnimatorStateInfo(0);
        return animState.shortNameHash.Equals(Animator.StringToHash(playAnimName));
    }

    public void playAnim(Transform judgeLine, Sprite textSprite)
    {
        spriteRenderer.sprite = textSprite;
        transform.position = judgeLine.position;
        showAnim.SetTrigger(playAnimName);

        isShow = true;
    }
}
