using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DSPlayerSkills : IResetable
{
    public List<DSPlayerSkill> skills;
    public Executable firstSkill;
    public Executable secondSkill;

    public DSPlayerSkills()
    {
        Reset();
    }

    public void Reset()
    {
        skills = new List<DSPlayerSkill>();
        firstSkill = null;
        secondSkill = null;
    }
}
