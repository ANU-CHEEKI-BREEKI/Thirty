using System;
using UnityEngine;

[Serializable]
public class Settings : IResetable, ILoadedDataApplyable, ISavable, ITempValuesApplyable, ICopyabe
{
    [SerializeField] DateTime savedDateTime;

    public GraphixSettings graphixSettings;
    public AudioSettings audioSettings;
    public CommonSettings commonSettings;

    public event Action OnSaved;
    public event Action OnLoaded;

    [Space]
    [SerializeField] Settings temporarySettings;
    /// <summary>
    /// Используются для временного хранения настроек. ---
    /// Например, меняем основные. Если не понравилость, просто применяем временные. Типа для бэкапа, короче
    /// </summary>
    public Settings TemporarySettings { get { return temporarySettings; } private set { temporarySettings = value; } }

    public Settings()
    {
        graphixSettings = new GraphixSettings();
        audioSettings = new AudioSettings();
        commonSettings = new CommonSettings();
    }

    public Settings(bool resetTemp = false) : this()
    {
        if (resetTemp)
            ResetTempValues();
        else
            TemporarySettings = null;
    }

    public void ApplyLoadedData(object data)
    {
        var d = data as Settings;

        graphixSettings = d.graphixSettings;
        audioSettings.ApplyLoadedData(d.audioSettings);
        commonSettings = d.commonSettings;
    }

    public void Reset()
    {
        graphixSettings.Reset();
        audioSettings.Reset();
        commonSettings.Reset();
    }

    public void Save()
    {
        var mes = "[non loc] Сохранение настроек игры...";

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
        GameManager.Instance.SavingManager.SaveData<Settings>(this.GetType().Name, this);
    }

    public void Load()
    {
        var mes = "[non loc] Загрузка сохраннных настроек...";

        ModalInfoPanel.Instance.Add(mes);
        Action<string, object, bool> onLoad = null;
        onLoad = (s, p, success) =>
        {
            if (s == this.GetType().Name)
            {
                ApplyLoadedData(p);
                ResetTempValues();
                ModalInfoPanel.Instance.Remove(mes);
                if (OnLoaded != null)
                    OnLoaded();
                GameManager.Instance.SavingManager.OnDataLoaded -= onLoad;
            }
        };
        GameManager.Instance.SavingManager.OnDataLoaded += onLoad;
        GameManager.Instance.SavingManager.LoadData<Settings>(this.GetType().Name);
    }

    public void ApplyTempValues()
    {
        ApplyLoadedData(TemporarySettings);
    }

    public void ResetTempValues()
    {
        TemporarySettings = Copy() as Settings;
    }

    public object Copy()
    {
        return new Settings(false)
        {
            graphixSettings = this.graphixSettings.Copy() as GraphixSettings,
            audioSettings = this.audioSettings.Copy() as AudioSettings,
            commonSettings = this.commonSettings.Copy() as CommonSettings
        };
    }
}
