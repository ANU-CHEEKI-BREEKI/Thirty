using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Executable : Item, IDescriptionable
{
    public enum ExecatableUseType { CLICK, DRAG_DROP_PLASE, DRAG_DIRECTION }

    [Header("Execatable")]

    [SerializeField] int id;
    public int Id { get { return id; } }

    [SerializeField] ExecatableUseType useType;
    public ExecatableUseType UseType { get { return useType; } }

    
    public abstract object DefaultStats { get; } 

    /// <summary>
    /// описание класса Skill
    /// </summary>
    public abstract bool Execute(object skillStats);

    /// <summary>
    /// описание класса Skill
    /// </summary>
    /// <param name="args">описание</param>
    public abstract void Init(params System.Object[] args);

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
