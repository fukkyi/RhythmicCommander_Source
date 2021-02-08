using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Option : MonoBehaviour
{
    [SerializeField][TextArea]
    protected string description;

    [SerializeField]
    protected RectTransform detailParent = null;
    [SerializeField]
    protected TextMeshProUGUI valueText;

    public virtual void Init()
    {

    }

    public virtual void ChangeValue(bool up, bool down, bool submit)
    {

    }

    public virtual void Selected()
    {

    }

    public virtual void UnSelected()
    {

    }

    public string GetDescription()
    {
        return description;
    }
}
