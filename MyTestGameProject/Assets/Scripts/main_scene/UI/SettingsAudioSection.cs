using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsAudioSection : MonoBehaviour
{
    [SerializeField] VolumeSlider sliderMusicVolume;
    [SerializeField] VolumeSlider sliderFxVolume;
    [SerializeField] VolumeSlider sliderUIVolume;
    [Space]
    [SerializeField] VolumeSlider sliderGeneralVolume;
    [Space]
    [SerializeField] Button reset;

    void Start ()
    {
        sliderMusicVolume.OnValueChanged += SliderMusicVolume_OnEndEditValue;
        sliderFxVolume.OnValueChanged += SliderFxVolume_OnEndEditValue;
        sliderUIVolume.OnValueChanged += SliderUIVolume_OnEndEditValue;

        sliderGeneralVolume.OnValueChanged += SliderGeneralVolume_OnEndEditValue;

        reset.onClick.AddListener(OnDefaultSetingsButtonClick);
    }
    
    void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        var aset = GameManager.Instance.SavablePlayerData.Settings.audioSettings;

        sliderMusicVolume.Value = aset.musicVolume.Value;
        sliderFxVolume.Value = aset.fxVolume.Value;
        sliderUIVolume.Value = aset.uiVolume.Value;
        sliderGeneralVolume.Value = aset.generalVolume.Value;
    }

    private void SliderMusicVolume_OnEndEditValue(float volume)
    {
        GameManager.Instance.SavablePlayerData.Settings.audioSettings.musicVolume.Value = volume;
    }

    private void SliderFxVolume_OnEndEditValue(float volume)
    {
        GameManager.Instance.SavablePlayerData.Settings.audioSettings.fxVolume.Value = volume;
    }

    private void SliderUIVolume_OnEndEditValue(float volume)
    {
        GameManager.Instance.SavablePlayerData.Settings.audioSettings.uiVolume.Value = volume;
    }
        
    private void SliderGeneralVolume_OnEndEditValue(float volume)
    {
        GameManager.Instance.SavablePlayerData.Settings.audioSettings.generalVolume.Value = volume;
    }

    void OnDefaultSetingsButtonClick()
    {
        GameManager.Instance.SavablePlayerData.Settings.audioSettings.Reset();
        Refresh();
    }
}
