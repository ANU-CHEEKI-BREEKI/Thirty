using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress : ISavable, IResetable
{
    [SerializeField] DSFlags flags;
    [SerializeField] DSPlayerScore score;
    [SerializeField] DSUnitStats stats;
    [SerializeField] DSPlayerSkills skills;

    public DSFlags Flags { get { return flags; } private set { flags = value; } }
    public DSPlayerScore Score { get { return score; } private set { score = value; } }
    public DSUnitStats Stats { get { return stats; } private set { stats = value; } }
    public DSPlayerSkills Skills { get { return skills; } private set { skills = value; } }

    public PlayerProgress()
    {
        Flags = new DSFlags();
        Score = new DSPlayerScore();
        Stats = new DSUnitStats();
        Skills = new DSPlayerSkills();
    }

    public void Load()
    {
        Flags.Load();
        Score.Load();
        Stats.Load();
        Skills.Load();
    }

    public void Save()
    {
        Flags.Save();
        Score.Save();
        Stats.Save();
        Skills.Save();
    }

    public void Reset()
    {
        Flags.Reset();
        Score.Reset();
        Stats.Reset();
        Skills.Reset();
    }
}
