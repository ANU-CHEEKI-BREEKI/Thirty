﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Description;

public class SkillArrowsValley : Skill
{
    [Serializable]
    public struct ArrowWalleyStats : ISkillRadiusable, ISkillLockable, ISkillCooldownable, IDescriptionable, ISkillDelayable
    {
        public Damage damage;
        [Range(5, 10)] public float radius;
        [Range(50, 500)] public int countOfArrows;
        [Range(0, 1080)] public float cooldown;
        public float delay;
        public bool unlocked;

        public float Radius { get { return radius; } }
        public bool Unlocked { get { return unlocked; } }
        public float Cooldown { get { return cooldown; } }

        public float Delay { get { return delay; } }

        public Description GetDescription()
        {
            Description d = new Description();

            DescriptionItem[] stats = new DescriptionItem[]
            {
                new DescriptionItem(){ Name = LocalizedStrings.baseDamage, Description = damage.BaseDamage.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = LocalizedStrings.armourDamage, Description = damage.ArmourDamage.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = LocalizedStrings.arrowWalley_arrowscount, Description = countOfArrows.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = LocalizedStrings.arrowWalley_radius, Description = radius.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
                new DescriptionItem(){ Name = LocalizedStrings.cooldown, Description = cooldown.ToString(StringFormats.floatNumber), ItPositiveDesc = true },
            };

            d.Stats = stats;

            return d;
        }
    }

    [Header("SkillArrowsValley")]
    [SerializeField] ArrowsValley origin;
    [Space]

    [SerializeField] ArrowWalleyStats stats;
    
    Vector2 castPosition;
    Quaternion castRotation;

    public override object DefaultStats { get { return stats; } }

    public override object CalcUpgradedStats(List<DSPlayerSkill.SkillUpgrade> upgrades)
    {
        ArrowWalleyStats newStats = (ArrowWalleyStats)base.CalcUpgradedStats(upgrades);

        foreach (var upgrade in upgrades)
        {
            if (upgrade.isUpgradeToUnlock && upgrade.level > 0)
                newStats.unlocked = true;
        }

        return newStats;
    }
    
    public override bool Execute(object skillStats)
    {
        bool res = base.Execute(skillStats);

        if (res)
        {
            ArrowWalleyStats stats;
            if (skillStats != null && skillStats is ArrowWalleyStats)
                stats = (ArrowWalleyStats)skillStats;
            else
                stats = this.stats;

            ArrowsValley valley = Instantiate(origin, Vector2.zero, castRotation);
            valley.Init(castPosition, stats.damage, stats.radius, stats.countOfArrows, owner, stats.delay);
            valley.StartValley();
        }
        return res;
    }

    /// <summary>
    /// opisanie
    /// </summary>
    /// <param name="args">
    /// <para>Squad - отряд который вызвал скилл</para>
    /// <para>Vector2 - место куда будут лететь стрелы</para>
    /// <para>Quaternion - сторона откуда будут лететь стрелы</para>
    /// </param>
    public override void Init(params object[] args)
    {
        if (args.Length > 0)
            owner = (Squad)args[0];
        if (args.Length > 1)
            castPosition = (Vector2)args[1];
        if (args.Length > 2)
            castRotation = (Quaternion)args[2];
    }
}
