using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavablePlayerData : ISavable, IResetable, IMergeable, ICopyabe
{
    [Header("Score")]
    [SerializeField] PlayerProgress playerProgress;
    public PlayerProgress PlayerProgress { get { return playerProgress; } private set { playerProgress = value; } }

    [Header("Settings")]
    [SerializeField] Settings settings;
    public Settings Settings { get { return settings; } private set { settings = value; } }

    bool checkedS = false;
    bool savedP = false;
    bool savedS = false;

    bool chechedL = false;
    bool loadedP = false;
    bool loadedS = false;

    public event Action OnSaved;
    public event Action OnLoaded;

    public SavablePlayerData()
    {
        playerProgress = new PlayerProgress();
        settings = new Settings(true);

        Reset();
    }

    public void Reset()
    {
        playerProgress.Reset();
        settings.Reset();
    }

    string mesLoad = "[non loc] Загрузка данных...";
    public void Load()
    {
        chechedL = false;
        loadedP = false;
        loadedS = false;

        ModalInfoPanel.Instance.Add(mesLoad);

        Action p = null;
        p = () =>
        {
            loadedP = true;
            playerProgress.OnLoaded -= p;

            CheckLoaded();
        };
        playerProgress.OnLoaded += p;

        Action s = null;
        s = () =>
        {
            loadedS = true;
            settings.OnLoaded -= s;

            CheckLoaded();
        };
        settings.OnLoaded += s;

        playerProgress.Load();
        settings.Load();
    }

    string mesSave = "[non loc] Сохранение данных...";
    public void Save()
    {
        checkedS = false;
        savedP = false;
        savedS = false;

        ModalInfoPanel.Instance.Add(mesSave);

        Action p = null;
        p = () =>
        {
            savedP = true;
            playerProgress.OnSaved -= p;

            CheckSaved();
        };
        playerProgress.OnSaved += p;

        Action s = null;
        s = () =>
        {
            savedS = true;
            settings.OnSaved -= s;

            CheckSaved();
        };
        settings.OnSaved += s;

        playerProgress.Save();
        settings.Save();
    }

    void CheckSaved()
    {
        if (!checkedS && savedP && savedS)
        {
            ModalInfoPanel.Instance.Remove(mesSave);
            if(OnSaved != null)
                OnSaved();
            checkedS = true;
        }
    }

    void CheckLoaded()
    {
        if (!chechedL && loadedP && loadedS)
        {
            ModalInfoPanel.Instance.Remove(mesLoad);
            if(OnLoaded != null)
                OnLoaded();
            chechedL = true;
        }
    }

    public void Merge(object data)
    {
        var d = data as SavablePlayerData;

        playerProgress.Merge(d.playerProgress);
    }

    public object Copy()
    {
        var str = JsonUtility.ToJson(this);
        return JsonUtility.FromJson<SavablePlayerData>(str);
    }
}
