﻿using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

[Serializable]
public class SkillStack : AExecutableStack, IDescriptionable
{
    public event Action<Executable> OnSkillChanged;
    public event Action<object> OnStatsChanged;

    [SerializeField] Executable skill;
    public Executable Skill { get { return skill; } set { skill = value; if (OnSkillChanged != null) OnSkillChanged(value); } }

    [SerializeField] object skillStats;
    public object SkillStats
    {
        get
        {
            if(skillStats == null && skill != null)
                skillStats = skill.DefaultStats;
            return skillStats;
        }
        set
        {
            skillStats = value; if (OnStatsChanged != null) OnStatsChanged(value);
        }
    }

    public override Item.MainProperties? MainProperties { get { if (skill != null) return skill.MainPropertie; else return null; } }

    public override Item Item
    {
        get
        {
            return skill;
        }
    }

    public override object Stats
    {
        get
        {
            return SkillStats;
        }
    }

    public SkillStack()
    {
    }

    public SkillStack(Executable skill, object skillStats)
    {
        this.skill = skill;
        this.skillStats = skillStats;
    }

    public Description GetDescription()
    {
        Description skillDesc = skill.GetDescription();
        var stats = skillStats as IDescriptionable;
        if (stats != null)
        {
            Description statsDesc = stats.GetDescription();

            var list = new List<Description.DescriptionItem>();
            if(skillDesc.Constraints != null)
                list.AddRange(skillDesc.Constraints);
            if (statsDesc.Constraints != null)
                list.AddRange(statsDesc.Constraints);
            skillDesc.Constraints = list.ToArray();

            list.Clear();
            if (skillDesc.Stats != null)
                list.AddRange(skillDesc.Stats);
            if (statsDesc.Stats != null)
                list.AddRange(statsDesc.Stats);
            skillDesc.Stats = list.ToArray();

            list.Clear();
            if (skillDesc.SecondStats != null)
                list.AddRange(skillDesc.SecondStats);
            if (statsDesc.SecondStats != null)
                list.AddRange(statsDesc.SecondStats);
            skillDesc.SecondStats = list.ToArray();

            skillDesc.StatsName = statsDesc.StatsName;
            skillDesc.SecondStatsName = statsDesc.SecondStatsName;

            skillDesc.Cost = statsDesc.Cost;

            if (skillDesc.Cost != null)
                skillDesc.Cost = new Description.CostInfo()
                {
                    CostPerOne = skillDesc.Cost.Value.CostPerOne,
                    CostAll = null,
                    CostCurrency = skill.MainPropertie.Currency
                };
        }
        skillDesc.Icon = skill.MainPropertie.Icon;
        skillDesc.UseType = skill.UseType.GetNameLocalise();

        return skillDesc;
    }
}
