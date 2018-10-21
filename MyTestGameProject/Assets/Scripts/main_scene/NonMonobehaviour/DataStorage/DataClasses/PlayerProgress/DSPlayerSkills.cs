using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DSPlayerSkills : IResetable, ILoadedDataApplyable
{
    public List<DSPlayerSkill> skills;
    public SkillStack firstSkill;
    public SkillStack secondSkill;

    public DSPlayerSkills()
    {
        Reset();
    }

    public void ApplyLoadedData(object data)
    {
        var d = data as DSPlayerSkills;
        if (d == null)
            d = new DSPlayerSkills();

        firstSkill.ApplyLoadedData(d.firstSkill);
        secondSkill.ApplyLoadedData(d.secondSkill);

        skills = d.skills;
    }

    public void Merge(object data)
    {
        var d = data as DSPlayerSkills;

        var tempList = new List<DSPlayerSkill>(skills);
        var newList = new List<DSPlayerSkill>(d.skills);

        foreach (var item in tempList)
            newList.RemoveAll((e)=> { return e.Id == item.Id; });

        skills.AddRange(newList);
    }

    public void Reset()
    {
        skills = new List<DSPlayerSkill>();
        firstSkill = null;
        secondSkill = null;
    }
}
