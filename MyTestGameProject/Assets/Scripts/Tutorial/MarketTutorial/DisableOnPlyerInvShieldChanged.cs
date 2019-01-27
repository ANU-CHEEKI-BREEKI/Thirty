using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Это скрипт-костыль для обучения в магазине
/// </summary>
public class DisableOnPlyerInvShieldChanged : MonoBehaviour
{
    void Start()
    {
        try
        {
            Squad.playerSquadInstance.Inventory.OnEquipmentChanged += Inventory_OnEquipmentChanged;
        }
        catch { }
    }

    void OnDestroy()
    {
        try
        {
            Squad.playerSquadInstance.Inventory.OnEquipmentChanged -= Inventory_OnEquipmentChanged;
        }
        catch { }
    }

    void Inventory_OnEquipmentChanged(EquipmentStack obj)
    {
        if (obj != null && !obj.EquipmentStats.Empty && obj.EquipmentStats.Type == EquipmentStats.TypeOfEquipment.SHIELD)
            gameObject.SetActive(false);
    }
}
