using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsBlock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI Value;

    [SerializeField] Color positiveValueColor;
    [SerializeField] Color negativeValueColor;

    public void SetName(string name)
    {
        if (name == null)
            name = string.Empty;

        Name.text = name;
    }

    public void SetValue(string value, bool positive = true)
    {
        if(positive)
            Value.color = positiveValueColor;
        else
            Value.color = negativeValueColor;

        Value.text = value;
    }
}
