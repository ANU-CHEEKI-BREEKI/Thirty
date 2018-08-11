using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Description;

public class SkillCharge : Skill
{
    [Serializable]
    public struct ChargeStats: ISkillLockable, ISkillCooldownable, IDescriptionable, ISkillDurationable
    {
        public UnitStatsModifier modifyer;
        [Range(1, 10)] public float duration;
        [Range(0, 1080)] public float cooldown;
        public bool unlocked;

        public bool Unlocked { get { return unlocked; } }
        public float Cooldown { get { return cooldown; } }
        public float Duration { get { return duration; } }

        public Description GetDescription()
        {
            var desc = modifyer.GetDescription();
            List<DescriptionItem> l = new List<DescriptionItem>(desc.Stats);
            l.Add(new DescriptionItem() { Name = LocalizedStrings.duration, Description = duration.ToString(StringFormats.floatNumber), ItPositiveDesc = true });
            l.Add(new DescriptionItem() { Name = LocalizedStrings.cooldown, Description = cooldown.ToString(StringFormats.floatNumber), ItPositiveDesc = true });

            return new Description() { Stats = l.ToArray() };
        }
    }

    [Header("SkillCharge")]
    [SerializeField] ChargeStats defaultStats;
    [Space]
    [SerializeField] List<FormationStats.Formations> canExecute;
    
    /// <summary>
    /// Учавствуе в CalcUpgradedStats. Вызывается родительским классом.
    /// </summary>
    public override object DefaultStats { get { return defaultStats; } }

    public override bool Execute(object skillStats)
    {
        base.Execute(skillStats);

        bool res = true;

        ChargeStats stats;
        if (skillStats != null && skillStats is ChargeStats)
            stats = (ChargeStats)skillStats;
        else
            stats = this.defaultStats;

        if (res && canExecute.Contains(owner.CurrentFormation))
            owner.Charge(stats.modifyer, stats.duration);
        else
            res = false;

        return res;
    }

    /// <summary>
    /// opisanie
    /// </summary>
    /// <param name="args">
    /// <para>Squad - отряд который вызвал скилл</para>
    /// </param>
    public override void Init(params object[] args)
    {
        if (args.Length > 0)
            owner = (Squad)args[0];
    }

    public override object CalcUpgradedStats(List<DSPlayerSkill.SkillUpgrade> upgrades)
    {
        ChargeStats newStats = (ChargeStats)base.CalcUpgradedStats(upgrades);

        foreach (var upgrade in upgrades)
        {
            if (upgrade.isUpgradeToUnlock && upgrade.level > 0)
                newStats.unlocked = true;
        }

        return newStats;
    }
}
