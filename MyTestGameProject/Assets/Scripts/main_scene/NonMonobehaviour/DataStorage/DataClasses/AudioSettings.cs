using System;
using UnityEngine;

[Serializable]
public class AudioSettings : ISavable, IResetable
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

    public void Save()
    {
        GameManager.Instance.SavingManager.SaveData<AudioSettings>(this.GetType().Name, this);
    }

    public void Load()
    {
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
        var g = GameManager.Instance.SavingManager.LoadData<AudioSettings>(this.GetType().Name);
        var fields = this.GetType().GetFields(flags);
        foreach (var f in fields)
            f.SetValue(this, f.GetValue(g));
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
