using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DropOnBreakResourcesManager : MonoBehaviour
{
    public static DropOnBreakResourcesManager Instance { get; private set; }

    public SOSquadSpawnerConsumablesResourse consumablesResourse;
    public SOCount consumableCount;
    [Space]
    public SOSquadSpawnerEquipmentResourse equipmentResourse;
    public SOCount equipmentCount;
    [Space]
    public SOSquadSpawnerMoneyResourse moneyResourse;
    public SOCount moneyCount;

    [ContextMenu("Init")]
    private void Awake()
    {
        Instance = this;
    }
}
