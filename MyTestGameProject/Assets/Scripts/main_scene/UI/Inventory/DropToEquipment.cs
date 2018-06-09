﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToEquipment : ADropToMe
{
    override public void OnDrop(PointerEventData eventData)
    {
        DragEquipment drag = eventData.pointerDrag.GetComponent<DragEquipment>();

        if (drag != null && drag.CanDrag)
        {
            var oldParentDrop = drag.OldParent.GetComponent<ADropToMe>();
            if (oldParentDrop != this)
            {
                DragEquipment thisDrag = transform.GetChild(0).GetComponent<DragEquipment>(); 
                //в этой ячейке полюбому должен быть inventoryItem
                if (drag.EquipStack.EquipmentStats.Type == thisDrag.EquipStack.EquipmentStats.Type 
                    && (drag.EquipStack.EquipmentStats.Id != thisDrag.EquipStack.EquipmentStats.Id
                    || drag.EquipStack.EquipmentStats.ItemDurability != thisDrag.EquipStack.EquipmentStats.ItemDurability))
                {
                    //если предметов достаточно на весь отряд
                    if (Squad.playerSquadInstance.UnitCount <= drag.EquipStack.Count)
                    {
                        EquipmentStack stack = new EquipmentStack(drag.EquipStack.EquipmentMainProperties, drag.EquipStack.EquipmentStats, Squad.playerSquadInstance.UnitCount);
                        if (oldParentDrop.CanGetFromThisIventory(stack))
                            if (AddToThisInventory(stack))
                            {
                                oldParentDrop.RemoveFromThisInventory(stack);
                                if (!thisDrag.EquipStack.EquipmentStats.Empty)
                                    oldParentDrop.AddToThisInventory(thisDrag.EquipStack);
                            }

                        Destroy(drag.gameObject);
                        RefreshUI();
                    }
                    else
                    {
                        Toast.Instance.Show(Localization.toast_not_enough_equipment_count);
                    }
                }
            }
        }
    }

    public override bool AddToThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        var stats = stack.EquipmentStats;
        Equipment eq = (Resources.Load(stack.EquipmentMainProperties.PathToPrefab) as GameObject).GetComponent<Equipment>();
        eq.Stats = stats;

        if (stats.Type == EquipmentStats.TypeOfEquipment.HEAD)
        {
            Squad.playerSquadInstance.Inventory.Helmet = eq;
        }
        else if (stats.Type == EquipmentStats.TypeOfEquipment.SHIELD)
        {
            if (Squad.playerSquadInstance.Inventory.Weapon.Stats.CanUseWithShield)
                Squad.playerSquadInstance.Inventory.Shield = eq;
            else
            {
                Toast.Instance.Show(Localization.toast_cant_use_with_current_weapon);
                return false;
            }
        }
        else if (stats.Type == EquipmentStats.TypeOfEquipment.BODY)
        {
            Squad.playerSquadInstance.Inventory.Body = eq;
        }
        else if (stats.Type == EquipmentStats.TypeOfEquipment.WEAPON)
        {
            if (stats.CanUseWithShield || Squad.playerSquadInstance.Inventory.Shield.Stats.Empty)
                Squad.playerSquadInstance.Inventory.Weapon = eq;
            else
            {
                Toast.Instance.Show(Localization.toast_cant_use_with_shield);
                return false;
            }
        }

        return true;
    }

    public override bool RemoveFromThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        var stats = stack.EquipmentStats;

        //очищаем инвентарь
        if (stats.Type == EquipmentStats.TypeOfEquipment.HEAD)
            Squad.playerSquadInstance.Inventory.Helmet = null;
        else if (stats.Type == EquipmentStats.TypeOfEquipment.SHIELD)
            Squad.playerSquadInstance.Inventory.Shield = null;
        else if (stats.Type == EquipmentStats.TypeOfEquipment.BODY)
            Squad.playerSquadInstance.Inventory.Body = null;
        else if (stats.Type == EquipmentStats.TypeOfEquipment.WEAPON)
            throw new Exception("Нельзя выбрасывать оружие вручную");

        return true;
    }

    public override bool CanGetFromThisIventory(AStack aStack)
    {
        return true;
    }
}
