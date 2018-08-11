using UnityEngine;
using System.Collections.Generic;

public abstract class Skill : Executable
{
    [Header("Skill")]
    [SerializeField] AnimationCurve upgradeLevelCost;
    public AnimationCurve UpgradeLevelCost { get { return upgradeLevelCost; } }
}
