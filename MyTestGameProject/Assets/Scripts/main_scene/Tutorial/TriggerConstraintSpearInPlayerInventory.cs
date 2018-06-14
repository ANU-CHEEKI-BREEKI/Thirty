using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintSpearInPlayerInventory : ATriggerConstraint
{
    public override bool IsTrue
    {
        get
        {
            var w = Squad.playerSquadInstance.Inventory.Weapon;
            bool res = true;
            return w.Stats.Type == EquipmentStats.TypeOfEquipment.WEAPON 
                && res == w.Stats.CanReformToPhalanx 
                && res == w.Stats.CanReformToPhalanxInFight;
        }
    }
}
