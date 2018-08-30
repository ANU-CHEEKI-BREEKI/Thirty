using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress : ISavable, IResetable, ITempValuesApplyable
{
    [SerializeField] DSFlags flags;
    [SerializeField] DSPlayerScore score;
    [SerializeField] DSUnitStats stats;
    [SerializeField] DSPlayerSkills skills;
    [SerializeField] DSPlayerEquipment equipment;

    public DSFlags Flags { get { return flags; } private set { flags = value; } }
    public DSPlayerScore Score { get { return score; } private set { score = value; } }
    public DSUnitStats Stats { get { return stats; } private set { stats = value; } }
    public DSPlayerSkills Skills { get { return skills; } private set { skills = value; } }
    /// <summary>
    /// Allowed equipment which will be able in market
    /// </summary>
    public DSPlayerEquipment Equipment { get { return equipment; } private set { equipment = value; } }

    public PlayerProgress()
    {
        Flags = new DSFlags();
        Score = new DSPlayerScore();
        Stats = new DSUnitStats();
        Skills = new DSPlayerSkills();
        Equipment = new DSPlayerEquipment();
    }

    public void Load()
    {
        Flags.Load();
        Score.Load();
        Stats.Load();
        Skills.Load();
        Equipment.Load();
    }

    public void Save()
    {
        Flags.Save();
        Score.Save();
        Stats.Save();
        Skills.Save();
        Equipment.Save();
    }

    public void Reset()
    {
        Flags.Reset();
        Score.Reset();
        Stats.Reset();
        Skills.Reset();
        Equipment.Reset();
    }

    public void ApplyTempValues()
    {
        Score.ApplyTempValues();
        Equipment.ApplyTempValues();
    }

    public void ResetTempValues()
    {
        Score.ResetTempValues();
        Equipment.ResetTempValues();
    }
}
