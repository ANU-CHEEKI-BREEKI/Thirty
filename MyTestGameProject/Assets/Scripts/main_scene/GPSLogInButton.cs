using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSLogInButton : MonoBehaviour
{
    [SerializeField] Image img;
    [Space]
    [SerializeField] Sprite loggedIn;
    [SerializeField] Sprite loggedOut;


    private void Awake()
    {
        if (img == null)
            img = GetComponent<Image>();

        GPSWrapper.OnPlayerLoggedInValueChanged += GPSWrapper_OnPlayerLoggedInValueChanged;
        GPSWrapper_OnPlayerLoggedInValueChanged(GPSWrapper.PlayerLoggedIn);
    }

    private void GPSWrapper_OnPlayerLoggedInValueChanged(bool val)
    {
        if (val)
            img.sprite = loggedIn;
        else
            img.sprite = loggedOut;
    }
}
