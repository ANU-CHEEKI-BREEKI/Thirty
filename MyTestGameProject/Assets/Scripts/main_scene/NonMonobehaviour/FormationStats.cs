using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Description;

public abstract class FormationStats
{
    public enum Formations { RANKS, PHALANX, RISEDSHIELDS }

    public readonly Formations FORMATION;
    
    public readonly float SQUAD_ADDITIONAL_SPEED;
    public readonly float SQUAD_ADDITIONAL_ROTATION_SPEED;

    public readonly float UNIT_ADDITIONAL_SPEED;
    public readonly float UNIT_ADDITIONAL_ROTATION_SPEED;

    public readonly float UNIT_ADDITIONAL_DAMAGE;

    public readonly float UNIT_ADDITIONAL_ATTACK;
    public readonly float UNIT_ADDITIONAL_DEFENCE;

    public readonly float UNIT_ADDITIONAL_DEFENCE_SECTOR;   

    public FormationStats(
        float SQUAD_ADDITIONAL_SPEED = 0,
        float SQUAD_ADDITIONAL_ROTATION_SPEED = 0,
        float UNIT_ADDITIONAL_SPEED = 0,
        float UNIT_ADDITIONAL_ROTATION_SPEED = 0,
        float UNIT_ADDITIONAL_DAMAGE = 0,
        float UNIT_ADDITIONAL_ATTACK = 0,
        float UNIT_ADDITIONAL_DEFENCE = 0,
        float UNIT_ADDITIONAL_DEFENCE_SECTOR = 0,
        Formations FORMATION = Formations.RANKS
    )
    {
        this.SQUAD_ADDITIONAL_SPEED = SQUAD_ADDITIONAL_SPEED;
        this.SQUAD_ADDITIONAL_ROTATION_SPEED = SQUAD_ADDITIONAL_ROTATION_SPEED;

        this.UNIT_ADDITIONAL_SPEED = UNIT_ADDITIONAL_SPEED;
        this.UNIT_ADDITIONAL_ROTATION_SPEED = UNIT_ADDITIONAL_ROTATION_SPEED;

        this.UNIT_ADDITIONAL_DAMAGE = UNIT_ADDITIONAL_DAMAGE;

        this.UNIT_ADDITIONAL_ATTACK = UNIT_ADDITIONAL_ATTACK;
        this.UNIT_ADDITIONAL_DEFENCE = UNIT_ADDITIONAL_DEFENCE;

        this.UNIT_ADDITIONAL_DEFENCE_SECTOR = UNIT_ADDITIONAL_DEFENCE_SECTOR;

        this.FORMATION = FORMATION;
    }

    public DescriptionItem[] GetModifiers()
    {
        List<DescriptionItem> res = new List<DescriptionItem>();

        string format = "###";
        bool positive = false;
        string plus = "+";
        string sign = string.Empty;

        if(UNIT_ADDITIONAL_SPEED != 0)
        {
            if (UNIT_ADDITIONAL_SPEED > 0)
            {
                positive = true;
                sign = plus;
            }
            else
            {
                positive = false;
                sign = string.Empty;
            }

            res.Add(new DescriptionItem()
            {
                Name = LocalizedStrings.speed,
                Description = string.Format(
                    "{0}{1}% {2}",
                    sign,
                    (UNIT_ADDITIONAL_SPEED*100).ToString(format),
                    LocalizedStrings.currentValue
                ),
                ItPositiveDesc = positive
            });
        }

        if (UNIT_ADDITIONAL_ROTATION_SPEED != 0)
        {
            if (UNIT_ADDITIONAL_ROTATION_SPEED > 0)
            {
                positive = true;
                sign = plus;
            }
            else
            {
                positive = false;
                sign = string.Empty;
            }

            res.Add(new DescriptionItem()
            {
                Name = LocalizedStrings.rotationSpeed,
                Description = string.Format("{0}{1}% {2}",
                    sign,
                    (UNIT_ADDITIONAL_ROTATION_SPEED*100).ToString(format),
                    LocalizedStrings.currentValue
                ),
                ItPositiveDesc = positive
            });
        }

        if (UNIT_ADDITIONAL_DAMAGE != 0)
        {
            if (UNIT_ADDITIONAL_DAMAGE > 0)
            {
                positive = true;
                sign = plus;
            }
            else
            {
                positive = false;
                sign = string.Empty;
            }

            res.Add(new DescriptionItem()
            {
                Name = LocalizedStrings.baseDamage,
                Description = string.Format("{0}{1}% {2}",
                    sign,
                    (UNIT_ADDITIONAL_DAMAGE*100).ToString(format),
                    LocalizedStrings.currentValue
                ),
                ItPositiveDesc = positive
            });
        }

        if (UNIT_ADDITIONAL_ATTACK != 0)
        {
            if (UNIT_ADDITIONAL_ATTACK > 0)
            {
                positive = true;
                sign = plus;
            }
            else
            {
                positive = false;
                sign = string.Empty;
            }

            res.Add(new DescriptionItem()
            {
                Name = LocalizedStrings.attack,
                Description = string.Format("{0}{1}% {2}",
                    sign,
                    (UNIT_ADDITIONAL_ATTACK * 100).ToString(format),
                    LocalizedStrings.currentValue
                ),
                ItPositiveDesc = positive
            });
        }

        if (UNIT_ADDITIONAL_DEFENCE != 0)
        {
            if (UNIT_ADDITIONAL_DEFENCE > 0)
            {
                positive = true;
                sign = plus;
            }
            else
            {
                positive = false;
                sign = string.Empty;
            }

            res.Add(new DescriptionItem()
            {
                Name = LocalizedStrings.defence,
                Description = string.Format("{0}{1}% {2}",
                    sign,
                    (UNIT_ADDITIONAL_DEFENCE * 100).ToString(format),
                    LocalizedStrings.currentValue
                ),
                ItPositiveDesc = positive
            });
        }

        return res.ToArray();
    }

    public class Ranks : FormationStats
    {
        public Ranks() { }
    }

    public class Phalanx : FormationStats
    {
        public Phalanx() : base(
            -0.8f, -0.9f,
            -0.8f, -0.955f,
             0.9f,
            -0.1f,    0.1f,
            -0.5f,
            Formations.PHALANX
        ) { }
    }

    public class RisedShields : FormationStats
    {
        public RisedShields() : base(
             0, 0,
             0, 0,
             0,
            -0.5f, -0.8f, 
             0,
             Formations.RISEDSHIELDS
        ) { }
    }
}
