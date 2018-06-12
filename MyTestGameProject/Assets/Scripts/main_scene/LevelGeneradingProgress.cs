using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelGeneradingProgress : MonoBehaviour
{
    [SerializeField]  [Range(0, 0.1f)] float deltaTime = 0.01f;

    private void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 1;
    }

    void Start()
    {
        StartCoroutine(ShowProgress());
    }
    
    IEnumerator ShowProgress()
    {
        var gr = Ground.Instance;
        var ind = ProgressIndicator.Instance;

        if (ind != null)
        {
            ind.Name = Localization.resoures_loadind;
            while (!gr.LoadingIsDone)
            {
                ind.Value = gr.LoadingProgress;
                yield return new WaitForSecondsRealtime(deltaTime);
            }
            ind.Value = gr.LoadingProgress;
            yield return new WaitForSecondsRealtime(deltaTime);

            ind.Name = Localization.level_generading;
            while (!gr.GenerationIsDone)
            {
                ind.Value = gr.GenerationProgress;
                yield return new WaitForSecondsRealtime(deltaTime);
            }
            ind.Value = gr.GenerationProgress;
            yield return new WaitForSecondsRealtime(deltaTime);
        }

        Destroy(gameObject);
    }
}
