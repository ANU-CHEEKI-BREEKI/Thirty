using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGraphixSection : MonoBehaviour
{
    [SerializeField] Toggle showDamageToggle;
    [SerializeField] Toggle allyOutlineToggle;
    [SerializeField] Toggle enemyOutlineToggle;
    [SerializeField] Toggle neutralOutlineToggle;
    [Space]
    [SerializeField] Button reset;

    private void Start()
    {
        Refresh();

        showDamageToggle.onValueChanged.AddListener(OnShowDamageToggleValChanged);
        allyOutlineToggle.onValueChanged.AddListener(OnAllyOutlineToggleValChanged);
        enemyOutlineToggle.onValueChanged.AddListener(OnEnemyOutlineToggleValChanged);
        neutralOutlineToggle.onValueChanged.AddListener(OnNeutralOutlineToggleValChanged);

        reset.onClick.AddListener(OnDefaultSetingsButtonClick);
    }

    void Refresh()
    {
        showDamageToggle.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.ShowDamage;
        allyOutlineToggle.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.AllyOutline;
        enemyOutlineToggle.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.EnemyOutline;
        neutralOutlineToggle.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.NeutralOutline;
    }

    public void OnShowDamageToggleValChanged(bool value)
    {
        GameManager.Instance.SavablePlayerData.Settings.graphixSettings.ShowDamage = value;
    }

    public void OnAllyOutlineToggleValChanged(bool value)
    {
        GameManager.Instance.SavablePlayerData.Settings.graphixSettings.AllyOutline = value;
    }

    public void OnEnemyOutlineToggleValChanged(bool value)
    {
        GameManager.Instance.SavablePlayerData.Settings.graphixSettings.EnemyOutline = value;
    }

    public void OnNeutralOutlineToggleValChanged(bool value)
    {
        GameManager.Instance.SavablePlayerData.Settings.graphixSettings.NeutralOutline = value;
    }

    void OnDefaultSetingsButtonClick()
    {
        GameManager.Instance.SavablePlayerData.Settings.graphixSettings.Reset();
        Refresh();
    }
}
