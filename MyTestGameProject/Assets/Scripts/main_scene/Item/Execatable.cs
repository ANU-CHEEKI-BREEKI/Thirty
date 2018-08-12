using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public abstract class Executable : Item, IDescriptionable
{
    public enum ExecatableUseType { CLICK, DRAG_DROP_PLASE, DRAG_DIRECTION }

    [Header("Execatable")]

    [SerializeField] int id;
    public int Id { get { return id; } }

    [SerializeField] ExecatableUseType useType;
    public ExecatableUseType UseType { get { return useType; } }

    [HideInInspector] public Squad owner;

    public abstract object DefaultStats { get; }

    /// <summary>
    /// описание класса Skill
    /// </summary>
    public virtual bool Execute(object skillStats)
    {
        if (owner != null)
        {
            Color sc = Color.black;
            Color ec;
            var gs = GameManager.Instance.Settings.graphixSettings;
            switch (owner.fraction)
            {
                case Squad.UnitFraction.ALLY:
                    sc = gs.AllyOutlineColor;
                    break;
                case Squad.UnitFraction.ENEMY:
                    sc = gs.EnemyOutlineColor;
                    break;
                case Squad.UnitFraction.NEUTRAL:
                    sc = gs.NeutralOutlineColor;
                    break;
            }
            ec = new Color(sc.r, sc.g, sc.b, 0.5f);

            PopUpTextController.Instance.AddTextLabel(
                Localization.GetString(MainPropertie.StringResourceName),
                owner.CenterSquad,
                startColor: sc,
                endColor: ec,
                fontSize: 15
            );
        }

        return true;
    }
    

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
