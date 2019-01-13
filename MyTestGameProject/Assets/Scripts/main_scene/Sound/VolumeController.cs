using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VolumeController : MonoBehaviour
{
    [SerializeField] SoundManager.SoundType type;
    [SerializeField] [Range(0f, 1f)] float damper = 1;

    AudioSource aus;
    float volumeGeneral;
    float volumeChanel;

    #region UNITY
    void Awake()
    {
        aus = GetComponent<AudioSource>();
    }

    void Start()
    {
        Subscribe(true);
    }

    void OnDestroy()
    {
        Subscribe(false);
    }

    void OnValidate()
    {
        if(aus != null)
            SetVolume();
    }
    #endregion

    public void ForciblyPlaySound()
    {
        aus.Play();
    }

    void Subscribe(bool positive)
    {
        var ausset = GameManager.Instance.SavablePlayerData.Settings.audioSettings;
        switch (type)
        {
            case SoundManager.SoundType.MUSIC:
                if (positive)
                    ausset.musicVolume.OnValueChanged += ChanelVolume_OnValueChanged;
                else
                    ausset.musicVolume.OnValueChanged -= ChanelVolume_OnValueChanged;
                volumeChanel = ausset.musicVolume.Value;
                break;
            case SoundManager.SoundType.UI:
                if (positive)
                    ausset.uiVolume.OnValueChanged += ChanelVolume_OnValueChanged;
                else
                    ausset.uiVolume.OnValueChanged -= ChanelVolume_OnValueChanged;
                volumeChanel = ausset.uiVolume.Value;
                break;
            case SoundManager.SoundType.FX:
                if (positive)
                    ausset.fxVolume.OnValueChanged += ChanelVolume_OnValueChanged;
                else
                    ausset.fxVolume.OnValueChanged -= ChanelVolume_OnValueChanged;
                volumeChanel = ausset.fxVolume.Value;
                break;
        }
        if(positive)
            ausset.generalVolume.OnValueChanged += GeneralVolume_OnValueChanged;
        else
            ausset.generalVolume.OnValueChanged -= GeneralVolume_OnValueChanged;
        volumeGeneral = ausset.generalVolume.Value;

        var sm = GameManager.Instance.SavingManager;
        if(positive)
            sm.OnDataLoaded += Sm_OnDataLoaded;
        else
            sm.OnDataLoaded -= Sm_OnDataLoaded;
        SetVolume();
    }

    void Sm_OnDataLoaded(string arg1, object arg2, bool arg3)
    {
        SetVolume();
    }

    void ChanelVolume_OnValueChanged(float newVal)
    {
        volumeChanel = newVal;
        SetVolume();
    }

    void GeneralVolume_OnValueChanged(float newVal)
    {
        volumeGeneral = newVal;
        SetVolume();
    }

    void SetVolume()
    {
        aus.volume = volumeGeneral * volumeChanel * damper;
    }
}
