using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/Equipment", fileName = "SO_SSR_Equipment")]
public class SOSquadSpawnerEquipmentResourse : ScriptableObject
{
    [SerializeField] AnimationCurve equipmentLevelDependency;
    [SerializeField] EquipmentContainer[] headByLevel;
    public EquipmentStack HeadByLevel { get { return GetEquipment(headByLevel); } }

    [SerializeField] EquipmentContainer[] bodyByLevel;
    public EquipmentStack BodyByLevel { get { return GetEquipment(bodyByLevel); } }

    [SerializeField] EquipmentContainer[] weaponByLevel;
    public EquipmentStack WeaponByLevel { get { return GetEquipment(weaponByLevel); } }

    [SerializeField] EquipmentContainer[] shieldByLevel;
    public EquipmentStack ShieldByLevel { get { return GetEquipment(shieldByLevel); } }
    
    EquipmentStack GetEquipment(EquipmentContainer[] equipments)
    {
        throw new NotImplementedException();
    }

    [Serializable]
    public class EquipmentContainer
    {
        [SerializeField] Equipment[] randomEquipment;
        [SerializeField] EquipmentStats.Durability durability;
    }
}
