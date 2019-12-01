using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress : IResetable, ITempValuesApplyable, ISavable, IMergeable, ILoadedDataApplyable
{
    [SerializeField] DateTime savedDateTime;

    [SerializeField] DSFlags flags;
    [SerializeField] DSPlayerScore score;
    [SerializeField] DSUnitStats stats;
    [SerializeField] DSPlayerSkills skills;
    /// <summary>
    /// доступная в магазине экипировка
    /// </summary>
    [SerializeField] DSPlayerEquipment equipment;
    /// <summary>
    /// кол-во солдат, хп, инвентарь
    /// </summary>
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

    public void UndoSettingsChanges()
    {
        Score.UndoSettingsChanges();
        Equipment.UndoSettingsChanges();
    }

    public void RecordSettings()
    {
        Score.RecordSettings();
        Equipment.RecordSettings();
    }

    public void ApplyLoadedData(object data)
    {
        Debug.Log("------------PlayerProgress loaded data applying...");

        var d = data as PlayerProgress;
        if (d == null)
            d = new PlayerProgress();

        Flags = d.Flags;
        Score.ApplyLoadedData(d.Score);
        Stats = d.Stats;
        Skills.ApplyLoadedData(d.Skills);
        Equipment = d.Equipment;
        Squad.ApplyLoadedData(d.Squad);
        Level = d.Level;

        Debug.Log("------------PlayerProgress loaded data was applyed");
    }

    public void Merge(object data)
    {
        Debug.Log("------------PlayerProgress data merging...");

        var d = data as PlayerProgress;

        Flags.Merge(d.Flags);
        Score.Merge(d.Score);
        Stats.Merge(d.Stats);
        Skills.Merge(d.Skills);
        Equipment.Merge(d.Equipment);
        Squad.Merge(d.Squad);
        Level.Merge(d.Level);

        Debug.Log("------------PlayerProgress data merged");
    }

    public void Save()
    {
        Debug.Log("------------PlayerProgress data saving...");

        var mes = LocalizedStrings.saving_progress;

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

            Debug.Log("------------PlayerProgress data saved: " + b);
        };
        GameManager.Instance.SavingManager.OnDataSaved += onSaved;
        GameManager.Instance.SavingManager.SaveData<PlayerProgress>(this.GetType().Name, this);
    }

    public void Load()
    {
        var mes = LocalizedStrings.loading_progress;

        Debug.Log("------------PlayerProgress data loading");

        ModalInfoPanel.Instance.Add(mes);
        Action<string, object, bool> onLoad = null;
        onLoad = (s, p, success) =>
        {
            if (s == this.GetType().Name)
            {
                Debug.Log("------------PlayerProgress saved data was read");

                try
                {
                    ApplyLoadedData(p);
                }
                catch(Exception ex)
                {
                    Debug.LogError("------------PlayerProgress data applying failed\r\n" + ex.ToString());
                }
                ModalInfoPanel.Instance.Remove(mes);
                if (OnLoaded != null)
                    OnLoaded();
                GameManager.Instance.SavingManager.OnDataLoaded -= onLoad;
            }

            Debug.Log("------------PlayerProgress data loaded: " + success);
        };
        GameManager.Instance.SavingManager.OnDataLoaded += onLoad;
        GameManager.Instance.SavingManager.LoadData<PlayerProgress>(this.GetType().Name);
    }

}
