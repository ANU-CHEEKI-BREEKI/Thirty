using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipment : Item
{
    [Header("Equipment properties")]
    [SerializeField] EquipmentStats stats;
    public EquipmentStats Stats { get { return stats; } set { stats = value; } } 
}
