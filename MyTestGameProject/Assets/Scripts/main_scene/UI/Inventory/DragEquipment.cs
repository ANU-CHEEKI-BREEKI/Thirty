using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragEquipment : Drag
{
    [SerializeField] EquipmentStack equipStack;
    public EquipmentStack EquipStack { get { return equipStack; } set { equipStack = value; } }

    public override AStack Stack
    {
        get
        {
            return EquipStack;
        }
        set
        {
            if (value is EquipmentStack)
                EquipStack = value as EquipmentStack;
            else
                throw new ArgumentException("не тот стак засунуть пытаешься.");
        }
    }

    protected override void OnCantDrag()
    {
        if (EquipStack.EquipmentStats.Type == EquipmentStats.TypeOfEquipment.WEAPON)
            Toast.Instance.Show(LocalizedStrings.toast_cant_drop_weapon);
        else
            base.OnCantDrag();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (CanCallClick)
            TipsPanel.Instance.Show(EquipStack.GetDescription(), thisTransform.position);
    }
}
