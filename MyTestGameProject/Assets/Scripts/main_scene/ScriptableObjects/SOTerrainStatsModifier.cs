using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "SO_TSM_")]
public class SOTerrainStatsModifier : ScriptableObject
{
    [SerializeField] SOStatsModifier veryLightWeightUnit;
    [SerializeField] SOStatsModifier lightWeightUnit;
    [SerializeField] SOStatsModifier mediumWeightUnit;
    [SerializeField] SOStatsModifier heavyWeightUnit;
    [SerializeField] SOStatsModifier veryHeavyWeightUnit;

    [SerializeField] bool needMask;
    public bool NeedMask { get { return needMask; } }

    public UnitStatsModifier GetModifierByEquipmentMass(UnitStats.EquipmentWeight equipmentMass)
    {
        var res = new UnitStatsModifier();
        switch (equipmentMass)
        {
            case UnitStats.EquipmentWeight.VERY_LIGHT:
                res = veryLightWeightUnit.Modifier;
                break;
            case UnitStats.EquipmentWeight.LIGHT:
                res = lightWeightUnit.Modifier;
                break;
            case UnitStats.EquipmentWeight.MEDIUM:
                res = mediumWeightUnit.Modifier;
                break;
            case UnitStats.EquipmentWeight.HEAVY:
                res = heavyWeightUnit.Modifier;
                break;
            case UnitStats.EquipmentWeight.VERY_HEAVY:
                res = veryHeavyWeightUnit.Modifier;
                break;
        }
        return res;
    }

    public UnitStatsModifier GetModifierByEquipmentMass(float equipmentMass)
    {
        return GetModifierByEquipmentMass(Extensions.GetWeightByMass(equipmentMass));
    }
}
