using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavablePlayerData : ISavable, IResetable
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

    public void Reset()
    {
        playerProgress.Reset();
        settings.Reset();
    }

    void CheckSaved()
    {
        if (!checkedS && OnSaved != null && savedP && savedS)
        {
            ModalInfoPanel.Instance.Remove(mesSave);
            OnSaved();
            checkedS = true;
        }
    }

    void CheckLoaded()
    {
        if (OnLoaded != null && loadedP && loadedS)
        {
            ModalInfoPanel.Instance.Remove(mesLoad);
            OnLoaded();
            chechedL = true;
        }
    }
}
