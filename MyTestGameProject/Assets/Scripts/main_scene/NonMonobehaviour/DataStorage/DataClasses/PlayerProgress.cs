using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress : ISavable, IResetable
{
    public DSPlayerScore score;
    public DSUnitStats stats;
    public DSPlayerSkills skills;

    public PlayerProgress()
    {
        score = new DSPlayerScore();
        stats = new DSUnitStats();
        skills = new DSPlayerSkills();
    }

    public void Load()
    {
        score.Load();
        stats.Load();
        skills.Load();
    }   

    public void Save()
    {
        score.Save();
        stats.Save();
        skills.Save();
    }

    public void Reset()
    {
        score.Reset();
        stats.Reset();
        skills.Reset();
    }
}
