using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class McSliderValue : MonoBehaviour {

    public Text text;
    public Slider slider;

    private void Start()
    {
        text.text = slider.value.ToString();
    }

    public void OnSliderValueChanged(float newValue)
    {
        text.text = newValue.ToString();
    }
}
