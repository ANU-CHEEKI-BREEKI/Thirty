using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class McToggleTypeOfBlock : 
    MonoBehaviour,
    IPointerClickHandler
{
    Toggle[] otherToggles;
    Toggle thisToggle;
    static Toggle lastPressedToggle;
    public static Toggle LastPressedToggle
    {
        get
        {
            return lastPressedToggle;
        }
    }

    static int height;
    public static int Height
    {
        get
        {
            return height;
        }
    }

    static int width;    
    public static int Width
    {
        get
        {
            return width;
        }
    }

    static bool blocksSelected;
    public static bool BlocksSelected
    {
        get
        {
            return blocksSelected;
        }
    }
    
    [SerializeField] TypeOfBlock type;
    public TypeOfBlock Type
    {
        get
        {
            return type;
        }
    }
    
    [SerializeField] public enum TypeOfBlock { DOT, VPLANE, HPLANE, SQUARE }

    void Start ()
    {
        otherToggles = gameObject.transform.parent.GetComponentsInChildren<Toggle>();
        thisToggle = gameObject.GetComponent<Toggle>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        lastPressedToggle = thisToggle;

        foreach (var toggle in otherToggles)
            if (toggle != thisToggle)
                toggle.isOn = false;

        if(thisToggle.isOn)
        {
            SetSize();
            blocksSelected = true;
        }else
        {
            blocksSelected = false;
        }
    }

    void SetSize()
    {
        switch (type)
        {
            case TypeOfBlock.DOT:
                width = 1;
                height = 1;
                break;
            case TypeOfBlock.VPLANE:
                width = 1;
                height = 2;
                break;
            case TypeOfBlock.HPLANE:
                width = 2;
                height = 1;
                break;
            case TypeOfBlock.SQUARE:
                width = 2;
                height = 2;
                break;
        }
    }

    public static void DeselectAll()
    {
        lastPressedToggle.isOn = false;
        blocksSelected = false;
    }
}
