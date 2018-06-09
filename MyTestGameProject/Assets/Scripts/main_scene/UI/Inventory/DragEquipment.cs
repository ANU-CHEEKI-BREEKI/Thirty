﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragEquipment : Drag
{   
    public EquipmentStack EquipStack { get; set; }

    protected override void OnCantDrag()
    {
        if (EquipStack.EquipmentStats.Type == EquipmentStats.TypeOfEquipment.WEAPON)
            Toast.Instance.Show(Localization.toast_cant_drop_weapon);
        else
            base.OnCantDrag();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        TipsPanel.Instance.Show(EquipStack.GetDescription(), thisTransform.position);
    }
}
