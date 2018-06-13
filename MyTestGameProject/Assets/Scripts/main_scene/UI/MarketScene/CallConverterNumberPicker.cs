using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CallConverterNumberPicker : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Type type;
    [SerializeField] NumberConverter converter;

    enum Type { INPUT_VALUE, OUTPUT_VALUE} 

    public void OnPointerClick(PointerEventData eventData)
    {
        Action<string> act = null;

        switch (type)
        {
            case Type.INPUT_VALUE:
                act = (val) => 
                {
                    float value = float.Parse(val);
                    //слайдер хранит в инте, походу, поэтому, проверяем на инт
                    if (value > int.MaxValue)
                        value = int.MaxValue;
                    else if (value < 0)
                        value = 0;

                    //нужно не только голду проверять!!! хилка то на серебре!

                    if (value > GameManager.Instance.PlayerProgress.Score.gold.Value)
                    {
                        value = GameManager.Instance.PlayerProgress.Score.gold.Value;
                        converter.SetMaxValues(0, value);
                    }
                    converter.InputValue = value;
                }; 
                break;
            case Type.OUTPUT_VALUE:
                act = (val) => 
                {
                    float value = float.Parse(val);
                    //слайдер хранит в инте, походу, поэтому, проверяем на инт
                    if (value > int.MaxValue)
                        value = int.MaxValue;
                    else if (value < 0)
                        value = 0;

                    converter.OutputValue = value;
                    value = converter.InputValue;
                    if (value > GameManager.Instance.PlayerProgress.Score.gold.Value)
                    {
                        value = GameManager.Instance.PlayerProgress.Score.gold.Value;
                        converter.SetMaxValues(0, value);
                    }
                    converter.InputValue = value;

                };
                break;
        }

        NumberPickerDialogBox.Instance.Show(
            "Введите желаемую суму",
            string.Empty,
            act,
            InputField.ContentType.IntegerNumber
        );
    }
}
