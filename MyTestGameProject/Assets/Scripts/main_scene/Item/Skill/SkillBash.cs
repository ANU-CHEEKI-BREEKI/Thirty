using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Description;

public class SkillBash : Skill
{
    [Serializable]
    struct BashStats : ISkillLockable, ISkillCooldownable, ISkillDurationable, ISkillTwoPhases, ISkillDelayable, IDescriptionable
    {
        public enum Type { WEAPON, SHIELD }

        [SerializeField] Type bashType;
        public Type BaschType { get { return bashType; } }
        [Space]
        [Range(0, 1080)] public float firstPhaseDuration;
        public UnitStatsModifier firstPhaseModifyer;
        [Space]
        [Range(0, 1080)] public float secondPhaseDuration;
        public UnitStatsModifier secondPhaseModifyer;
        [Space]        
        [Range(0, 1080)] public float cooldown;
        [SerializeField] float delay;

        public bool unlocked;
        public bool Unlocked { get { return unlocked; } }
        public float Cooldown { get { return cooldown; } }
        public float Duration { get { return firstPhaseDuration + secondPhaseDuration; } }
        public float Delay { get { return delay; } }

        public object PhaseStats(int phase)
        {
            if (phase <= 1)
                return firstPhaseModifyer;
            else
                return secondPhaseModifyer;
        }

        public float PhaseDuration(int phase)
        {
            if (phase <= 1)
                return firstPhaseDuration;
            else
                return secondPhaseDuration;
        }

        public Description GetDescription()
        {
            var desc = firstPhaseModifyer.GetDescription();
            List<DescriptionItem> l = new List<DescriptionItem>(desc.Stats);
            l.Add(new DescriptionItem() { Name = LocalizedStrings.duration, Description = firstPhaseDuration.ToString(StringFormats.floatNumber), ItPositiveDesc = true });

            var desc2 = secondPhaseModifyer.GetDescription();
            List<DescriptionItem> l2 = new List<DescriptionItem>(desc2.Stats);
            l2.Add(new DescriptionItem() { Name = LocalizedStrings.duration, Description = secondPhaseDuration.ToString(StringFormats.floatNumber), ItPositiveDesc = true });

            DescriptionItem coold = new DescriptionItem()
            {
                ItPositiveDesc = true,
                Name = LocalizedStrings.cooldown,
                Description = cooldown.ToString(StringFormats.floatNumber)
            };

            DescriptionItem constr = new DescriptionItem()
            {
                ItPositiveDesc = false,
                Name = LocalizedStrings.attention,
                Description = "-non loc- Это умение имеет два этапа"
            };

            DescriptionItem[] c = new DescriptionItem[]
            {
                constr,
                coold
            };

            return new Description()
            {
                Constraints = c,
                StatsName = "-non loc- Первый этап:",
                Stats = l.ToArray(),
                SecondStatsName = "-non loc- Второй этап:",
                SecondStats = l2.ToArray()
            };
        }
    }

    [SerializeField] BashStats defaultStats;
    [SerializeField] protected List<FormationStats.Formations> canExecute;

    /// <summary>
    /// Учавствуе в CalcUpgradedStats. Вызывается родительским классом.
    /// </summary>
    public override object DefaultStats { get { return defaultStats; } }

    public override void Init(params object[] args)
    {
        if (args.Length > 0)
            owner = (Squad)args[0];
    }

    public override bool Execute(object skillStats)
    {
        var res = base.Execute(skillStats);
        BashStats stats = new BashStats();
        try
        {
            stats = (BashStats)skillStats;
        }
        catch
        {
            Debug.Log("Были переданы статы не того скила... скорее всего... кароче сто то с приведением типа...");
            return false;
        }

        if(owner != null)
        {
            if (canExecute.Contains(owner.CurrentFormation))
            {
                if (!(stats.BaschType == BashStats.Type.SHIELD && owner.Inventory.Shield.EquipmentStats.Empty))
                {
                    owner.StartCoroutine(ExecuteWithPhases(stats, owner));
                }
                else
                {
                    Debug.Log("Без щита незя бить щитом...");
                    Toast.Instance.Show("-non loc- Без щита незя бить щитом...");
                    res = false;
                }
            }
            else
            {
                Debug.Log("В данном построении нельзя использовать скилл");
                Toast.Instance.Show("-non loc- В данном построении нельзя использовать это умение");
                res = false;
            }
        }
        else
        {
            Debug.Log("Без овнера этот скилл не работает...");
            res = false;
        }

        return res;
    }

    IEnumerator ExecuteWithPhases(BashStats stats, Squad owner)
    {
        //ждем задержку
        yield return new WaitForSeconds(stats.Delay);

        //первая фаза

        //добавляем модификатор
        owner.AddStatsModifier(stats.firstPhaseModifyer);

        //если оружием то оружием если нет то нет
        bool weapon;
        if (stats.BaschType == BashStats.Type.WEAPON)
            weapon = true;
        else
            weapon = false;

        //наносим удар врагу
        int cnt = owner.UnitCount;
        var pos = owner.UnitPositions;
        for (int i = 0; i < cnt; i++)
        {
            var unit = pos[i].Unit;
            var unitStats = unit.Stats;

            var enemyes = unit.FindEnemyes(weapon);
            int cnt2 = enemyes.Length;
            for (int j = 0; j < cnt2; j++)
            {
                if (!weapon)
                    enemyes[j].FallDown();
                enemyes[j].TakeHit(unitStats.Damage);
            }
        }

        //ждем вторую фазу
        yield return new WaitForSeconds(stats.firstPhaseDuration);
       
        //вторая фаза

        //убираем прошлый модификатор и добавляем новый
        owner.RemoveStatsModifier(stats.firstPhaseModifyer);
        owner.AddStatsModifier(stats.secondPhaseModifyer);

        //ждем конца скилла
        yield return new WaitForSeconds(stats.secondPhaseDuration);

        //убираем модификатор
        owner.RemoveStatsModifier(stats.secondPhaseModifyer);
    }

    public override Description GetDescription()
    {
        var desc = base.GetDescription();

        DescriptionItem canUse = new DescriptionItem();
        canUse.ItPositiveDesc = false;

        canUse.Name = LocalizedStrings.attention;

        StringBuilder sb = new StringBuilder();
        sb.Append("-non loc- можно использовать в построениях: ");
        for (int i = 0; i < canExecute.Count; i++)
        {
            sb.Append(canExecute[i].GetNamelocalize());
            if(i < canExecute.Count - 1)
                sb.Append(", ");
        }
        canUse.Description = sb.ToString();
        desc.Constraints = new DescriptionItem[] { canUse };

        return desc;
    }

}
