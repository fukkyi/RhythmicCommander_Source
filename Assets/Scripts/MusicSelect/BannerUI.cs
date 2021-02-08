using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BannerUI : MonoBehaviour
{
    [SerializeField]
    private UnityEvent selectEvent;
    [SerializeField]
    private Animator selectAnim;

    public void SetSelectingAnim(bool play)
    {
        selectAnim.SetBool("Selecting", play);
    }

    public void Select()
    {
        selectAnim.SetTrigger("Select");

        if (selectEvent != null) {
            selectEvent.Invoke();
        }
    }
}
