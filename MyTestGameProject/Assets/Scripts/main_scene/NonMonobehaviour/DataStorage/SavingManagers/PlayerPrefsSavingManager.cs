using System;
using UnityEngine;

public class PlayerPrefsSavingManager : ISavingManager
{
    public T LoadData<T>(string name)
    {
        if (!PlayerPrefs.HasKey(name))
            SaveData<T>(name, Activator.CreateInstance(typeof(T)));

        return JsonUtility.FromJson<T>(PlayerPrefs.GetString(name));
    }

    public void SaveData<T>(string name, object data)
    {
        PlayerPrefs.SetString(name, JsonUtility.ToJson((T)data));
    }
}
