using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UiEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public UnityEvent OnDown;
    public UnityEvent OnUp;
    public UnityEvent OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnUp.Invoke();
    }
}
