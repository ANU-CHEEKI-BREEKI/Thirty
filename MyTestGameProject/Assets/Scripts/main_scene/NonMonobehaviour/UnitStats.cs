using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UnitStats
{
    public const float MAX_ATTACK = 0.9f;
    public const float MAX_DEFENCE = 0.9f;
    public const float MAX_MISSILE_BLOCK = 0.95f;

    public const float NORMAL_EQUIPED_MASS = 75;
    public const float NORMAL_NUDE_MASS = 60;

    [SerializeField] [Range(0, 200)] float health;
    public float Health { get { return health; } set { health = value; } }

    [SerializeField] [Range(0, 200)] float mass;
    public float Mass { get { return mass; } }

    [SerializeField] [Range(0, 500)] float armour;
    public float Armour { get { return armour; } }

    [SerializeField] Damage damage;
    public Damage Damage { get { return damage; } }

    [Space]
    [SerializeField] [Range(0, 1)] float attack;
    public float Attack { get { return attack; } }

    [SerializeField] [Range(0, 1)] float defence;
    public float Defence { get { return defence; } }

    [Space]
    [SerializeField] [Range(0, 180)] float defenceHalfSector;
    public float DefenceHalfSector { get { return defenceHalfSector; } }

    [SerializeField] [Range(0, 1)] float defenceGoingThrough;
    public float DefenceGoingThrought { get { return defenceGoingThrough; } }

    [SerializeField] [Range(0, 1)] float missileBlock;
    public float MissileBlock { get { return missileBlock; } }

    [Space]
    [SerializeField] [Range(0, 10)] float attackDistance;
    public float AttackDistance { get { return attackDistance; } }

    [Space]
    [SerializeField] [Range(1, 100)] float speed;
    public float Speed { get { return speed; } }

    [SerializeField] [Range(1, 100)] float acceleration;
    public float Acceleration { get { return acceleration; } }

    [SerializeField] [Range(1, 360)] float rotationSpeed;
    public float RotationSpeed { get { return rotationSpeed; } }

    [Space]
    [SerializeField] [Range(0, 2)] float chargeImpact;
    public float ChargeImpact { get { return chargeImpact; } }

    [SerializeField] [Range(0, 2)] float chargeDeflect;
    public float ChargeDeflect { get { return chargeDeflect; } }

    [SerializeField] [Range(0, 5)] float unitChargeAddDamage;

    public float ChargeAddDamage { get { return unitChargeAddDamage; } }

    public static UnitStats CalcStats(UnitStats baseStats, EquipmentStats[] equipmentStats, FormationStats formationStats)
    {
        int i;
        float k;

        int equipCnt = equipmentStats.Length;

        for (i = 0; i < equipCnt; i++)
            baseStats.mass += equipmentStats[i].Mass;

         k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddAttack;
        baseStats.attack *= k * (1 + formationStats.UNIT_ADDITIONAL_ATTACK);
        baseStats.attack = baseStats.attack > MAX_ATTACK ? MAX_ATTACK : baseStats.attack;

        k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddDefence;
        baseStats.defence *= k * (1 + formationStats.UNIT_ADDITIONAL_DEFENCE);
        baseStats.defence = baseStats.defence > MAX_DEFENCE ? MAX_DEFENCE : baseStats.defence;
                
        for (i = 0; i < equipCnt; i++)
            if(equipmentStats[i].Type != EquipmentStats.TypeOfEquipment.SHIELD)
            baseStats.armour += equipmentStats[i].Armour;

        float t = 0;
        float st = 0;
        for (i = 0; i < equipCnt; i++)
            if (equipmentStats[i].Type != EquipmentStats.TypeOfEquipment.SHIELD)
                t += equipmentStats[i].MissileBlock;
            else
                st = equipmentStats[i].MissileBlock;
        if (formationStats.FORMATION != FormationStats.Formations.RISEDSHIELDS)
            st *= 0.5f;
        baseStats.missileBlock += t + st;

        float armDmg = 0, baseDmg = 0;
        for (i = 0; i < equipCnt; i++)
        {
            armDmg += equipmentStats[i].Damag.ArmourDamage;
            baseDmg += equipmentStats[i].Damag.BaseDamage;
        }
        baseStats.damage = new Damage(
            (baseStats.damage.BaseDamage + baseDmg) * (1 + formationStats.UNIT_ADDITIONAL_DAMAGE), 
            baseStats.damage.ArmourDamage + armDmg
        );


        for (i = 0; i < equipCnt; i++)
            if (equipmentStats[i].Type == EquipmentStats.TypeOfEquipment.WEAPON)
                if(!equipmentStats[i].CanReformToPhalanxInFight && formationStats.FORMATION != FormationStats.Formations.PHALANX)
                    baseStats.attackDistance += 1;
                else
                    baseStats.attackDistance += equipmentStats[i].AttackDistance;

        k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddDefenceHalfSector;
        baseStats.defenceHalfSector *= k * (1 + formationStats.UNIT_ADDITIONAL_DEFENCE_SECTOR);

        k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddSpeed;
        baseStats.speed *= k * (1 + formationStats.UNIT_ADDITIONAL_SPEED);

        k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddAcceleretion;
        baseStats.acceleration *= k;

        k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddRotationSpeed;
        baseStats.rotationSpeed *= k * (1 + formationStats.UNIT_ADDITIONAL_ROTATION_SPEED);

        k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddChargeImpact;
        baseStats.chargeImpact *= k;

        k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddChargeDeflect;
        baseStats.chargeDeflect *= k;

        k = 1;
        for (i = 0; i < equipCnt; i++)
            k += equipmentStats[i].AddChargeDamage;
        baseStats.unitChargeAddDamage *= k;


        return baseStats;
    }

    public static UnitStats RewriteStats(UnitStats baseStats, DSUnitStats newStats)
    {
        baseStats.health = newStats.Health.Value;
        baseStats.attack = newStats.Attack.Value;
        baseStats.defence = newStats.Defence.Value;
        baseStats.speed = newStats.Speed.Value;
        baseStats.acceleration = newStats.Aceleration.Value;
        baseStats.rotationSpeed = newStats.RotationSpeed.Value;

        return baseStats;
    }

    public static UnitStats ModifyStats(UnitStats baseStats, UnitStatsModifyer modifyer, UnitStatsModifyer.UseType usetype = UnitStatsModifyer.UseType.APPLY)
    {
        if (usetype == UnitStatsModifyer.UseType.APPLY)
        {
            if (modifyer.Mass.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.mass += modifyer.Mass.Value;
            else
                baseStats.mass *= 1 + modifyer.Mass.Value;

            if (modifyer.Attack.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.attack += modifyer.Attack.Value;
            else
                baseStats.attack *= 1 + modifyer.Attack.Value;

            if (modifyer.Defence.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.defence += modifyer.Defence.Value;
            else
                baseStats.defence *= 1 + modifyer.Defence.Value;

            if (modifyer.Armour.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.armour += modifyer.Armour.Value;
            else
                baseStats.armour *= 1 + modifyer.Armour.Value;

            if (modifyer.MissileBlock.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.missileBlock += modifyer.MissileBlock.Value;
            else
                baseStats.missileBlock *= 1 + modifyer.MissileBlock.Value;

            float armDmg = baseStats.damage.ArmourDamage;
            if (modifyer.ArmourDamage.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                armDmg += modifyer.ArmourDamage.Value;
            else
                armDmg *= 1 + modifyer.ArmourDamage.Value;

            float baseDmg = baseStats.damage.BaseDamage;
            if (modifyer.BaseDamage.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseDmg += modifyer.BaseDamage.Value;
            else
                baseDmg *= 1 + modifyer.BaseDamage.Value;

            baseStats.damage = new Damage(baseDmg, armDmg);

            if (modifyer.AttackDistance.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.attackDistance += modifyer.AttackDistance.Value;
            else
                baseStats.attackDistance *= 1 + modifyer.AttackDistance.Value;

            if (modifyer.DefenceHalfSector.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.defenceHalfSector += modifyer.DefenceHalfSector.Value;
            else
                baseStats.defenceHalfSector *= 1 + modifyer.DefenceHalfSector.Value;

            if (modifyer.Speed.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.speed += modifyer.Speed.Value;
            else
                baseStats.speed *= 1 + modifyer.Speed.Value;

            if (modifyer.Acceleration.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.acceleration += modifyer.Acceleration.Value;
            else
                baseStats.acceleration *= 1 + modifyer.Acceleration.Value;

            if (modifyer.RotationSpeed.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.rotationSpeed += modifyer.RotationSpeed.Value;
            else
                baseStats.rotationSpeed *= 1 + modifyer.RotationSpeed.Value;

            if (modifyer.ChargeImpact.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.chargeImpact += modifyer.ChargeImpact.Value;
            else
                baseStats.chargeImpact *= 1 + modifyer.ChargeImpact.Value;

            if (modifyer.ChargeDeflect.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.chargeDeflect += modifyer.ChargeDeflect.Value;
            else
                baseStats.chargeDeflect *= 1 + modifyer.ChargeDeflect.Value;

            if (modifyer.ChargeAddDamage.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.unitChargeAddDamage += modifyer.ChargeAddDamage.Value;
            else
                baseStats.unitChargeAddDamage *= 1 + modifyer.ChargeAddDamage.Value;
        }
        else
        {
            if (modifyer.Mass.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.mass -= modifyer.Mass.Value;
            else
                baseStats.mass /= 1 + modifyer.Mass.Value;

            if (modifyer.Attack.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.attack -= modifyer.Attack.Value;
            else
                baseStats.attack /= 1 + modifyer.Attack.Value;

            if (modifyer.Defence.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.defence -= modifyer.Defence.Value;
            else
                baseStats.defence /= 1 + modifyer.Defence.Value;

            if (modifyer.Armour.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.armour -= modifyer.Armour.Value;
            else
                baseStats.armour /= 1 + modifyer.Armour.Value;

            if (modifyer.MissileBlock.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.missileBlock -= modifyer.MissileBlock.Value;
            else
                baseStats.missileBlock /= 1 + modifyer.MissileBlock.Value;

            float armDmg = baseStats.damage.ArmourDamage;
            if (modifyer.ArmourDamage.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                armDmg -= modifyer.ArmourDamage.Value;
            else
                armDmg /= 1 + modifyer.ArmourDamage.Value;

            float baseDmg = baseStats.damage.BaseDamage;
            if (modifyer.BaseDamage.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseDmg -= modifyer.BaseDamage.Value;
            else
                baseDmg /= 1 + modifyer.BaseDamage.Value;

            baseStats.damage = new Damage(baseDmg, armDmg);

            if (modifyer.AttackDistance.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.attackDistance -= modifyer.AttackDistance.Value;
            else
                baseStats.attackDistance /= 1 + modifyer.AttackDistance.Value;

            if (modifyer.DefenceHalfSector.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.defenceHalfSector -= modifyer.DefenceHalfSector.Value;
            else
                baseStats.defenceHalfSector /= 1 + modifyer.DefenceHalfSector.Value;

            if (modifyer.Speed.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.speed -= modifyer.Speed.Value;
            else
                baseStats.speed /= 1 + modifyer.Speed.Value;

            if (modifyer.Acceleration.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.acceleration -= modifyer.Acceleration.Value;
            else
                baseStats.acceleration /= 1 + modifyer.Acceleration.Value;

            if (modifyer.RotationSpeed.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.rotationSpeed -= modifyer.RotationSpeed.Value;
            else
                baseStats.rotationSpeed /= 1 + modifyer.RotationSpeed.Value;

            if (modifyer.ChargeImpact.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.chargeImpact -= modifyer.ChargeImpact.Value;
            else
                baseStats.chargeImpact /= 1 + modifyer.ChargeImpact.Value;

            if (modifyer.ChargeDeflect.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.chargeDeflect -= modifyer.ChargeDeflect.Value;
            else
                baseStats.chargeDeflect /= 1 + modifyer.ChargeDeflect.Value;

            if (modifyer.ChargeAddDamage.VType == UnitStatsModifyer.Modifyer.ValueType.UNIT)
                baseStats.unitChargeAddDamage -= modifyer.ChargeAddDamage.Value;
            else
                baseStats.unitChargeAddDamage /= 1 + modifyer.ChargeAddDamage.Value;
        }

        return baseStats;
    }
}
