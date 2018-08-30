﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragConsumable : Drag
{
    [SerializeField] ConsumableStack consumableStack;
    public ConsumableStack ConsumableStack { get { return consumableStack; } set { consumableStack = value; } }

    public override AStack Stack
    {
        get
        {
            return ConsumableStack;
        }
        set
        {
            if (value is ConsumableStack)
                ConsumableStack = value as ConsumableStack;
            else
                throw new ArgumentException("не тот стак засунуть пытаешься.");
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if(CanCallClick)
            TipsPanel.Instance.Show(consumableStack.GetDescription(), thisTransform.position);
    }

    public override void Present()
    {
        throw new NotImplementedException();
    }

    protected override void OnCantDrag()
    {
        base.OnCantDrag();
    }
}
