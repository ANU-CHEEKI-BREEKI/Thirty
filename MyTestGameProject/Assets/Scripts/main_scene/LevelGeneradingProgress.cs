using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelGeneradingProgress : MonoBehaviour
{
    [SerializeField]  [Range(0, 0.1f)] float deltaTime = 0.01f;

    Ground gr;
    ProgressIndicator ind;

    private void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 1;
    }

    void Start()
    {
        gr = Ground.Instance;
        ind = ProgressIndicator.Instance;

        StartCoroutine(ShowProgress());
    }
    
    IEnumerator ShowProgress()
    {
        if (ind != null)
        {
            ind.Name = Localization.resoures_loadind;
            gr.OnWorkDone += Foo;

            while (GameManager.Instance.GamePaused)
            {
                ind.Value = gr.Progress;
                yield return new WaitForSecondsRealtime(deltaTime);
            }
            ind.Value = gr.Progress;
            yield return new WaitForSecondsRealtime(deltaTime);
        }

        Destroy(gameObject);        
    }

    void Foo()
    {
        ind.Name = Localization.level_generading; ind.Value = 0;
        gr.OnWorkDone -= Foo;
    }
}
