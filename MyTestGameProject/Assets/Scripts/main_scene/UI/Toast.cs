using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Toast : MonoBehaviour, IPointerClickHandler
{
    public enum ToastLifetime { FAST = 1, MEDIUM = 2, SLOW = 3 }
    
    public static Toast Instance { get; private set; }

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float floatDuration = 0.2f;
    [SerializeField] Vector2 floatShiftInPercent;
    [SerializeField] [Range(0, 1)] float startAlpha = 0.5f;

    CanvasGroup cg;
    new RectTransform transform;

    Coroutine lastCoroutine;

    void Awake()
    {
        Instance = this;
        cg = GetComponent<CanvasGroup>();
        transform = base.transform as RectTransform;

        Hide();
    }

    public void Show(string message, ToastLifetime lifetime = ToastLifetime.MEDIUM)
    {
        Hide();
        text.text = message;
        gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform);

        lastCoroutine = StartCoroutine(Float(floatDuration, (float)lifetime, floatShiftInPercent, startAlpha));
    }

    void Hide()
    {
        if (lastCoroutine != null)
            StopCoroutine(lastCoroutine);
        gameObject.SetActive(false);
    }
    
    IEnumerator Float(float floatDuration, float lifetime, Vector2 floatShiftInPercent, float startAlpha)
    {
        transform.anchoredPosition = Vector2.zero;
        cg.alpha = startAlpha;
        Vector2 shift = new Vector2(floatShiftInPercent.x * Camera.main.pixelWidth, floatShiftInPercent.y * Camera.main.pixelHeight);
        float deltaAlpha = 1 - startAlpha;

        float duration = floatDuration;

        float delta;
        while (floatDuration > 0 || lifetime > 0)
        {
            delta = Time.deltaTime;
            floatDuration -= delta;
            lifetime -= delta;

            if(floatDuration > 0)
            {
                transform.anchoredPosition += shift * delta / duration;
                cg.alpha += deltaAlpha * delta;
            }
            else
            {
                transform.anchoredPosition = shift;
                cg.alpha = 1;
            }

            if (lifetime < 0)
                break;

            yield return null;
        }

        Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Hide();
    }
}
