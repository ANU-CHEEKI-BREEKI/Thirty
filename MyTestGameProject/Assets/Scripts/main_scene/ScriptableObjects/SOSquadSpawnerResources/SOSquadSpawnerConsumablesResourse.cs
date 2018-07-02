using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/Consumables", fileName = "SO_SSR_Consumables")]
public class SOSquadSpawnerConsumablesResourse : ScriptableObject
{
    [SerializeField] AnimationCurve consumableLevelDependency;
    [SerializeField] ConsumableContainer[] consumablesByLevel;
    public ConsumableStack ConsumableByLevel { get { return GetConsumable(consumablesByLevel); } }
    
    ConsumableStack GetConsumable(ConsumableContainer[] equipments)
    {
        throw new NotImplementedException();
    }

    [Serializable]
    public class ConsumableContainer
    {
        [SerializeField] Consumable[] randomConsumables;
    }
}
