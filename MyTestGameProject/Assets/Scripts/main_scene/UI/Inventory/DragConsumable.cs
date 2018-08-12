using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragConsumable : Drag
{
    [SerializeField] ConsumableStack consumableStack;
    public ConsumableStack ConsumableStack { get { return consumableStack; } set { consumableStack = value; } }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if(CanCallClick)
            TipsPanel.Instance.Show(consumableStack.GetDescription(), thisTransform.position);
    }

    protected override void OnCantDrag()
    {
        base.OnCantDrag();
    }
}
