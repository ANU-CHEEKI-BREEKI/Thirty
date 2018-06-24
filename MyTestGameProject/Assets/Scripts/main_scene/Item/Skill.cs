using UnityEngine;
using System.Collections;

public abstract class Skill : Executable
{
    [Header("Skill")]
    [SerializeField] AnimationCurve upgradeLevelCost;
    public AnimationCurve UpgradeLevelCost { get { return upgradeLevelCost; } }
}
