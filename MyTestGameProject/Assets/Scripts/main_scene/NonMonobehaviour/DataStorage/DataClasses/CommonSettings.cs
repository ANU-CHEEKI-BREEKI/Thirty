using System;
using UnityEngine;

[Serializable]
public class CommonSettings : ISavable, IResetable
{
    /// <summary>
    /// Язык локализации
    /// </summary>
    public SystemLanguage Language = SystemLanguage.Russian;

    public void Save()
    {
        GameManager.Instance.SavingManager.SaveData<CommonSettings>(this.GetType().Name, this);
    }

    public void Load()
    {
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
        var g = GameManager.Instance.SavingManager.LoadData<CommonSettings>(this.GetType().Name);
        var fields = this.GetType().GetFields(flags);
        foreach (var f in fields)
            f.SetValue(this, f.GetValue(g));
    }

    public void Reset()
    {
        Language = SystemLanguage.Russian;
    }
}