using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugCunvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    [SerializeField] TextMeshProUGUI fpsScaledText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI wholeLevelText;
    [SerializeField] TextMeshProUGUI timescaleText;

    [SerializeField] TextMeshProUGUI savingManagerText;

    static DebugCunvas instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Init();

        DontDestroyOnLoad(gameObject);

        savingManagerText.text = GameManager.Instance.SavingManager.GetType().ToString();
    }

    void Init()
    {
        StartCoroutine(Coroutine(
            condition: () => { return true; },
            action: () => { fpsText.text = "fps:    " + (1f / Time.unscaledDeltaTime).ToString(StringFormats.floatNumber); },
            cleanup: () => { },
            deltaTime: 0.1f,
            type: CoroutineType.REAL_TIME
        ));

        StartCoroutine(Coroutine(
            condition: () => { return true; },
            action: () => { fpsScaledText.text = "ground:    " + GameManager.Instance.CurrentLevel.GroundType.ToString(); },
            cleanup: () => { },
            deltaTime: 0.1f,
            type: CoroutineType.REAL_TIME
        ));

        StartCoroutine(Coroutine(
            condition: () => { return true; },
            action: () => { levelText.text = "level:    " + GameManager.Instance.CurrentLevel.Level.ToString(StringFormats.intNumber); },
            cleanup: () => { },
            deltaTime: 1f,
            type: CoroutineType.REAL_TIME
        ));

        StartCoroutine(Coroutine(
           condition: () => { return true; },
           action: () => { wholeLevelText.text = "whole level:    " + GameManager.Instance.CurrentLevel.WholeLevel.ToString(StringFormats.intNumber); },
           cleanup: () => { },
           deltaTime: 1f,
           type: CoroutineType.REAL_TIME
       ));

        StartCoroutine(Coroutine(
            condition: () => { return true; },
            action: () => { timescaleText.text = "timescale:    " + Time.timeScale.ToString(StringFormats.floatNumber); },
            cleanup: () => { },
            deltaTime: 0.1f,
            type: CoroutineType.REAL_TIME
        ));
    }

    public enum CoroutineType { SCALED_TIME, REAL_TIME }

    IEnumerator Coroutine(Func<bool> condition, Action action, Action cleanup, float deltaTime, CoroutineType type)
    {
        while (condition())
        {
            action();

            if (type == CoroutineType.REAL_TIME)
                yield return new WaitForSecondsRealtime(deltaTime);
            else if (type == CoroutineType.SCALED_TIME)
                yield return new WaitForSeconds(deltaTime);
        }
        cleanup();
    }
}
