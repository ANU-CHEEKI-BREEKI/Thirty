using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using static Description;

[Serializable]
public struct EquipmentStats : IDescriptionable
{
    public enum TypeOfEquipment { HEAD, BODY, SHIELD, WEAPON }
    public enum Durability { DAMAGED = 30, WORN = 70, NEW = 100 }

    const float percentKoef = 0.01f;

    [Header("Базовые свойства")]
    [SerializeField] TypeOfEquipment type;
    public TypeOfEquipment Type { get { return type; } }

    [SerializeField] int id;
    public int Id { get { return id; } }

    [SerializeField] bool empty;
    public bool Empty { get { return empty; } }

    [SerializeField] bool canReformToFhalanx;
    public bool CanReformToPhalanx { get { return canReformToFhalanx; } }

    [SerializeField] bool canReformToPhalanxInFight;
    public bool CanReformToPhalanxInFight { get { return canReformToPhalanxInFight; } }

    [SerializeField] bool canUseWithShield;
    public bool CanUseWithShield { get { return canUseWithShield; } }

    [Tooltip("Состояние предмета определяет его стоимость и свойства.\r\nПри обращении к свойствам экипировки, они будут возвращать значения с учетом состояния.")]
    [SerializeField] Durability durability;
    public Durability ItemDurability { get { return durability; } set { durability = value; } }

    [Header("Свойства, на которые НЕ влияет состояние экипировки", order = 1)] [Space]
    #region Dyrability Independence
    [Header("----прямые свойства----", order = 2)]
    [SerializeField] int mass;
    public int Mass { get { return mass; } }

    [SerializeField] [Range(0, 10)] float attackDistance;
    public float AttackDistance { get { return attackDistance; } }

    [SerializeField] [Range(-1, 1)] float chargeImpact;
    public float ChargeImpact { get { return chargeImpact; } }

    [SerializeField] [Range(-1, 1)] float chargeDeflect;
    public float ChargeDeflect { get { return chargeDeflect; } }

    [Header("----свойства-модификаторы----", order = 3)]
    [SerializeField] [Range(-1, 1)] float addAttack;
    public float AddAttack { get { return addAttack; } }

    [SerializeField] [Range(-1, 1)] float addDefence;
    public float AddDefence { get { return addDefence; } }

    [SerializeField] [Range(-1, 1)] float addDefenceHalfSector;
    public float AddDefenceHalfSector { get { return addDefenceHalfSector; } }

    [SerializeField] [Range(-1, 1)] float addSpeed;
    public float AddSpeed { get { return addSpeed; } }

    [SerializeField] [Range(-1, 1)] float addAcceleretion;
    public float AddAcceleretion { get { return addAcceleretion; } }

    [SerializeField] [Range(-1, 1)] float addRotationSpeed;
    public float AddRotationSpeed { get { return addRotationSpeed; } }

    #endregion

    [Header("Свойства на которые влияет состояние экипировки", order = 1)] [Space]
    #region Dyrability Dependence
    [Header("----прямые свойства----", order = 2)]
    [SerializeField] int cost;
    public int Cost { get { var val = (int)Math.Round(cost * percentKoef * (int)ItemDurability, 2); if (val <= 0) val = 1; return val; } }

    [SerializeField] [Range(0, 500)] float armour;
    public float Armour { get { return (float)Math.Round(armour * percentKoef * (int)ItemDurability, 2); } }

    [SerializeField] Damage damage;
    public Damage Damag { get { return new Damage((float)Math.Round(damage.BaseDamage * percentKoef * (int)ItemDurability, 2), (float)Math.Round(damage.ArmourDamage * percentKoef * (int)ItemDurability, 2)); } }

    [SerializeField] [Range(0, 1)] float missileBlock;
    public float MissileBlock { get { return (float)Math.Round(missileBlock * percentKoef * (int)ItemDurability, 2); } }

    [Header("----свойства-модификаторы----", order = 3)]
    [SerializeField] [Range(-1, 1)] float addChargeDamage;
    public float AddChargeDamage { get { return (float)Math.Round(addChargeDamage * percentKoef * (int)ItemDurability, 2); } }
    #endregion

    public Description GetDescription()
    {
        Description res = new Description();

        if (!Empty)
        {

            string desc;
            DescriptionItem d;

            List<DescriptionItem> constraints = new List<DescriptionItem>();

            if (!canReformToFhalanx)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.attention,
                    Description = LocalizedStrings.weaponConstraint_cantReformPhalanx,
                    ItPositiveDesc = canReformToFhalanx
                };
                constraints.Add(d);
            }
            else
            {
                if (!canReformToPhalanxInFight)
                {
                    d = new DescriptionItem()
                    {
                        Name = LocalizedStrings.attention,
                        Description = LocalizedStrings.weaponConstraint_cantReformPhalanxInFight,
                        ItPositiveDesc = canReformToPhalanxInFight
                    };
                    constraints.Add(d);
                }
            }

            if (!canUseWithShield)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.attention,
                    Description = LocalizedStrings.weaponConstraint_cantUseShield,
                    ItPositiveDesc = canUseWithShield
                };
                constraints.Add(d);
            }



            List<DescriptionItem> stats = new List<DescriptionItem>();

            d = new DescriptionItem()
            {
                Name = LocalizedStrings.mass,
                Description = Mass.ToString(StringFormats.floatNumber),
                ItPositiveDesc = true
            };
            stats.Add(d);

            if (AttackDistance > 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.attackDistance,
                    Description = AttackDistance.ToString(StringFormats.floatNumber),
                    ItPositiveDesc = true
                };
                stats.Add(d);
            }

            if (AddAttack != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.attack,
                    Description = AddAttack.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = AddAttack > 0
                };
                stats.Add(d);
            }

            if (AddDefence != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.defence,
                    Description = AddDefence.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = AddDefence > 0
                };
                stats.Add(d);
            }

            if (AddDefenceHalfSector != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.defenceHalfSector,
                    Description = AddDefenceHalfSector.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = AddDefenceHalfSector > 0
                };
                stats.Add(d);
            }

            if (AddSpeed != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.speed,
                    Description = AddSpeed.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = AddSpeed > 0
                };
                stats.Add(d);
            }

            if (AddAcceleretion != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.acceleration,
                    Description = AddAcceleretion.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = AddAcceleretion > 0
                };
                stats.Add(d);
            }

            if (AddRotationSpeed != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.rotationSpeed,
                    Description = AddRotationSpeed.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = AddRotationSpeed > 0
                };
                stats.Add(d);
            }

            if (ChargeImpact != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.chargeImpact,
                    Description = ChargeImpact.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = ChargeImpact > 0
                };
                stats.Add(d);
            }

            if (ChargeDeflect != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.chargeDeflect,
                    Description = ChargeDeflect.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = ChargeDeflect > 0
                };
                stats.Add(d);
            }

            if (Armour > 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.armour,
                    Description = Armour.ToString(StringFormats.floatNumber),
                    ItPositiveDesc = true
                };
                stats.Add(d);
            }

            if (Damag.BaseDamage > 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.baseDamage,
                    Description = Damag.BaseDamage.ToString(StringFormats.floatNumber),
                    ItPositiveDesc = true
                };
                stats.Add(d);
            }

            if (Damag.ArmourDamage > 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.armourDamage,
                    Description = Damag.ArmourDamage.ToString(StringFormats.floatNumber),
                    ItPositiveDesc = true
                };
                stats.Add(d);
            }

            if (MissileBlock != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.missileBlock,
                    Description = MissileBlock.ToString(StringFormats.floatNumberPercent),
                    ItPositiveDesc = MissileBlock > 0
                };
                stats.Add(d);
            }

            if (AddChargeDamage != 0)
            {
                d = new DescriptionItem()
                {
                    Name = LocalizedStrings.chargeDamage,
                    Description = AddChargeDamage.ToString(StringFormats.floatSignNumberPercent) + LocalizedStrings.baseValue,
                    ItPositiveDesc = AddChargeDamage > 0
                };
                stats.Add(d);
            }

            EquipmentStats st = this;
            res.Condition = new Description.ConditionsInfo() {
                Name = ItemDurability.GetNameLocalise(),
                Value = (Description.ConditionsInfo.Conditions)Enum.GetNames(typeof(Durability)).ToList().FindIndex((s)=> { return Enum.GetName(typeof(Durability), st.ItemDurability) == s; })
            };
            res.Cost = new Description.CostInfo() { CostPerOne = Cost };
            res.Constraints = constraints.ToArray();
            res.Stats = stats.ToArray();

        }

        return res;
    }

    public bool Equals(EquipmentStats eqS)
    {
        return type == eqS.type && id == eqS.id && durability == eqS.durability;
    }
}
