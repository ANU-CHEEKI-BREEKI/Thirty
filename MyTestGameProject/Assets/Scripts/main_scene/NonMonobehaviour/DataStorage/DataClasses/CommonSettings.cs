using System;
using UnityEngine;

[Serializable]
public class CommonSettings : ICopyabe
{
    /// <summary>
    /// Язык локализации
    /// </summary>
    public SystemLanguage Language { get; set; } = SystemLanguage.English;

    public event Action<SystemLanguage> OnLanguageChanged;

    public object Copy()
    {
        return new CommonSettings() { Language = this.Language };
    }

    public void Reset()
    {
        Language = SystemLanguage.Russian;
    }
}