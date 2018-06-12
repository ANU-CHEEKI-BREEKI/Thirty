using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI valueText;

    public float Value
    {
        get
        {
            return image.fillAmount;
        }

        set
        {
            image.fillAmount = value;
            valueText.text = value.ToString(StringFormats.intNumberPercent);
        }
    }

    public string Name
    {
        get
        {
            return nameText.text;
        }

        set
        {
            nameText.text = value;
        }
    }

    static public ProgressIndicator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
