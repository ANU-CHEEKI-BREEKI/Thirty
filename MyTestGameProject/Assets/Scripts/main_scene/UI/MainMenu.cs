using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{    
    public void OnPlay()
    {
        FadeScreen.Instance.OnFadeOn += GameManager.Instance.LoadNextLevel;
        FadeScreen.Instance.FadeOn(0.5f);
    }
}
