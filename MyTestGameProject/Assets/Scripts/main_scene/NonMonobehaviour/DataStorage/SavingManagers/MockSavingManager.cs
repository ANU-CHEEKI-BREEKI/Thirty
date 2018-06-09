using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockSavingManager// : ISavingManager
{
    Settings Data = new Settings();
    PlayerProgress Progress = new PlayerProgress();

    public Settings Settings
    {
        get
        {
            return Data;
        }

        set
        {
            Data = value;
        }
    }

    public PlayerProgress PlayerProgress
    {
        get
        {
            return Progress;
        }

        set
        {
            Progress = value;
        }
    }
}
