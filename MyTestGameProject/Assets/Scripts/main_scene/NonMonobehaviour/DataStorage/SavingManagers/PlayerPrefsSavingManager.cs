using System;
using UnityEngine;

public class PlayerPrefsSavingManager : ISavingManager
{
    public override void SaveData<T>(string name, object data)
    {
        PlayerPrefs.SetString(name, JsonUtility.ToJson((T)data));
        CallOnDataSaved(name, true);
    }

    public override void LoadData<T>(string name)
    {
        if (!PlayerPrefs.HasKey(name))
            SaveData<T>(name, (T)Activator.CreateInstance(typeof(T)));
        CallOnDataLoaded(name, JsonUtility.FromJson<T>(PlayerPrefs.GetString(name)));
    }
}
