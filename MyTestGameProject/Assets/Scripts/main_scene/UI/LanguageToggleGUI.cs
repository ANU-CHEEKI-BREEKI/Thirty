using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
[ExecuteInEditMode]
public class LanguageToggleGUI : MonoBehaviour
{
    [SerializeField] Sprite ico;
    [SerializeField] Sprite maskImage;
    [Space]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] Image mask;

    public Toggle Toggle { get; private set; }
    public Sprite Ico
    {
        set
        {
            ico = value;
            if (image != null)
                image.sprite = value;
        }
    }


    public event Action<Toggle, bool> OnValCh = (t, b) => { };

    public string Text
    {
        set
        {
            if(text != null)
                text.text = value;
        }
    }

    private void OnEnable()
    {
        if (Toggle == null)
        {
            Toggle = GetComponent<Toggle>();
            Toggle.onValueChanged.AddListener(ValChanged);
        }
    }

    private void OnDisable()
    {
        if (Toggle != null)
            Toggle.onValueChanged.RemoveListener(ValChanged);
    }

    private void ValChanged(bool val)
    {
        OnValCh.Invoke(Toggle, val);
    }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        if (image != null)
            image.sprite = ico;
        if (mask != null)
            mask.sprite = maskImage;
    }

#if UNITY_EDITOR

    void Update()
    {
        Init();   
    }

#endif

}
