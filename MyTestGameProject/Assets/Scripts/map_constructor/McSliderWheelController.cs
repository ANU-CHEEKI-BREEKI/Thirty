using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class McSliderWheelController : 
    MonoBehaviour,
    IScrollHandler
{
    Slider slider;
    [SerializeField] float step = 1;

    public void OnScroll(PointerEventData eventData)
    {
        slider.value += step * eventData.scrollDelta.y;
    }

    // Use this for initialization
    void Start ()
    {
        slider = gameObject.GetComponent<Slider>();
	}
}
