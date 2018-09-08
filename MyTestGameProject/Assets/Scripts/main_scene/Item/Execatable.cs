using System;
using System.Collections.Generic;
using System.Text;
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

    [SerializeField] List<FormationStats.Formations> cantExecute;
    
    /// <summary>
    /// описание класса Skill
    /// </summary>
    public virtual bool Execute(object skillStats)
    {
        bool res = true;
        if (owner != null)
        {
            if (!cantExecute.Contains(owner.CurrentFormation))
            {
                Color sc = Color.black;
                Color ec;
                var gs = GameManager.Instance.SavablePlayerData.Settings.graphixSettings;
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
            else
            {
                Toast.Instance.Show(LocalizedStrings.toast_cant_use_skill_in_this_formation);
                res = false;
            }
        }

        return res;
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
        var desc = new Description()
        {
            Name = Localization.GetString(MainPropertie.StringResourceName),
            Desc = Localization.GetString(MainPropertie.StringResourceDescription)
        };

        Description.DescriptionItem canUse = new Description.DescriptionItem();
        canUse.ItPositiveDesc = false;

        canUse.Name = LocalizedStrings.attention;

        StringBuilder sb = new StringBuilder();
        sb.Append(LocalizedStrings.cant_use_in_this_formations);
        for (int i = 0; i < cantExecute.Count; i++)
        {
            sb.Append(cantExecute[i].GetNamelocalize());
            if (i < cantExecute.Count - 1)
                sb.Append(", ");
        }
        canUse.Description = sb.ToString();
        desc.Constraints = new Description.DescriptionItem[] { canUse };


        return desc;
    }
}
