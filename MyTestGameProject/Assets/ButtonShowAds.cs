﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonShowAds : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickBtn);
    }
    void OnClickBtn()
    {
        if(!IAPWrapper.IsOlreadyBought(IAPWrapper.Const.NonConsumable.ID_DISABLE_ADS))
            GADWrapper.ShowInterstitialAd(GADWrapper.Const.InterstitialAds.ID_ALL_KIND_OF_INTERSTITIAL_ADS, false, true);
    }
}
