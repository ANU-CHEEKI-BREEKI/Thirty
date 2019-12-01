using System;
using UnityEngine;

[Serializable]
public class CommonSettings : ICopyabe
{
    [SerializeField] private SystemLanguage language = SystemLanguage.English;
    /// <summary> 
    /// Язык локализации
    /// </summary>
    public SystemLanguage Language { get => language; set { language = value; } }

    public object Copy()
    {
        return new CommonSettings() { Language = this.Language };
    }

    public void Reset()
    {
        Language = SystemLanguage.English;
    }
}