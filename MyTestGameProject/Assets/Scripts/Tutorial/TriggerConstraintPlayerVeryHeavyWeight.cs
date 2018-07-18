using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintPlayerVeryHeavyWeight : ATriggerConstraint
{
    public override bool IsTrue
    {
        get
        {
            bool res = false;
            var s = Squad.playerSquadInstance;
            if (s != null)
                res = Extensions.GetWeightByMass(s.UnitStats.EquipmentMass) == UnitStats.EquipmentWeight.VERY_HEAVY;
            return res;
        }
    }
}
