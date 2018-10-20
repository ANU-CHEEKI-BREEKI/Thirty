using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress : IResetable, ITempValuesApplyable, ILoadedDataApplyable, ISavable, IMergeable
{
    [SerializeField] DateTime savedDateTime;

    [SerializeField] DSFlags flags;
    [SerializeField] DSPlayerScore score;
    [SerializeField] DSUnitStats stats;
    [SerializeField] DSPlayerSkills skills;
    [SerializeField] DSPlayerEquipment equipment;
    [SerializeField] DSPlayerSquad squad;
    [SerializeField] LevelInfo level; 

    public DSFlags Flags { get { return flags; } private set { flags = value; } }
    public DSPlayerScore Score { get { return score; } private set { score = value; } }
    public DSUnitStats Stats { get { return stats; } private set { stats = value; } }
    public DSPlayerSkills Skills { get { return skills; } private set { skills = value; } }
    /// <summary>
    /// Allowed equipment which will be able in market
    /// </summary>
    public DSPlayerEquipment Equipment { get { return equipment; } private set { equipment = value; } }
    public DSPlayerSquad Squad { get { return squad; } private set { squad = value; } }
    public LevelInfo Level { get { return level; } private set { level = value; } }

    public event Action OnSaved;
    public event Action OnLoaded;

    public PlayerProgress()
    {
        Flags = new DSFlags();
        Score = new DSPlayerScore();
        Stats = new DSUnitStats();
        Skills = new DSPlayerSkills();
        Equipment = new DSPlayerEquipment();
        Squad = new DSPlayerSquad();
        Level = new LevelInfo();
    }
    
    public void Reset()
    {
        Flags.Reset();
        Score.Reset();
        Stats.Reset();
        Skills.Reset();
        Equipment.Reset();
        Squad.Reset();
        Level.Reset();
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

    public void ApplyLoadedData(object data)
    {
        var d = data as PlayerProgress;

        Flags = d.Flags;
        Score.ApplyLoadedData(d.Score);
        Stats = d.Stats;
        Skills.ApplyLoadedData(d.Skills);
        Equipment = d.Equipment;
        Squad.ApplyLoadedData(d.Squad);
        Level = d.Level;
    }

    public void Merge(object data)
    {
        var d = data as PlayerProgress;

        Flags.Merge(d.Flags);
        Score.Merge(d.Score);
        Stats.Merge(d.Stats);
        Skills.Merge(d.Skills);
        Equipment.Merge(d.Equipment);
        Squad.Merge(d.Squad);
        Level.Merge(d.Level);
    }

    public void Save()
    {
        var mes = "[non loc] Сохранение прогресса игры...";

        savedDateTime = DateTime.Now;

        ModalInfoPanel.Instance.Add(mes);
        Action<string, bool> onSaved = null;
        onSaved = (s, b) =>
        {
            if (s == this.GetType().Name)
            {
                ModalInfoPanel.Instance.Remove(mes);
                if (OnSaved != null)
                    OnSaved();
                GameManager.Instance.SavingManager.OnDataSaved -= onSaved;
            }
        };
        GameManager.Instance.SavingManager.OnDataSaved += onSaved;
        GameManager.Instance.SavingManager.SaveData<PlayerProgress>(this.GetType().Name, this);
    }

    public void Load()
    {
        var mes = "[non loc] Загрузка сохранённого прогресса...";

        ModalInfoPanel.Instance.Add(mes);
        Action<string, object, bool> onLoad = null;
        onLoad = (s, p, success) =>
        {
            if (s == this.GetType().Name)
            {
                ApplyLoadedData(p);
                ModalInfoPanel.Instance.Remove(mes);
                if (OnLoaded != null)
                    OnLoaded();
                GameManager.Instance.SavingManager.OnDataLoaded -= onLoad;
            }
        };
        GameManager.Instance.SavingManager.OnDataLoaded += onLoad;
        GameManager.Instance.SavingManager.LoadData<PlayerProgress>(this.GetType().Name);
    }

}
