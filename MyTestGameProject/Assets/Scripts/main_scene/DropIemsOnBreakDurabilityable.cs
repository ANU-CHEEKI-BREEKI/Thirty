using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Durabilityable))]
public class DropIemsOnBreakDurabilityable : MonoBehaviour {

    Durabilityable durabilityable;

    [SerializeField] SOSquadSpawnerConsumablesResourse consumablesResourse;
    [SerializeField] SOSquadSpawnerEquipmentResourse equipmentResourse;
    [SerializeField] SOCount count;

    private void Awake()
    {
        durabilityable = GetComponent<Durabilityable>();
        durabilityable.OnBreak += Durabilityable_OnBreak;
    }

    private void OnDestroy()
    {
        if(durabilityable != null)
            durabilityable.OnBreak -= Durabilityable_OnBreak;
    }

    private void Durabilityable_OnBreak()
    {
        if(equipmentResourse != null)
        {
            int cnt = count.RandomCount;
            var eq = equipmentResourse.EquipmentByLevel;
            for (int i = 0; i < cnt; i++)
                DropingItemsManager.Instance.DropEquipment(eq, transform, 2);
        }

        Destroy(this);
    }
}
