using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAllDropOnBreakComponents : MonoBehaviour
{
    [ContextMenu("Execute")]
    private void Awake()
    {
        Execute();
        DestroyImmediate(this, true);
    }

    void Execute()
    {
        var m = DropOnBreakResourcesManager.Instance;
        if (m == null)
            throw new System.Exception("Менеджер для ресурсов сначала инициализировать надо");

        var allDrops = Tools.Others.GetAllComponentsWithAllChildrens<DropIemsOnBreakDurabilityable>(transform);
        foreach (var d in allDrops)
        {
            d.equipmentResourse = m.equipmentResourse;
            d.equipmentCount = m.equipmentCount;

            d.consumablesResourse = m.consumablesResourse;
            d.consumableCount = m.consumableCount;

            d.moneyResourse = m.moneyResourse;
            d.moneyCount = m.moneyCount;
        }
    }
}
