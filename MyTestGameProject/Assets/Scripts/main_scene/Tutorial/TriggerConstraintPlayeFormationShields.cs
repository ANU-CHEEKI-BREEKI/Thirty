using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintPlayeFormationShields : ATriggerConstraint
{
    public override bool IsTrue
    {
        get
        {
            if (Squad.playerSquadInstance != null)
                return Squad.playerSquadInstance.CurrentFormation == FormationStats.Formations.RISEDSHIELDS;
            else
                return false;
        }
    }
}
