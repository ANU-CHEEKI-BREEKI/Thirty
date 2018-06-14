using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScreen : MonoBehaviour
{
    CanvasGroup cg;

    public event Action OnFadeOff, OnFadeOn;

    public static FadeScreen Instance { get; private set; }

    public bool FadeOnStartScene { get; set; } = true;

    bool fadeWhenStart = true;

    private void Awake()
    {
        Instance = this;

        cg = GetComponent<CanvasGroup>();
        cg.alpha = 1;
    }

    void Start ()
    {
        if(FadeOnStartScene)
            FateOnStartScene();
    }

    public void FateOnStartScene()
    {
        FadeOff(2f);
    }

    IEnumerator Fade(float duration, bool fade, CanvasGroup cg)
    {
        float time = 0;
        int t = 1;
        if (!fade)
            t *= -1;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            cg.alpha += t * Time.unscaledDeltaTime / duration;

            yield return null;
        }

        if (fade)
        {
            cg.alpha = 1;
            if (OnFadeOn != null) OnFadeOn();
        }
        else
        {
            cg.alpha = 0;
            if (OnFadeOff != null) OnFadeOff();
        }
    }

    public void FadeOff(float duration)
    {
        StartCoroutine(Fade(duration, false, cg));
    }

    public void FadeOn(float duration)
    {
        StartCoroutine(Fade(duration, true, cg));
    }




    public static void FadeOff(CanvasGroup cg, float duration, MonoBehaviour owner, Action OnFadeOff = null)
    {
        owner.StartCoroutine(StaticFade(duration, false, cg, null, OnFadeOff));
    }

    public static void FadeOn(CanvasGroup cg, float duration, MonoBehaviour owner, Action OnFadeOn = null)
    {
        owner.StartCoroutine(StaticFade(duration, true, cg, OnFadeOn, null));
    }

    static IEnumerator StaticFade(float duration, bool fade, CanvasGroup cg, Action OnFadeOn = null, Action OnFadeOff = null)
    {
        float time = 0;
        int t = 1;
        if (!fade)
            t *= -1;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            cg.alpha += t * Time.unscaledDeltaTime / duration;

            yield return null;
        }

        if (fade)
        {
            cg.alpha = 1;
            if (OnFadeOn != null) OnFadeOn();
        }
        else
        {
            cg.alpha = 0;
            if (OnFadeOff != null) OnFadeOff();
        }
    }
}
