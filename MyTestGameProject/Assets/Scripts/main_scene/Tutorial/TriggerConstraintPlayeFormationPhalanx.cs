using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintPlayeFormationPhalanx : ATriggerConstraint
{
    public override bool IsTrue
    {
        get
        {
            return Squad.playerSquadInstance.CurrentFormation == FormationStats.Formations.PHALANX;
        }
    }
}
