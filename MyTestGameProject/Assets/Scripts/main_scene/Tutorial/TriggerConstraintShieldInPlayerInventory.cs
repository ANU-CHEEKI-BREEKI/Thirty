using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintShieldInPlayerInventory : ATriggerConstraint
{
    public override bool IsTrue
    {
        get
        {
            if (Squad.playerSquadInstance != null)
            {
                var s = Squad.playerSquadInstance.Inventory.Shield;
                return !s.EquipmentStats.Empty && s.EquipmentStats.Type == EquipmentStats.TypeOfEquipment.SHIELD;
            }
            else return false;
        }
    }
}

