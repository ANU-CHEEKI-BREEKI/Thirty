using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class DSPlayerSquad : IResetable, IMergeable
{
    [SerializeField] bool isEmpty;
    public bool IsEmpty { get { return isEmpty; } }

    [SerializeField] EquipmentStack helmetStack;
    [SerializeField] EquipmentStack bodyStack;
    [SerializeField] EquipmentStack shieldStack;
    [SerializeField] EquipmentStack weaponStack;
    [SerializeField] ConsumableStack firstConsumable;
    [SerializeField] ConsumableStack secondConsumable;
    [SerializeField] EquipmentStack[] inventory;

    public EquipmentStack Helmet { get { return helmetStack; } }
    public EquipmentStack Body { get { return bodyStack; } }
    public EquipmentStack Shield { get { return shieldStack; } }
    public EquipmentStack Weapon { get { return weaponStack; } }
    public ConsumableStack FirstConsumable { get { return firstConsumable; } }
    public ConsumableStack SecondConsumable { get { return secondConsumable; } }
    public EquipmentStack[] Inventory { get { return inventory; } }

    [SerializeField] int count;
    /// <summary>
    /// Количество живых юнитов
    /// </summary>
    public int Count { get { return count; } }
    [SerializeField] float[] health;
    /// <summary>
    /// Здоровье каждого живого юнита.
    /// КОПИЯ массива
    /// </summary>
    public float[] Health { get { return new List<float>(health).ToArray(); } }

    public DSPlayerSquad()
    {
        Reset();
    }

    public void SetSquadValues(Squad squad)
    {
        isEmpty = false;

        var inv = squad.Inventory;
        helmetStack = new EquipmentStack(inv.Helmet);
        bodyStack = new EquipmentStack(inv.Body);
        shieldStack = new EquipmentStack(inv.Shield);
        weaponStack = new EquipmentStack(inv.Weapon);
        firstConsumable = new ConsumableStack(inv.FirstConsumable);
        secondConsumable = new ConsumableStack(inv.SecondConsumable);

        inventory = new EquipmentStack[inv.Length];
        for (int i = 0; i < inv.Length; i++)
            inventory[i] = inv[i];

        count = squad.UnitCount;
        health = new float[count];
        var poss = squad.UnitPositions;
        for (int i = 0; i < count; i++)
            health[i] = poss[i].Unit.Health;
    }

    public void Reset()
    {
        isEmpty = true;

        helmetStack = null;
        bodyStack = null;
        shieldStack = null;
        weaponStack = null;

        firstConsumable = null;
        secondConsumable = null;

        inventory = null;

        if (Squad.playerSquadInstance != null)
            count = Squad.playerSquadInstance.FULL_SQUAD_UNIT_COUNT;
        else
            count = 30;
        health = new float[count];
    }

    public void Merge(object data)
    {
        var d = data as DSPlayerSquad;
        inventory = d.inventory;
        count = d.count;
        health = d.health;

        helmetStack = new EquipmentStack(d.Helmet);
        bodyStack = new EquipmentStack(d.Body);
        shieldStack = new EquipmentStack(d.Shield);
        weaponStack = new EquipmentStack(d.Weapon);
        firstConsumable = new ConsumableStack(d.FirstConsumable);
        secondConsumable = new ConsumableStack(d.SecondConsumable);
    }
}
