using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : Item, IDescriptionable
{
    public enum SkillUseType { CLICK, DRAG_DROP_PLASE, DRAG_DIRECTION }

    [Header("Skill")]

    [SerializeField] int id;
    public int Id { get { return id; } }

    [SerializeField] SkillUseType useType;
    public SkillUseType UseType { get { return useType; } }

    [SerializeField] AnimationCurve upgradeLevelCost;
    public AnimationCurve UpgradeLevelCost { get { return upgradeLevelCost; } }

    public abstract object DefaultStats { get; } 

    /// <summary>
    /// описание класса Skill
    /// </summary>
    public abstract bool DoSkill(object skillStats);

    /// <summary>
    /// описание класса Skill
    /// </summary>
    /// <param name="args">описание</param>
    public abstract void InitSkill(params System.Object[] args);

    public virtual object CalcUpgradedStats(List<DSPlayerSkill.SkillUpgrade> upgrades)
    {
        object stats = DefaultStats;

        foreach (var u in upgrades)
        {
            if (u.FieldName != null && u.FieldName != "")
            {
                var names = u.FieldName.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                stats = stats.IncreaseNumberField(u.AdditionalValue, names);
            }
        }

        return stats;
    }

    public virtual Description GetDescription()
    {
        return new Description()
        {
            Name = Localization.GetString(MainPropertie.StringResourceName),
            Desc = Localization.GetString(MainPropertie.StringResourceDescription)
        };
    }
}
