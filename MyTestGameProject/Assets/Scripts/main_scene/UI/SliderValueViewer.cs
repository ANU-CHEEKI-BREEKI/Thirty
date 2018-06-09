using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SliderValueViewer : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] Slider slider;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        slider.onValueChanged.AddListener(OnValChanged);
    }

    void OnEnable()
    {
        OnValChanged(slider.value);
    }

    void OnValChanged(float newVal)
    {
        text.text = newVal.ToString(StringFormats.floatNumber);
    }
}
