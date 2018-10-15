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

    [Header("StartEquipment")]
    [SerializeField] Equipment helmet;
    [SerializeField] Equipment body;
    [SerializeField] Equipment shield;
    [SerializeField] Equipment weapon;
    [Space]
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
            if (helmetStack == null)
                helmetStack = Helmet;
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
            if (bodyStack == null)
                bodyStack = Body;
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
            if (shieldStack == null)
                shieldStack = Shield;
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
            if (weaponStack == null)
                weaponStack = Weapon;
            if (value == null)
                throw new Exception("Нельзя убрать оружие!!!");

            weaponStack.EquipmentMainProperties = value.EquipmentMainProperties;
            weaponStack.EquipmentStats = value.EquipmentStats;

            EquipmentChanged(weaponStack);
        }
    }
    public SkillStack FirstSkill
    {
        get
        {
            return firstSkill;
        }
        set
        {
            if (value == null)
                firstSkill.Skill = null;
            else
            {
                firstSkill.Skill = value.Skill;
                firstSkill.SkillStats = value.SkillStats;
            }
        }
    }
    public SkillStack SecondSkill
    {
        get
        {
            return secondSkill;
        }
        set
        {
            if (value == null)
                secondSkill.Skill = null;
            else
            {
                secondSkill.Skill = value.Skill;
                secondSkill.SkillStats = value.SkillStats;
            }
        }
    }
    public ConsumableStack FirstConsumable
    {
        get
        {
            return firstConsumable;
        }
        set
        {
            if (value == null)
                firstConsumable.Consumable = null;
            else
            {
                firstConsumable.Consumable = value.Consumable;
                firstConsumable.ConsumableStats = value.ConsumableStats;
                firstConsumable.Count = value.Count;
            }
        }
    }
    public ConsumableStack SecondConsumable
    {
        get
        {
            return secondConsumable;
        }
        set
        {
            if (value == null)
                secondConsumable.Consumable = null;
            else
            {
                secondConsumable.Consumable = value.Consumable;
                secondConsumable.ConsumableStats = value.ConsumableStats;
                secondConsumable.Count = value.Count;
            }
        }
    }

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
            if (value == null)
                inventory[index] = null;
            else
                inventory[index] = new EquipmentStack(value);
        }
    }
}