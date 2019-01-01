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
        if (d == null)
            d = new Settings();

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
        Debug.Log("------------Settings data saving...");

        var mes = LocalizedStrings.saving_settings;

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

            Debug.Log("------------Settings data saved: " + b);
        };
        GameManager.Instance.SavingManager.OnDataSaved += onSaved;
        GameManager.Instance.SavingManager.SaveData<Settings>(this.GetType().Name, this);
    }

    public void Load()
    {
        var mes = LocalizedStrings.loading_settings;

        Debug.Log("------------Settings data loading");

        ModalInfoPanel.Instance.Add(mes);
        Action<string, object, bool> onLoad = null;
        onLoad = (s, p, success) =>
        {
            if (s == this.GetType().Name)
            {
                Debug.Log("------------Settings saved data was read");
                
                try
                {
                    ApplyLoadedData(p);
                }
                catch(Exception ex)
                {
                    Debug.LogError("------------Settings data applying failed + \r\n" + ex.ToString());
                }
                ResetTempValues();
                ModalInfoPanel.Instance.Remove(mes);
                if (OnLoaded != null)
                    OnLoaded();
                GameManager.Instance.SavingManager.OnDataLoaded -= onLoad;
            }

            Debug.Log("------------Settings data loaded: " + success);
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
