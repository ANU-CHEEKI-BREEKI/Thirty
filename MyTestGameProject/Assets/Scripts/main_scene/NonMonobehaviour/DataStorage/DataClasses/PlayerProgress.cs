using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress : IResetable, ITempValuesApplyable, ILoadedDataApplyable, ISavable
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

    public event Action OnSaved;
    public event Action OnLoaded;

    public PlayerProgress()
    {
        Flags = new DSFlags();
        Score = new DSPlayerScore();
        Stats = new DSUnitStats();
        Skills = new DSPlayerSkills();
        Equipment = new DSPlayerEquipment();
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

    public void ApplyLoadedData(object data)
    {
        var d = data as PlayerProgress;

        Flags = d.Flags;
        Score.ApplyLoadedData(d.Score);
        Stats = d.Stats;
        Skills = d.Skills;
        Equipment = d.Equipment;
    }

    public void Save()
    {
        var mes = "[non loc] Сохранение прогресса игры...";

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
        Action<string, object> onLoad = null;
        onLoad = (s, p) =>
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
