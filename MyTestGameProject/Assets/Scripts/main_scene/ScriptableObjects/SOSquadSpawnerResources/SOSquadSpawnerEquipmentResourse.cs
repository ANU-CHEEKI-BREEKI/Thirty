using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/Equipment", fileName = "SO_SSR_Equipment")]
public class SOSquadSpawnerEquipmentResourse : ScriptableObject
{
    [SerializeField] AnimationCurve equipmentLevelDependency;
    [SerializeField] EquipmentContainer[] equipmentByLevel;
    public EquipmentStack EquipmentByLevel { get { return GetEquipment(equipmentByLevel); } }
    
    EquipmentStack GetEquipment(EquipmentContainer[] equipments)
    {
        EquipmentStack res = null;

        if (equipments.Length > 0)
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            float val = equipmentLevelDependency.Evaluate(t);
            int index = Mathf.RoundToInt(equipmentLevelDependency.Evaluate(t) * (equipments.Length - 1));
            int l2 = equipments[index].randomEquipment.Length;
            int l3 = equipments[index].durability.Length;
            if (l2 > 0 && l3 > 0)
            {
                var eq = equipments[index].randomEquipment[UnityEngine.Random.Range(0, l2)];
                var eqMS = eq.MainPropertie;
                var eqS = eq.Stats;
                eqS.ItemDurability = equipments[index].durability[UnityEngine.Random.Range(0, l3)];
                res = new EquipmentStack(eqMS, eqS);
            }
        }

        return res;
    }

    [Serializable]
    public class EquipmentContainer
    {
        public Equipment[] randomEquipment;
        public EquipmentStats.Durability[] durability;
    }
}
