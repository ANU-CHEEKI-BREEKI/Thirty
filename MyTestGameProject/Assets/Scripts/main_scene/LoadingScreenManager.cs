using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public static GameManager.SceneIndex NextLevel { get; set; }

    static LoadingScreenManager instance;

    [SerializeField] float granteeMinimemTimeToLoad = 2;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start ()
    {
        var operation = SceneManager.LoadSceneAsync((int)NextLevel);
        StartCoroutine(Loading(operation));
    }
    
    IEnumerator Loading(AsyncOperation operation)
    {
        float time = 0;
        operation.allowSceneActivation = false;
        var prioryty = Application.backgroundLoadingPriority;
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        var ind = ProgressIndicator.Instance;
        ind.Name = Localization.loading;
        ind.Value = 0;

        while (operation.progress < 0.9f || time < granteeMinimemTimeToLoad)
        {
            yield return null;
            ind.Value = operation.progress;
            time += Time.unscaledDeltaTime;
        }

        yield return null;

        ind.Value = 1;
        Application.backgroundLoadingPriority = prioryty;
        operation.allowSceneActivation = true;
    }
	

}
