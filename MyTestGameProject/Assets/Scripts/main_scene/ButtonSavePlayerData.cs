using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSavePlayerData : MonoBehaviour
{
    public void AllpySettingsTempValuesAndSaveSettings()
    {
        AllpySettingsTempValues();
        SaveSettings();
    }

    public void ResetSettingsTempValuesAndSaveSettings()
    {
        ResetSettingsTempValues();
        SaveSettings();
    }

    public void AllpySettingsTempValues()
    {
        GameManager.Instance.SavablePlayerData.Settings.ApplyTempValues();
    }

    public void ResetSettingsTempValues()
    {
        GameManager.Instance.SavablePlayerData.Settings.ResetTempValues();
    }

    public void SaveProgress()
    {
        GameManager.Instance.SavablePlayerData.PlayerProgress.Save();
    }

    public void SaveSettings()
    {
        GameManager.Instance.SavablePlayerData.Settings.Save();
    }

    public void SaveAll()
    {
        GameManager.Instance.SavablePlayerData.Save();
    }
}


