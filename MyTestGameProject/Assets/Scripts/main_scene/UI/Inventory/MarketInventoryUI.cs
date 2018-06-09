﻿using System.Collections;
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

    [Header("Автозаполнение магазина")]
    [SerializeField] bool autoFillMarket = true;
    [SerializeField] Equipment[] originalEquipment;
    [Space]
    [SerializeField] AnimationCurve dependanceOfGameLevel;
    [SerializeField] int minLevel = 1;
    [SerializeField] int maxLevel = 20;
    
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
        AutoFill();
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
        if (!gameObject.activeSelf)
            return;

        int cnt = inventory.Count;
        for (int i = 0; i < cnt; i++)
        {
            if(items[i].childCount > 0)
                Destroy(items[i].GetChild(0).gameObject);
            SetImage(inventoryItemOriginal, items[i], inventory[i], true);
        }

        int cnt2 = items.Count;
        if(cnt2 > cnt)
            for (int i = cnt; i < cnt2; i++)
                if (items[i].childCount > 0)
                    Destroy(items[i].GetChild(0).gameObject);
    }

    public override GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    {
        var go = base.SetImage(origin, cell, stack, canDrag);
        if (go != null)
        {
            var st = stack as EquipmentStack;
            var drag = go.GetComponent<DragEquipment>();
            var txt = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            drag.EquipStack = st;
            txt.text = drag.EquipStack.Count.ToString();
        }
        return go;
    }

    void AutoFill()
    {
        int cnt = originalEquipment.Length;

        int cnt2 = itemsContainer.childCount - 6;
        for(int i = 0; i < cnt2; i++)
        {
            Equipment eq = originalEquipment[Random.Range(0, cnt)];
            EquipmentStats qs = eq.Stats;

            int r = Random.Range(0, 3);
            if(r == 0)
            {
                qs.ItemDurability = EquipmentStats.Durability.NEW;
            }else if(r == 1)
            {
                qs.ItemDurability = EquipmentStats.Durability.DAMAGED;
            }
            else if(r == 2)
            {
                qs.ItemDurability = EquipmentStats.Durability.WORN;
            }
            EquipmentStack eqs = new EquipmentStack(eq.MainPropertie, qs, Random.Range(10, 100));
            AddToInventory(eqs);
        }
    }
}
