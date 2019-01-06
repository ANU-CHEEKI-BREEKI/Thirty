using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCommonSetting : MonoBehaviour
{    
    [Header("Data")]
    [SerializeField] Lang[] AvalableLanguages;
    [Header("UI")]
    [SerializeField] LanguageToggleGUI[] toggles;
    [Space]
    [SerializeField] Button reset;

    [Serializable]
    public class Lang
    {
        [SerializeField] SystemLanguage language;
        public SystemLanguage Language { get { return language; } }
        [SerializeField] string sourceString;
        public string UserLangString
        {
            get
            {
                var res = Localization.GetString(sourceString);
                if (res == LocalizedStrings.missing_string)
                    res = language.ToString();
                return res;
            }
        }
        [SerializeField] Sprite ico;
        public Sprite Ico { get { return ico; } }
    }

    private void Start()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].OnValCh += OnValChanged;
            toggles[i].Text = AvalableLanguages[i].UserLangString;
            toggles[i].Ico = AvalableLanguages[i].Ico;
        }
        reset.onClick.AddListener(OnDefaultSetingsButtonClick);
    }

    void OnEnable()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        foreach (var t in toggles)
            if(t != null)
                t.OnValCh -= OnValChanged;
    }

    void Refresh()
    {
        var len = AvalableLanguages.Where(
            l => l.Language == GameManager.Instance.SavablePlayerData.Settings.commonSettings.Language
        ).First();

        var t = toggles[
            AvalableLanguages.ToList().IndexOf(len)
        ];

        t.Toggle.isOn = true;
    }
    void OnValChanged(Toggle sender, bool newVal)
    {
        if (newVal)
        {
            GameManager.Instance.SavablePlayerData.Settings.commonSettings.Language = AvalableLanguages[sender.transform.GetSiblingIndex()].Language;
            foreach (var tt in toggles)
                if(tt.Toggle != sender)
                    tt.Toggle.isOn = false;
        }
    }

    void OnDefaultSetingsButtonClick()
    {
        GameManager.Instance.SavablePlayerData.Settings.commonSettings.Language = SystemLanguage.Russian;
        Refresh();
    }
}
