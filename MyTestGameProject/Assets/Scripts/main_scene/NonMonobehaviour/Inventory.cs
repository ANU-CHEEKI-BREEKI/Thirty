using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    public Squad Squad { get; set; }
    /// <summary>
    /// arg is a new  equipment
    /// </summary>
    public event Action<EquipmentStack> OnEquipmentChanged;

    EquipmentStack helmetStack;
    EquipmentStack bodyStack;
    EquipmentStack shieldStack;
    EquipmentStack weaponStack;

    [SerializeField] Equipment helmet;
    [SerializeField] Equipment body;
    [SerializeField] Equipment shield;
    [SerializeField] Equipment weapon;

    [SerializeField] SkillStack firstSkill;
    [SerializeField] SkillStack secondSkill;
    [SerializeField] ConsumableStack firstConsumable;
    [SerializeField] ConsumableStack secondConsumable;

    [Space]
    [SerializeField] Equipment helmetNone;
    [SerializeField] Equipment bodyNone;
    [SerializeField] Equipment shieldNone;

    [SerializeField] EquipmentStack[] inventory = new EquipmentStack[3];
    public int Length { get { return inventory.Length; } }

    public EquipmentStack Helmet
    {
        get
        {
            if(helmetStack == null)
                helmetStack = new EquipmentStack()
                {
                    EquipmentMainProperties = helmet.MainPropertie,
                    EquipmentStats = helmet.Stats
                };
            return helmetStack;
        }
        set
        {
            if (value == null)
            {
                helmetStack.EquipmentMainProperties = helmetNone.MainPropertie;
                helmetStack.EquipmentStats = helmetNone.Stats;
            }
            else
            {
                helmetStack.EquipmentMainProperties = value.EquipmentMainProperties;
                helmetStack.EquipmentStats = value.EquipmentStats;
            }
            EquipmentChanged(helmetStack);
        }
    }
    public EquipmentStack Body
    {
        get
        {
            if (bodyStack == null)
                bodyStack = new EquipmentStack()
                {
                    EquipmentMainProperties = body.MainPropertie,
                    EquipmentStats = body.Stats
                };
            return bodyStack;
        }
        set
        {
            if (value == null)
            {
                bodyStack.EquipmentMainProperties = bodyNone.MainPropertie;
                bodyStack.EquipmentStats = bodyNone.Stats;
            }
            else
            {
                bodyStack.EquipmentMainProperties = value.EquipmentMainProperties;
                bodyStack.EquipmentStats = value.EquipmentStats;
            }
            EquipmentChanged(bodyStack);
        }
    }
    public EquipmentStack Shield
    {
        get {
            if (shieldStack == null)
                shieldStack = new EquipmentStack()
                {
                    EquipmentMainProperties = shield.MainPropertie,
                    EquipmentStats = shield.Stats
                };
            return shieldStack;
        }
        set
        {
            if (value == null)
            {
                shieldStack.EquipmentMainProperties = shieldNone.MainPropertie;
                shieldStack.EquipmentStats = shieldNone.Stats;
            }
            else
            {
                shieldStack.EquipmentMainProperties = value.EquipmentMainProperties;
                shieldStack.EquipmentStats = value.EquipmentStats;
            }
            EquipmentChanged(shieldStack);
        }
    }
    public EquipmentStack Weapon
    {
        get {
            if (weaponStack == null)
                weaponStack = new EquipmentStack()
                {
                    EquipmentMainProperties = weapon.MainPropertie,
                    EquipmentStats = weapon.Stats
                };
            return weaponStack;
        }
        set
        {
            if (value == null)
                throw new Exception("Нельзя убрать оружие!!!");

            weaponStack.EquipmentMainProperties = value.EquipmentMainProperties;
            weaponStack.EquipmentStats = value.EquipmentStats;

            EquipmentChanged(weaponStack);
        }
    }
    public SkillStack FirstSkill { get { return firstSkill; } set { firstSkill = value; } }
    public SkillStack SecondSkill { get { return secondSkill; } set { secondSkill = value; } }
    public ConsumableStack FirstConsumable { get { return firstConsumable; } set { firstConsumable = value; } }
    public ConsumableStack SecondConsumable { get { return secondConsumable; } set { secondConsumable = value; } }

    void EquipmentChanged(EquipmentStack eq)
    {
        if (OnEquipmentChanged != null) OnEquipmentChanged(eq);
    }

    public EquipmentStack this[int index]
    {
        get
        {
            return inventory[index];
        }

        set
        {
            inventory[index] = value;
        }
    }
}
