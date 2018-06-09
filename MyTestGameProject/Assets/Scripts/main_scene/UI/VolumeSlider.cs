using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] Slider slider;
    public float Value { get { return slider.value; } set { slider.value = value; } }
    public event Action<float> OnEndEditValue;
    public event Action<float> OnValueChanged;

    private void Awake()
    {
        if(slider == null)
            slider = GetComponent<Slider>();
    }

    void Start ()
    {
        slider.onValueChanged.AddListener(OnValChanged);
	}

    void OnValChanged(float newVal)
    {
        if (OnValueChanged != null)
            OnValueChanged(newVal);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(OnEndEditValue != null)
            OnEndEditValue(slider.value);
    }
}
