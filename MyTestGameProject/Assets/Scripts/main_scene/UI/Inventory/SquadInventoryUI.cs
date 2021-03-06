﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadInventoryUI : AInventoryUI
{
    public static SquadInventoryUI Instance { get; private set; }

    [SerializeField] GameObject inventoryItemOriginal;

    [Header("Equipment")]
    [SerializeField] Transform helmetCell;
    [SerializeField] Transform bodyCell;
    [SerializeField] Transform shieldCell;
    [SerializeField] Transform weaponCell;
    [Header("Inventory")]
    [SerializeField] Transform[] inventoryCells;
    
    bool canRefresh = true;
    [Space]
    [SerializeField] float timeToRefresh = 0.2f;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
        Squad.playerSquadInstance.OnUitCountChanged += RefreshOnCntShanged;
    }

    private void OnDestroy()
    {
        if (Squad.playerSquadInstance != null)
            Squad.playerSquadInstance.OnUitCountChanged -= RefreshOnCntShanged;
    }

    void RefreshOnCntShanged(int cnt)
    {
        if (canRefresh)
            StartCoroutine(WaitForRefresh());
    }

    IEnumerator WaitForRefresh()
    {
        canRefresh = false;
        yield return new WaitForSeconds(timeToRefresh);
        canRefresh = true;
        RefreshUI();
    }

    override public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy)
            return;

        //Equipment
        int cnt = Squad.playerSquadInstance.UnitCount;
        SetImage(
            inventoryItemOriginal,
            helmetCell,
            new EquipmentStack(Squad.playerSquadInstance.Inventory.Helmet, cnt),
            !Squad.playerSquadInstance.Inventory.Helmet.EquipmentStats.Empty && CanDrag
        );
        SetImage(
            inventoryItemOriginal, 
            bodyCell,
            new EquipmentStack(Squad.playerSquadInstance.Inventory.Body, cnt),
            !Squad.playerSquadInstance.Inventory.Body.EquipmentStats.Empty && CanDrag
        );
        SetImage(
            inventoryItemOriginal, 
            shieldCell,
            new EquipmentStack(Squad.playerSquadInstance.Inventory.Shield, cnt),
            !Squad.playerSquadInstance.Inventory.Shield.EquipmentStats.Empty && CanDrag
        );
        SetImage(
            inventoryItemOriginal, 
            weaponCell,
            new EquipmentStack(Squad.playerSquadInstance.Inventory.Weapon, cnt),
            false
        );

        //Inventory
        cnt = inventoryCells.Length;
        for (int i = 0; i < cnt; i++)
        {
            if (Squad.playerSquadInstance.Inventory[i] != null)
            {
                if (Squad.playerSquadInstance.Inventory[i].Count > 0)
                {
                    SetImage(inventoryItemOriginal, inventoryCells[i], Squad.playerSquadInstance.Inventory[i], CanDrag);
                }
                else if (Squad.playerSquadInstance.Inventory[i].Count <= 0)
                {
                    SetImage(null, inventoryCells[i], null, false);
                    Squad.playerSquadInstance.Inventory[i] = null;
                }
            }
            else
            {
                SetImage(null, inventoryCells[i], null, false);
            }
        }
    }

    public override GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    {
        var go = base.SetImage(origin, cell, stack, canDrag);
        if (go != null)
        {
            var st = stack as EquipmentStack;
            var drag = go.GetComponent<DragEquipment>();
            var txt = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            //drag.EquipStack = st;
            if (!st.EquipmentStats.Empty)
                txt.text = drag.EquipStack.Count.ToString();
            else
                txt.enabled = false;
        }
        return go;
    }
}
