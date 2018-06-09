using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressCircle : MonoBehaviour
{
    [Range(0, 0.1f)] public float deltaTime = 0.1f;
    [Range(0, 5)] public float timeToFill = 1f;

    [SerializeField] Image circle;
    [SerializeField] Text progressText;

    int t = 1;

    private void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 1;

        circle.fillAmount = 0;
    }

    void Start()
    {
        StartCoroutine(Running());
    }
    
    IEnumerator Running()
    {
        float time = 0;
        float val = 1 / timeToFill * deltaTime;

        while (!Ground.Instance.GenerationIsDone)
        {
            float progress = Ground.Instance.Progress;
            circle.fillAmount = progress;
            progressText.text = progress.ToString(StringFormats.floatNumberPercent);

            time = Time.realtimeSinceStartup + deltaTime;
            while (Time.realtimeSinceStartup < time)
                yield return null;
        }

        circle.fillAmount = 1;
        progressText.text = 1.ToString(StringFormats.floatNumberPercent);

        time = Time.realtimeSinceStartup + deltaTime;
        while (Time.realtimeSinceStartup < time)
            yield return null;

        Destroy(gameObject);
    }
}
