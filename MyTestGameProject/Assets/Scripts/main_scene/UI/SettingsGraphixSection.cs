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

    void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Settings != null && GameManager.Instance.Settings.graphixSettings != null)
            GameManager.Instance.Settings.graphixSettings.Save();
    }

    void Refresh()
    {
        showDamageToggle.isOn = GameManager.Instance.Settings.graphixSettings.ShowDamage;
        allyOutlineToggle.isOn = GameManager.Instance.Settings.graphixSettings.AllyOutline;
        enemyOutlineToggle.isOn = GameManager.Instance.Settings.graphixSettings.EnemyOutline;
        neutralOutlineToggle.isOn = GameManager.Instance.Settings.graphixSettings.NeutralOutline;

        //так не переключает бл!!!!
        //borderOutline.isOn = GameManager.Instance.Settings.graphixSettings.OutlineType == GraphixSettings.OutlineTypes.BORDER;
        //underlayerOutline.isOn = GameManager.Instance.Settings.graphixSettings.OutlineType == GraphixSettings.OutlineTypes.UNDERLAYER;

        //а так переключает
        var b1 = GameManager.Instance.Settings.graphixSettings.OutlineType == GraphixSettings.OutlineTypes.BORDER;
        var b2 = GameManager.Instance.Settings.graphixSettings.OutlineType == GraphixSettings.OutlineTypes.UNDERLAYER;
        borderOutline.isOn = b1;
        underlayerOutline.isOn = b2;
    }

    public void OnBorderOutlineToggleValChanged(bool value)
    {
        if(value)
            GameManager.Instance.Settings.graphixSettings.OutlineType = GraphixSettings.OutlineTypes.BORDER;
        else
            GameManager.Instance.Settings.graphixSettings.OutlineType = GraphixSettings.OutlineTypes.UNDERLAYER;
    }

    public void OnShowDamageToggleValChanged(bool value)
    {
        GameManager.Instance.Settings.graphixSettings.ShowDamage = value;
    }

    public void OnAllyOutlineToggleValChanged(bool value)
    {
        GameManager.Instance.Settings.graphixSettings.AllyOutline = value;
    }

    public void OnEnemyOutlineToggleValChanged(bool value)
    {
        GameManager.Instance.Settings.graphixSettings.EnemyOutline = value;
    }

    public void OnNeutralOutlineToggleValChanged(bool value)
    {
        GameManager.Instance.Settings.graphixSettings.NeutralOutline = value;
    }

    void OnDefaultSetingsButtonClick()
    {
        GameManager.Instance.Settings.graphixSettings.Reset();
        Refresh();
    }
}
