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
    [SerializeField] Toggle borderOutline;
    [SerializeField] Toggle underlayerOutline;
    [Space]
    [SerializeField] Button reset;

    private void Start()
    {
        Refresh();

        showDamageToggle.onValueChanged.AddListener(OnShowDamageToggleValChanged);
        allyOutlineToggle.onValueChanged.AddListener(OnAllyOutlineToggleValChanged);
        enemyOutlineToggle.onValueChanged.AddListener(OnEnemyOutlineToggleValChanged);
        neutralOutlineToggle.onValueChanged.AddListener(OnNeutralOutlineToggleValChanged);
        borderOutline.onValueChanged.AddListener(OnBorderOutlineToggleValChanged);

        reset.onClick.AddListener(OnDefaultSetingsButtonClick);
    }

    void Refresh()
    {
        showDamageToggle.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.ShowDamage;
        allyOutlineToggle.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.AllyOutline;
        enemyOutlineToggle.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.EnemyOutline;
        neutralOutlineToggle.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.NeutralOutline;

        //так не переключает бл!!!!
        //borderOutline.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.OutlineType == GraphixSettings.OutlineTypes.BORDER;
        //underlayerOutline.isOn = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.OutlineType == GraphixSettings.OutlineTypes.UNDERLAYER;

        //а так переключает
        var b1 = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.OutlineType == GraphixSettings.OutlineTypes.BORDER;
        var b2 = GameManager.Instance.SavablePlayerData.Settings.graphixSettings.OutlineType == GraphixSettings.OutlineTypes.UNDERLAYER;
        borderOutline.isOn = b1;
        underlayerOutline.isOn = b2;
    }

    public void OnBorderOutlineToggleValChanged(bool value)
    {
        if(value)
            GameManager.Instance.SavablePlayerData.Settings.graphixSettings.OutlineType = GraphixSettings.OutlineTypes.BORDER;
        else
            GameManager.Instance.SavablePlayerData.Settings.graphixSettings.OutlineType = GraphixSettings.OutlineTypes.UNDERLAYER;
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
