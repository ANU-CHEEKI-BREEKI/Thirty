using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_SM_")]
public class SOStatsModifier : ScriptableObject
{
    [ContextMenuItem("ReserModifiers", "ReserModifiers")]
    [SerializeField] UnitStatsModifier modifier;
    public UnitStatsModifier Modifier { get { return modifier; } }

    void ReserModifiers()
    {
        modifier = modifier.Reset();
    }
}