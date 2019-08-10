using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] Slider slider;
    public float Value { get { return slider.value; } set { slider.value = value; } }
    public event Action<float> OnEndEditValue;
    public event Action<float> OnValueChanged;

    [SerializeField] AudioSource uiEffect;
    [SerializeField] float uiEffectDuration = 0.2f;
    Coroutine uiEffCor = null;

    private void Awake()
    {
        if(slider == null)
            slider = GetComponent<Slider>();
    }

    void Start ()
    {
        slider.onValueChanged.AddListener(OnValChanged);
	}

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    void OnValChanged(float newVal)
    {
        //SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.SoundClipsContainer.UI.SliderValChanged), SoundManager.SoundType.UI);

        if(uiEffect != null)
        {
            if (uiEffCor != null)
                StopCoroutine(uiEffCor);
            else
                uiEffect.Play();
            uiEffCor = StartCoroutine(UiEffect());
        }


        if (OnValueChanged != null)
            OnValueChanged(newVal);
    }

    IEnumerator UiEffect()
    {
        yield return new WaitForSecondsRealtime(uiEffectDuration);
        uiEffect.Stop();
        uiEffCor = null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(OnEndEditValue != null)
            OnEndEditValue(slider.value);
    }
}
