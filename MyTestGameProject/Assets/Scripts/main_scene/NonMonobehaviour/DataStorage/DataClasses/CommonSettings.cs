using System;
using UnityEngine;

[Serializable]
public class CommonSettings : ICopyabe
{
    /// <summary>
    /// Язык локализации
    /// </summary>
    public SystemLanguage Language = SystemLanguage.Russian;

    public object Copy()
    {
        return new CommonSettings() { Language = this.Language };
    }

    public void Reset()
    {
        Language = SystemLanguage.Russian;
    }
}