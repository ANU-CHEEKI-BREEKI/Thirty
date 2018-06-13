using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NumberConverter))]
public class ScoreMarket : MonoBehaviour
{
    enum ConvertType { SILVER, EXPIRIENCE }

    [SerializeField] ConvertType convertFor;

    NumberConverter converter;

    private void Start()
    {
        converter = GetComponent<NumberConverter>();
        converter.OnCliclOk += Converter_OnCliclOk;
        GameManager.Instance.PlayerProgress.Score.gold.OnValueChanged += Gold_OnValueChanged;
        converter.SetMaxValues(0, GameManager.Instance.PlayerProgress.Score.gold.Value);
    }

    private void OnDestroy()
    {
        GameManager.Instance.PlayerProgress.Score.gold.OnValueChanged -= Gold_OnValueChanged;
        if(converter != null)
            converter.OnCliclOk -= Converter_OnCliclOk;
    }

    private void Gold_OnValueChanged(float oldVal, float newVal, DSPlayerScore.Score sender)
    {
        converter.SetMaxValues(0, newVal);
    }

    private void Converter_OnCliclOk()
    {
        float inputValue = converter.InputValue;
        float outputValue = converter.OutputValue;

        switch (convertFor)
        {
            case ConvertType.SILVER:
                GameManager.Instance.PlayerProgress.Score.silver.Value += outputValue;
                break;
            case ConvertType.EXPIRIENCE:
                GameManager.Instance.PlayerProgress.Score.expirience.Value += outputValue;
                break;
        }
        GameManager.Instance.PlayerProgress.Score.gold.Value -= inputValue;
        GameManager.Instance.PlayerProgress.Score.Save();
    }
}
