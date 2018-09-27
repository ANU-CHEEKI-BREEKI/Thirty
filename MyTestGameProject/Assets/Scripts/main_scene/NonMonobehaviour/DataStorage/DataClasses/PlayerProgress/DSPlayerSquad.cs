using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class DSPlayerSquad : IResetable, IMergeable
{
    [SerializeField] Inventory inventory;
    /// <summary>
    /// Использовать только для экипировки и расходников!!!! Скиллы хранятся отдельно!!!!
    /// </summary>
    public Inventory Inventory { get { return inventory; } }
    [SerializeField] int count;
    /// <summary>
    /// Количество живых юнитов
    /// </summary>
    public int Count { get { return count; } }
    float[] health;
    /// <summary>
    /// Здоровье каждого живого юнита.
    /// КОПИЯ массива
    /// </summary>
    public float[] Health { get { return new List<float>(health).ToArray(); } }

    public DSPlayerSquad()
    {
        Reset();
    }

    public DSPlayerSquad(Squad squad)
    {
        inventory = JsonUtility.FromJson<Inventory>(JsonUtility.ToJson(squad.Inventory));
        count = squad.UnitCount;
        health = new float[count];
        var poss = squad.UnitPositions;
        for (int i = 0; i < count; i++)
            health[i] = poss[i].Unit.Health;
    }

    public void Reset()
    {
        inventory = new Inventory();
        if (Squad.playerSquadInstance != null)
            count = Squad.playerSquadInstance.FULL_SQUAD_UNIT_COUNT;
        else
            count = 30;
        health = new float[count];
    }

    public void Merge(object data)
    {
        
    }
}
