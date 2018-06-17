using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintSpearInPlayerInventory : ATriggerConstraint
{
    public override bool IsTrue
    {
        get
        {
            if (Squad.playerSquadInstance != null)
            {
                var w = Squad.playerSquadInstance.Inventory.Weapon;
                bool res = true;
                return w.EquipmentStats.Type == EquipmentStats.TypeOfEquipment.WEAPON
                    && res == w.EquipmentStats.CanReformToPhalanx
                    && res == w.EquipmentStats.CanReformToPhalanxInFight;
            }
            else return false;
        }
    }
}
