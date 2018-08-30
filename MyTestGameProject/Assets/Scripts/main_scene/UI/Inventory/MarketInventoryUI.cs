using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketInventoryUI : AInventoryUI
{
    [SerializeField] GameObject inventoryItemOriginal;
    [Space]
    [Tooltip("Родительский объект для ячеек предметов. КРОМЕ ячеек внем не должно быть НИЧЕГО.")]
    [SerializeField] Transform itemsContainer;
        
    List<Transform> items;
    List<EquipmentStack> inventory;

    public bool Fill { get { return inventory.Count == items.Count; } }

    public static MarketInventoryUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        inventory = new List<EquipmentStack>();

        //получаем ссылки контейнеров для итемов
        int cnt = itemsContainer.childCount;
        items = new List<Transform>(itemsContainer.childCount);
        for (int i = 0; i < cnt; i++)
            items.Add(itemsContainer.GetChild(i));
    }

    private void Start()
    {
        RefreshUI();
    }

    public void AddToInventory(EquipmentStack stack)
    {
        bool t = false;
        foreach (var inv in inventory)
        {
            if (stack.EquipmentStats.Type == inv.EquipmentStats.Type 
                && stack.EquipmentStats.Id == inv.EquipmentStats.Id 
                && stack.EquipmentStats.ItemDurability == inv.EquipmentStats.ItemDurability)
            {
                t = true;
                inv.PushItems(stack);
                break;
            }
        }

        if(!t)
        {
            if (inventory.Count < items.Count)
                inventory.Add(stack);
            else
                throw new System.Exception("больше нет места в инвентаре");
        }
        
    }

    public void RemoveFtomInventory(EquipmentStack stack)
    {
        bool t = false;
        EquipmentStack toRemove = null;
        foreach (var inv in inventory)
        {
            if (stack.EquipmentStats.Type == inv.EquipmentStats.Type 
                && stack.EquipmentStats.Id == inv.EquipmentStats.Id
                && stack.EquipmentStats.ItemDurability == inv.EquipmentStats.ItemDurability)
            {
                t = true;
                inv.PopItems(stack.Count);
                if (inv.Count == 0)
                    toRemove = inv;
                break;
            }
        }
        inventory.Remove(toRemove);

        if (!t)
            throw new System.Exception("Алё. Таких предметов в магазине нет!");

    }

    override public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy)
            return;

        int cnt = inventory.Count;
        for (int i = 0; i < cnt; i++)
        {
            SetImage(inventoryItemOriginal, items[i], inventory[i], true);
        }

        int cnt2 = items.Count;
        if(cnt2 > cnt)
            for (int i = cnt; i < cnt2; i++)
                SetImage(null, items[i], null, false);
    }

    public override GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    {
        var go = base.SetImage(origin, cell, stack, canDrag);
        if (go != null)
        {
            var drag = go.GetComponent<DragEquipment>();

            drag.showNewEquipment = true;
            drag.Stack = stack;//нужно чтобы инициализировать значек показывающий новую экипировку
            drag.Present();
        }
        return go;
    }
}
