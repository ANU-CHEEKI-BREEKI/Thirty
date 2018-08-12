using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerClickHandler
{
    public event Action<PointerEventData, CustomButton> OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClick != null)
            OnClick(eventData, this);
    }
}
