using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillStack : AStack, IDescriptionable
{
    public event Action<Skill> OnSkillChanged;
    public event Action<object> OnStatsChanged;

    [SerializeField] Skill skill;
    public Skill Skill { get { return skill; } set { skill = value; if (OnSkillChanged != null) OnSkillChanged(value); } }

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

    public SkillStack()
    {
    }

    public SkillStack(Skill skill, object skillStats)
    {
        this.skill = skill;
        this.skillStats = skillStats;
    }

    public Description GetDescription()
    {
        Description skillDesc = skill.GetDescription();
        var desc = skillStats as IDescriptionable;
        if (desc != null)
        {
            Description statsDesc = desc.GetDescription();
            skillDesc.Constraints = statsDesc.Constraints;
            skillDesc.Stats = statsDesc.Stats;
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
