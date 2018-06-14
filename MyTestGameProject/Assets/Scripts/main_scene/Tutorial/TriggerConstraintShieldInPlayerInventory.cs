using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintShieldInPlayerInventory : ATriggerConstraint
{
    public override bool IsTrue
    {
        get
        {
            var s = Squad.playerSquadInstance.Inventory.Shield;
            return !s.Stats.Empty && s.Stats.Type == EquipmentStats.TypeOfEquipment.SHIELD;
        }
    }
}

