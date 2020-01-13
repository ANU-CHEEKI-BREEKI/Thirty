using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    /// <summary>
    /// Возвращаемое значение будет указыать нужно ли выполнять функцию OnPointerClick
    /// </summary>
    public event Func<Drag, bool> BeforeClick;

    bool canCallClick = false;
    protected bool CanCallClick { get { return canCallClick; } }

    protected Transform thisTransform;
    Transform canvasTransform;
    public Transform OldParent { get; private set; }
    CanvasGroup canvasGroup;

    abstract public AStack Stack { get; set; }

    public bool CanDrag { get; set; }

    void Start()
    {
        thisTransform = transform;
        canvasTransform = thisTransform.GetComponentInParentUpToHierarchy<Canvas>().transform;
        canvasGroup = thisTransform.GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            int count = Input.touchCount;
            Touch touch;
            if (count > 0)
            {
                touch = Input.GetTouch(Input.touchCount - 1);
            }
            else
            {
                touch = new Touch();
                touch.position = Input.mousePosition;
            }

            thisTransform.position = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
        }

        TipsPanel.Instance.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            OldParent = thisTransform.parent;
            thisTransform.SetParent(canvasTransform);
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            OnCantDrag();
        }
    }

    protected virtual void OnCantDrag()
    {
        Toast.Instance.Show(LocalizedStrings.toast_cant_drop);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            if (thisTransform.parent == canvasTransform)
            {
                if (OldParent.childCount == 0)
                    thisTransform.SetParent(OldParent);
                else
                    Destroy(gameObject);
            }
            canvasGroup.blocksRaycasts = true;
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (BeforeClick != null)
            canCallClick = BeforeClick(this);
        else
            canCallClick = true;
    }

    public abstract void Present();
}
