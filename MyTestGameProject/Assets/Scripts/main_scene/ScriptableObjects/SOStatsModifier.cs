using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_SM_")]
public class SOStatsModifier : ScriptableObject
{
    [SerializeField] UnitStatsModifier modifier;
    public UnitStatsModifier Modifier { get { return modifier; } }
}