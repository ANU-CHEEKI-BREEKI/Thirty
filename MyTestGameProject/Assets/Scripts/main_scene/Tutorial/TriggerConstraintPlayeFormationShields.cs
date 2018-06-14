using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintPlayeFormationShields : ATriggerConstraint
{
    public override bool IsTrue
    {
        get
        {
            return Squad.playerSquadInstance.CurrentFormation == FormationStats.Formations.RISEDSHIELDS;
        }
    }
}
