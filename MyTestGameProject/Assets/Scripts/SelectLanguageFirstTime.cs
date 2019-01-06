using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLanguageFirstTime : MonoBehaviour
{
    static public SelectLanguageFirstTime Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
}
