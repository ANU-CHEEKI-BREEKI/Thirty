using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Durabilityable))]
public class DropIemsOnBreakDurabilityable : MonoBehaviour
{
    Durabilityable durabilityable;

    public SOSquadSpawnerConsumablesResourse consumablesResourse;
    public SOCount consumableCount;
    [Space]
    public SOSquadSpawnerEquipmentResourse equipmentResourse;
    public SOCount equipmentCount;
    [Space]
    public SOSquadSpawnerMoneyResourse moneyResourse;
    public SOCount moneyCount;

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
        var rnd = Random.Range(0,2);

        if (rnd == 0)
        {
            if (equipmentResourse != null)
            {
                int cnt = equipmentCount.RandomCount;
                var eq = equipmentResourse.EquipmentByLevel;
                for (int i = 0; i < cnt; i++)
                    DropingItemsManager.Instance.DropEquipment(eq, transform, 2);
            }
        }
        else if (rnd == 1)
        {
            if (moneyResourse != null)
            {
                var m = moneyResourse.MoneyByLevel;
                var cnt = moneyCount.CountByLevel;
                if (m.Currency == DSPlayerScore.Currency.GOLD)
                {
                    cnt /= 50;
                    if (cnt == 0) cnt = 1;
                }
                m.Use(cnt);
            }
        }

        Destroy(this);
    }
}
