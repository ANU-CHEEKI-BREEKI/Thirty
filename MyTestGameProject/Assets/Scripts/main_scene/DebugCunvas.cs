using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugCunvas : MonoBehaviour
{
    [SerializeField] private bool update = true;

    [SerializeField] TextMeshProUGUI fpsText;
    [SerializeField] TextMeshProUGUI fpsScaledText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI wholeLevelText;
    [SerializeField] TextMeshProUGUI timescaleText;
    [Space]
    [SerializeField] TextMeshProUGUI savingManagerText;
    [Space]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI scene;

    static DebugCunvas instance;



    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (update)
            Init();

        DontDestroyOnLoad(gameObject);
    }

    void Init()
    {
        if(fpsText != null)
        StartCoroutine(Coroutine(
            condition: () => { return true; },
            action: () => { fpsText.text = "fps: " + (1f / Time.unscaledDeltaTime).ToString(StringFormats.floatNumber); },
            cleanup: () => { },
            deltaTime: 0.5f,
            type: CoroutineType.REAL_TIME
        ));

        if (fpsScaledText != null)
        StartCoroutine(Coroutine(
            condition: () => { return true; },
            action: () => {
                if (fpsScaledText != null)
                    fpsScaledText.text = "ground:    " + GameManager.Instance.CurrentLevel.GroundType.ToString();
            },
            cleanup: () => { },
            deltaTime: 0.1f,
            type: CoroutineType.REAL_TIME
        ));

        if (levelText != null)
        StartCoroutine(Coroutine(
            condition: () => { return true; },
            action: () => {
                if (levelText != null)
                    levelText.text = "level:    " + GameManager.Instance.CurrentLevel.Level.ToString(StringFormats.intNumber);
            },
            cleanup: () => { },
            deltaTime: 1f,
            type: CoroutineType.REAL_TIME
        ));

        if (savingManagerText != null)
        StartCoroutine(Coroutine(
           condition: () => { return true; },
           action: () => 
           {
               wholeLevelText.text = "whole level:    " + GameManager.Instance.CurrentLevel.WholeLevel.ToString(StringFormats.intNumber);
               if (GameManager.Instance.SavingManager != null)
                   savingManagerText.text = GameManager.Instance.SavingManager.GetType().ToString();
           },
           cleanup: () => { },
           deltaTime: 1f,
           type: CoroutineType.REAL_TIME
       ));

        if (timescaleText != null)
        StartCoroutine(Coroutine(
            condition: () => { return true; },
            action: () => {
                if(timescaleText != null) 
                    timescaleText.text = "timescale:    " + Time.timeScale.ToString(StringFormats.floatNumber);
            },
            cleanup: () => { },
            deltaTime: 0.1f,
            type: CoroutineType.REAL_TIME
        ));

        if (text != null)
        StartCoroutine(Coroutine(
           condition: () => { return true; },
           action: () => {
               if (text != null)
                   text.text = "dpi: " + Screen.dpi;
           },
           cleanup: () => { },
           deltaTime: 0.1f,
           type: CoroutineType.REAL_TIME
       ));

        if (scene != null)
        StartCoroutine(Coroutine(
           condition: () => { return true; },
           action: () => {
               if (scene != null)
                   scene.text = "scene: " + (GameManager.SceneIndex)SceneManager.GetActiveScene().buildIndex;
           },
           cleanup: () => { },
           deltaTime: 1f,
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
