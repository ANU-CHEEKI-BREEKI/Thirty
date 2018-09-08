using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class AudioSettings : ILoadedDataApplyable, ICopyabe
{
    public CalledValue generalVolume;
    [Space]
    public CalledValue musicVolume;
    public CalledValue fxVolume;
    public CalledValue uiVolume;

    public AudioSettings()
    {
        generalVolume = new CalledValue();
        musicVolume = new CalledValue();
        fxVolume = new CalledValue();
        uiVolume = new CalledValue();

        Reset();
    }

    public void ApplyLoadedData(object data)
    {
        var d = data as AudioSettings;

        generalVolume.Value = d.generalVolume.Value;
        musicVolume.Value = d.musicVolume.Value;
        fxVolume.Value = d.fxVolume.Value;
        uiVolume.Value = d.uiVolume.Value;
    }

    public object Copy()
    {
        var res = new AudioSettings();
        res.generalVolume.Value = this.generalVolume.Value;
        res.musicVolume.Value = this.musicVolume.Value;
        res.fxVolume.Value = this.fxVolume.Value;
        res.uiVolume.Value = this.uiVolume.Value;
        return res;
    }

    public void Reset()
    {
        generalVolume.Value = 1;
        musicVolume.Value = 1;
        fxVolume.Value = 1;
        uiVolume.Value = 1;
    }

    [Serializable]
    public class CalledValue
    {
        public event Action<float> OnValueChanged;

        [SerializeField] float value = 1;
        public float Value
        {
            get { return value; }
            set { this.value = value; if (OnValueChanged != null) OnValueChanged(value); }
        }
    }
}
