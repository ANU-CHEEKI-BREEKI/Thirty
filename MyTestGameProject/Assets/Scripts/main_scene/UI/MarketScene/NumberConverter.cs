using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberConverter : MonoBehaviour
{
    const string format = "# ### ### ### ##0";

    [Header("UI")]
    [SerializeField] TextMeshProUGUI textValueConfertFrom;
    [SerializeField] TextMeshProUGUI textValueConvertTo;
    [SerializeField] TextMeshProUGUI textCurrentValueSelected;
    [SerializeField] TextMeshProUGUI textCurrentValueConvertedSelected;
    [SerializeField] Slider slider;
    [SerializeField] Button btnOk;
    [Space]
    [SerializeField] AudioSource uiEffect;
    [SerializeField] float uiEffectDuration = 0.2f;
    Coroutine uiEffCor = null;

    [Header("Script")]
    [SerializeField] int valueConvertFrom;
    [SerializeField] int valueConvertTo;
    

    float k;

    float convertedValue;

    public event Action OnCliclOk;

    public float InputValue
    {
        get { return slider.value; }
        set
        {
            if (value > slider.maxValue)
                slider.maxValue = value;
            else if(value < slider.minValue)
                slider.minValue = value;
            slider.value = value;
        }
    }

    public float OutputValue
    {
        get { return convertedValue; }
        set
        {            
            float t = value / k;
            if ((int)t != t) t = (int)t + 1;
            if (t > slider.maxValue)
                slider.maxValue = t;
            else if (t < slider.minValue)
                slider.minValue = t;
            slider.value = t;
        }
    }

    void Awake ()
    {
        btnOk.onClick.AddListener(OnApply);
        slider.onValueChanged.AddListener(OnSliderValChanged);
        slider.wholeNumbers = true;

        k = (float)valueConvertTo / valueConvertFrom;

        textValueConfertFrom.text = valueConvertFrom.ToString(StringFormats.intNumber);
        textValueConvertTo.text = valueConvertTo.ToString(StringFormats.intNumber);
    }

    void OnApply()
    {
        if (OnCliclOk != null)
            OnCliclOk();
    }

    void OnSliderValChanged(float val)
    {
        if (uiEffect != null)
        {
            if (uiEffCor != null)
                StopCoroutine(uiEffCor);
            else
                uiEffect.Play();
            uiEffCor = StartCoroutine(UiEffect());
        }

        int ost = (int)val % valueConvertFrom;
        if (ost != 0)
        {
            slider.value = val - ost;
            return;
        }

        convertedValue = Convert(val);
        textCurrentValueSelected.text = val.ToString(format);
        textCurrentValueConvertedSelected.text = convertedValue.ToString(format);
    }

    IEnumerator UiEffect()
    {
        yield return new WaitForSecondsRealtime(uiEffectDuration);
        uiEffect.Stop();
        uiEffCor = null;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();

    }
    float Convert(float val)
    {
        return val * k;
    }

    public void SetMaxValues(float minValue, float maxValue)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = 0;

        textCurrentValueSelected.text = slider.value.ToString(format);
        textCurrentValueConvertedSelected.text = convertedValue.ToString(format);
    }
}
