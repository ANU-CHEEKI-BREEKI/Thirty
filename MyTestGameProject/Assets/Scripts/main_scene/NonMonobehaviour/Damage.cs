using System;
using UnityEngine;

[Serializable]
public struct Damage
{
    [SerializeField] [Range(0, 500)] float baseDamage;
    public float BaseDamage { get { return baseDamage; } }

    [SerializeField] [Range(0, 100)] float armourDamage;
    public float ArmourDamage { get { return armourDamage; } }

    public Damage(float baseDmg, float armourDmg)
    {
        baseDamage = baseDmg;
        armourDamage = armourDmg;
    }
}