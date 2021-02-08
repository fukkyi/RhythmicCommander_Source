using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneBeam : MonoBehaviour
{

    public bool isShow = false;

    private SpriteRenderer spriteRenderer = null;
    private Vector3 showPos = new Vector3(0, 4.15f, 0);
    private Vector3 hidePos = new Vector3(0, -100, 0);

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localPosition = hidePos;
    }

    public void SetColor(Color beamColor)
    {
        spriteRenderer.color = beamColor;
    }

    public void Show()
    {     
        transform.localPosition = showPos;
        isShow = true;
    }

    public void Hide()
    {
        transform.localPosition = hidePos;
        isShow = false;
    }
}
