using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenLogo : MonoBehaviour
{
    static public StartScreenLogo Instance { get; private set; }
    
    public bool LogoEnded { get; private set; }

    public event Action OnLogoEnded = () => { };

    private void Awake()
    {
        Instance = this;
    }

    public void _StartScreenLogoAnimationEnd()
    {
        transform.gameObject.SetActive(false);
        LogoEnded = true;
        OnLogoEnded.Invoke();
    }
}
