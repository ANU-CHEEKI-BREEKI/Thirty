using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class DSFlags : ISavable, IResetable
{
    [SerializeField] bool isFirstStartGame;
    [SerializeField] bool needTraining;

    public bool IsFirstStartGame { get { return isFirstStartGame; } private set { isFirstStartGame = value; } }
    public bool NeedTraining { get { return needTraining; } private set { needTraining = value; } }

    public DSFlags()
    {
        Reset();
    }

    public void Save()
    {
        GameManager.Instance.SavingManager.SaveData<DSFlags>(this.GetType().Name, this);
    }

    public void Load()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var g = GameManager.Instance.SavingManager.LoadData<DSFlags>(this.GetType().Name);
        var fields = this.GetType().GetFields(flags);
        foreach (var f in fields)
            f.SetValue(this, f.GetValue(g));
    }

    public void Reset()
    {
        isFirstStartGame = true;
        needTraining = true;
    }
}
