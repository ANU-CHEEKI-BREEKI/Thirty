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
    public event Action<Equipment> OnEquipmentChanged;

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
    //public Equipment HelmetNone { get { return helmetNone; } }

    [SerializeField] Equipment bodyNone;
    //public Equipment BodyNone { get { return bodyNone; } }

    [SerializeField] Equipment shieldNone;
    //public Equipment ShieldNone { get { return shieldNone; } }

    [SerializeField] EquipmentStack[] inventory = new EquipmentStack[3];
    public int Length { get { return inventory.Length; } }

    public Equipment Helmet
    {
        get { return helmet; }
        set
        {
            if (value == null)
                helmet = helmetNone;
            else
                helmet = value;
            EquipmentChanged(helmet);
        }
    }
    public Equipment Body
    {
        get { return body; }
        set
        {
            if (value == null)
                body = bodyNone;
            else
                body = value;
            EquipmentChanged(body);
        }
    }
    public Equipment Shield
    {
        get { return shield; }
        set
        {
            if (value == null)
                shield = shieldNone;
            else
                shield = value;
            EquipmentChanged(shield);
        }
    }
    public Equipment Weapon
    {
        get { return weapon; }
        set
        {
            if (value == null)
                throw new Exception("Нельзя убрать оружие!!!");

            weapon = value;

            EquipmentChanged(weapon);
        }
    }
    public SkillStack FirstSkill { get { return firstSkill; } set { firstSkill = value; } }
    public SkillStack SecondSkill { get { return secondSkill; } set { secondSkill = value; } }
    public ConsumableStack FirstConsumable { get { return firstConsumable; } set { firstConsumable = value; } }
    public ConsumableStack SecondConsumable { get { return secondConsumable; } set { secondConsumable = value; } }

    void EquipmentChanged(Equipment eq)
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
