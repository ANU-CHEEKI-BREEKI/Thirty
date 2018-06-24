using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DSPlayerSkills : ISavable, IResetable
{
    public List<DSPlayerSkill> skills;
    public Executable firstSkill;
    public Executable secondSkill;

    public DSPlayerSkills()
    {
        Reset();
    }

    public void Save()
    {
        GameManager.Instance.SavingManager.SaveData<DSPlayerSkills>(this.GetType().Name, this);
    }

    public void Load()
    {
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
        var g = GameManager.Instance.SavingManager.LoadData<DSPlayerSkills>(this.GetType().Name);
        var fields = this.GetType().GetFields(flags);
        foreach (var f in fields)
            f.SetValue(this, f.GetValue(g));
    }

    public void Reset()
    {
        skills = new List<DSPlayerSkill>();
        skills.Add(new DSPlayerSkill(1));
        skills.Add(new DSPlayerSkill(2));
    }
}
