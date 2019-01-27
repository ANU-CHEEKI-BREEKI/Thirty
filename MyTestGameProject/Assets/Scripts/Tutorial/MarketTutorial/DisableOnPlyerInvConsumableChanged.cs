using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Это скрипт-костыль для обучения в магазине
/// </summary>
public class DisableOnPlyerInvConsumableChanged : MonoBehaviour
{
    void Start()
    {
        try
        {
            Squad.playerSquadInstance.Inventory.FirstConsumable.OnConsumableChanged += OnConsumableChanged;
            Squad.playerSquadInstance.Inventory.SecondConsumable.OnConsumableChanged += OnConsumableChanged;
        }
        catch { }
    }

    void OnDestroy()
    {
        try
        {
            Squad.playerSquadInstance.Inventory.FirstConsumable.OnConsumableChanged -= OnConsumableChanged;
            Squad.playerSquadInstance.Inventory.SecondConsumable.OnConsumableChanged -= OnConsumableChanged;
        }
        catch { }
    }

    private void OnConsumableChanged(Consumable obj)
    {
        if (obj != null)
            gameObject.SetActive(false);
    }
}
