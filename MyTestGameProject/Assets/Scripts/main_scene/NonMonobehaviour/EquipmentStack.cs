using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentStack : AStack, IDescriptionable
{
    [SerializeField] Item.MainProperties equipmentMainProperties;
    public Item.MainProperties EquipmentMainProperties { get { return equipmentMainProperties; } set { equipmentMainProperties = value; } }

    [SerializeField] EquipmentStats equipmentStats;
    public EquipmentStats EquipmentStats { get { return equipmentStats; } set { equipmentStats = value; } }

    [SerializeField] int count = 0;
    public int Count { get { return count; } }

    public override Item.MainProperties? MainProperties { get { return equipmentMainProperties; } }

    public override Item Item
    {
        get
        {
            return null;
        }
    }

    public EquipmentStack()
    {
    }

    public EquipmentStack(Item.MainProperties equipmentMainProperties, EquipmentStats equipmentStats, int count = 1)
    {
        this.equipmentMainProperties = equipmentMainProperties;
        this.equipmentStats = equipmentStats;
        this.count = count;
    }

    public EquipmentStack(Equipment item, int count = 1)
    {
        this.equipmentMainProperties = item.MainPropertie;
        this.equipmentStats = item.Stats;
        this.count = count;
    }

    public EquipmentStack(EquipmentStack itemStackToCopy)
    {
        this.equipmentMainProperties = itemStackToCopy.equipmentMainProperties;
        this.equipmentStats = itemStackToCopy.equipmentStats;
        this.count = itemStackToCopy.count;
    }

    public EquipmentStack(EquipmentStack itemStackToCopy, int count)
    {
        this.equipmentMainProperties = itemStackToCopy.equipmentMainProperties;
        this.equipmentStats = itemStackToCopy.equipmentStats;
        this.count = count;
    }


    public void PushItems(EquipmentStack stack)
    {
        if (stack.equipmentStats.Type == this.equipmentStats.Type 
            && stack.equipmentStats.Id == this.equipmentStats.Id
            && stack.equipmentStats.ItemDurability == this.equipmentStats.ItemDurability)
            this.count += stack.Count;
        else
            throw new System.Exception("Тип или Id предметов, которые Вы пытаетесь стакать, разные. Нельзя стакать разные предметы.");
    }

    public void PopItems(int count = 1)
    {
        if (count > this.count)
            throw new System.Exception("Нельзя взять больше предметов чем есть в стаке (есть " + this.count + ", пытаетесь взять " + count);

        this.count -= count;
    }

    public Description GetDescription()
    {
        var desc = equipmentStats.GetDescription();

        desc.Icon = equipmentMainProperties.Icon;
        desc.Name = Localization.GetString(equipmentMainProperties.StringResourceName);
        desc.Desc = Localization.GetString(equipmentMainProperties.StringResourceDescription);
        
        if (desc.Cost != null)
            desc.Cost = new Description.CostInfo()
            {
                CostPerOne = desc.Cost.Value.CostPerOne,
                CostAll = desc.Cost.Value.CostPerOne * count,
                CostCurrency = equipmentMainProperties.Currency
            };

        return desc;
    }
}
