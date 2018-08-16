using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tools;
using UnityEngine;
using static Description;

public class SkillBuffSelf : Skill
{
    [Serializable]
    struct BuffStats : ISkillLockable, ISkillCooldownable, ISkillDurationable, ISkillTwoPhases, IDescriptionable
    {
        [Range(0, 1080)] public float firstPhaseDuration;
        public UnitStatsModifier firstPhaseModifyer;
        [Space]
        [Range(0, 1080)] public float secondPhaseDuration;
        public UnitStatsModifier secondPhaseModifyer;
        [Space]
        [Range(0, 1080)] public float cooldown;

        public bool unlocked;
        public bool Unlocked { get { return unlocked; } }
        public float Cooldown { get { return cooldown; } }
        public float Duration { get { return firstPhaseDuration + secondPhaseDuration; } }

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

            if (secondPhaseDuration > 0)
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
                Description = LocalizedStrings.two_phases_skill
            };


            List<DescriptionItem> c = new List<DescriptionItem>();
            if (secondPhaseDuration > 0)
                c.Add(constr);

            if (secondPhaseDuration > 0)
                c.Add(coold);
            else
                l.Add(coold);

            var res = new Description()
            {
                Constraints = c.ToArray(),
                Stats = l.ToArray(),
            };

            if (secondPhaseDuration > 0)
            {
                res.StatsName = LocalizedStrings.first_phase;
                res.SecondStatsName = LocalizedStrings.second_phase;
                res.SecondStats = l2.ToArray();
            }
            return res;
        }
    }

    [SerializeField] BuffStats defaultStats;

    Dictionary<Squad, Coroutine> coroutines = new Dictionary<Squad, Coroutine>();

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
        if (res)
        {
            BuffStats stats = new BuffStats();
            try
            {
                stats = (BuffStats)skillStats;
            }
            catch
            {
                Debug.Log("Были переданы статы не того скила... скорее всего... кароче сто то с приведением типа...");
                return false;
            }

            if (owner != null)
            {
                Coroutine cor = null;
                if (coroutines.ContainsKey(owner))
                    cor = coroutines[owner];
                if (cor != null)
                    owner.StopCoroutine(cor);
                coroutines[owner] = owner.StartCoroutine(ExecuteWithPhases(stats, owner));
            }
            else
            {
                Debug.Log("Без овнера этот скилл не работает...");
                res = false;
            }
        }
        return res;
    }

    IEnumerator ExecuteWithPhases(BuffStats stats, Squad owner)
    {
        //первая фаза

        //добавляем модификатор
        owner.AddStatsModifier(stats.firstPhaseModifyer);
        
        //ждем вторую фазу
        yield return new WaitForSeconds(stats.firstPhaseDuration);

        //убираем модификатор
        owner.RemoveStatsModifier(stats.firstPhaseModifyer);

        //вторая фаза

        //если вторая фаза есть вообще
        if (stats.secondPhaseDuration > 0)
        {
            //добавляем модификатор
            owner.AddStatsModifier(stats.secondPhaseModifyer);

            //ждем конца скилла
            yield return new WaitForSeconds(stats.secondPhaseDuration);

            //убираем модификатор
            owner.RemoveStatsModifier(stats.secondPhaseModifyer);
        }

        coroutines[owner] = null;
    }

    public override object CalcUpgradedStats(List<DSPlayerSkill.SkillUpgrade> upgrades)
    {
        BuffStats newStats = (BuffStats)base.CalcUpgradedStats(upgrades);

        foreach (var upgrade in upgrades)
        {
            if (upgrade.isUpgradeToUnlock && upgrade.level > 0)
                newStats.unlocked = true;
        }

        return newStats;
    }
}
