using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchivementStandToTheLast
{
    static bool playerSquadInFight = false;
    public static int killsCount;
    public static bool PlayerSquadInFight
    {
        set
        {
            playerSquadInFight = value;
            if (!value)
                killsCount = 0;
        }
    }
}
