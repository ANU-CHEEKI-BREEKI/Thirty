using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/Consumables", fileName = "SO_SSR_Consumables")]
public class SOSquadSpawnerConsumablesResourse : ScriptableObject
{
    [SerializeField] AnimationCurve consumableChanseLevelDependency;
    public bool AllowOwnConsumable
    {
        get
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            float val = consumableChanseLevelDependency.Evaluate(t);
            return val > UnityEngine.Random.value;
        }
    }
    [Space]
    [SerializeField] AnimationCurve consumableLevelDependency;
    [SerializeField] ConsumableContainer[] consumablesByLevel;
    public ConsumableStack ConsumableByLevel { get { return GetConsumable(consumablesByLevel); } }
    
    ConsumableStack GetConsumable(ConsumableContainer[] equipments)
    {
        ConsumableStack res = null;

        if (equipments.Length > 0)
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            float val = consumableLevelDependency.Evaluate(t);
            int index = Mathf.RoundToInt(consumableLevelDependency.Evaluate(t) * (equipments.Length - 1));
            int l2 = equipments[index].randomConsumables.Length;
            var consum = equipments[index].randomConsumables[UnityEngine.Random.Range(0, l2)];
            res = new ConsumableStack(consum, consum.DefaultStats);
        }

        return res;
    }

    [Serializable]
    public class ConsumableContainer
    {
        public Consumable[] randomConsumables;
    }
}
