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

    int countOfPilumsToVolley;

    Vector2 castPosition;

    [Serializable]
    public struct PilumsVolleyStats : ISkillCooldownable, IStackCountConstraintable, ISkillCostable, ISkillDirectionable, IDescriptionable, ISkillDelayable
    {
        public Damage damage;
        public float speed;
        [Space]
        [SerializeField] float distance;        
        [SerializeField] float cooldown;
        [SerializeField] int maxCount;
        [SerializeField] int costPerOne;
        [SerializeField] float delay;
        

        public float Cooldown { get { return cooldown; } }
        
        public int Cost { get { return costPerOne; } }

        public int MaxCount { get { return maxCount; } }

        public float Distance { get { return distance; } }

        public float Delay { get { return delay; } }

        public Description GetDescription()
        {
            Description d = new Description();

            DescriptionItem[] stats = new DescriptionItem[]
            {
                new DescriptionItem(){ Name = LocalizedStrings.baseDamage, Description = damage.BaseDamage.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = LocalizedStrings.armourDamage, Description = damage.ArmourDamage.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = LocalizedStrings.cooldown, Description = cooldown.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = LocalizedStrings.attackDistance, Description = distance.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = LocalizedStrings.flyingSpeed, Description = speed.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
            };

            d.Stats = stats;
            d.Cost = new Description.CostInfo() { CostPerOne = costPerOne };

            return d;
        }        
    }
    
    public override object DefaultStats { get { return stats; } }

    public override bool Execute(object skillStats)
    {
        base.Execute(skillStats);
        
        bool res = true;
        PilumsVolleyStats stats = new PilumsVolleyStats();

        if (skillStats != null)
        {
            stats = (PilumsVolleyStats)skillStats;

            if (countOfPilumsToVolley <= 0)
                res = false;
        }
        else
        {
            res = false;
        }

        if (res)
        {
            var init = new InitStruct()
            {
                owner = this.owner,
                castPosition = this.castPosition,
                countOfPilumsToVolley = this.countOfPilumsToVolley
            };
            GameManager.Instance.StartCoroutine(DelayForExecute(stats, stats.Delay, init));
        }

        return true;
    }

    IEnumerator DelayForExecute(PilumsVolleyStats stats, float delay, InitStruct init)
    {
        yield return new WaitForSeconds(1);

        var volley = Instantiate(origin, init.owner.CenterSquad, init.owner.PositionsTransform.rotation);
        volley.Init(init.castPosition, stats.damage, stats.Distance, stats.speed, init.countOfPilumsToVolley, init.owner, CallBack);
        volley.StartVolley();
    }

    /// <summary>
    /// opisanie
    /// </summary>
    /// <param name="args">
    /// <para>Squad - отряд который вызвал скилл</para>
    /// <para>Vector2 - место куда будут лететь пилумы</para>
    /// <para>int - кол во доступных пилумов</para>
    /// </param>
    public override void Init(params object[] args)
    {
        owner = args[0] as Squad;
        castPosition = (Vector2)args[1];
        countOfPilumsToVolley = (int)args[2];
    }

    struct InitStruct
    {
        public Squad owner;
        public Vector2 castPosition;
        public int countOfPilumsToVolley;
    }
}
