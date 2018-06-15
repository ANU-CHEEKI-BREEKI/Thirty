using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintPlayerTakesUnits : ATriggerConstraint
{
    int unitCnt = -1;

    public override bool IsTrue
    {
        get
        {
            if (Squad.playerSquadInstance != null)
            {
                var res = Squad.playerSquadInstance.UnitCount > unitCnt && unitCnt > 0;
                if (!res)
                    unitCnt = Squad.playerSquadInstance.UnitCount;
                return res;
            }
            else return false;
        }
    }
}