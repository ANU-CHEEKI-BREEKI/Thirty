using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Description;

public class ConsumablePilumsVolley : Consumable
{
    [Header("ConsumablePilumsVolley")]
    [SerializeField] PilumsVolley origin;
    [Space]

    [SerializeField] PilumsVolleyStats stats;

    [HideInInspector] public Squad owner;
    int countOfPilumsToVolley;

    Vector2 castPosition;

    [Serializable]
    public struct PilumsVolleyStats : ISkillCooldownable, IStackCountConstraintable, ISkillCostable, ISkillDirectionable, IDescriptionable
    {
        public Damage damage;
        public float speed;
        [Space]
        [SerializeField] float distance;        
        [SerializeField] float cooldown;
        [SerializeField] int maxCount;
        [SerializeField] int costPerOne;
        

        public float Cooldown { get { return cooldown; } }
        
        public int Cost { get { return costPerOne; } }

        public int MaxCount { get { return maxCount; } }

        public float Distance { get { return distance; } }

        public Description GetDescription()
        {
            Description d = new Description();

            DescriptionItem[] stats = new DescriptionItem[]
            {
                new DescriptionItem(){ Name = Localization.baseDamage, Description = damage.BaseDamage.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = Localization.armourDamage, Description = damage.ArmourDamage.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = Localization.cooldown, Description = cooldown.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = Localization.attackDistance, Description = distance.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = Localization.flyingSpeed, Description = speed.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
            };

            d.Stats = stats;
            d.Cost = new Description.CostInfo() { CostPerOne = costPerOne };

            return d;
        }        
    }
    
    public override object DefaultStats { get { return stats; } }

    public override bool DoSkill(object skillStats)
    {
        bool res = true;

        if (skillStats != null)
        {
            PilumsVolleyStats stats = (PilumsVolleyStats)skillStats;

            if (countOfPilumsToVolley > 0)
            {
                var volley = Instantiate(origin, owner.CenterSquad, owner.PositionsTransform.rotation);
                volley.Init(castPosition, stats.damage, stats.Distance, stats.speed, countOfPilumsToVolley, owner, CallBack);
                volley.StartVolley();
            }
            else
            {
                res = false;
            }
        }
        else
        {
            res = false;
        }
        return true;
    }

    /// <summary>
    /// opisanie
    /// </summary>
    /// <param name="args">
    /// <para>Squad - отряд который вызвал скилл</para>
    /// <para>Vector2 - место куда будут лететь пилумы</para>
    /// <para>int - кол во доступных пилумов</para>
    /// </param>
    public override void InitSkill(params object[] args)
    {
        owner = args[0] as Squad;
        castPosition = (Vector2)args[1];
        countOfPilumsToVolley = (int)args[2];
    }
}
