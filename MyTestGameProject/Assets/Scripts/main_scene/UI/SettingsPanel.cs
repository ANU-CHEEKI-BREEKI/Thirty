﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    //[Header("когда вкл панельку - делаем бэкап настроек")]

    public void RecodrSettings()
    {
        //когда вкл панельку - делаем бэкап настроек
        GameManager.Instance.SavablePlayerData.Settings.RecordSettings();
    }
}