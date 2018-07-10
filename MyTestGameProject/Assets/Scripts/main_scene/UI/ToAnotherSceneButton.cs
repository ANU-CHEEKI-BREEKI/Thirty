using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToAnotherSceneButton : MonoBehaviour
{
    Button btn;
    [SerializeField] bool needConfirm;
    [SerializeField] GameManager.SceneIndex scene;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (needConfirm)
        {
            DialogBox.Instance
                .SetTitle("[non localice] вы уверены?")
                .SetPrefButtonHeight(80)
                .SetText("[non localice] точто точно???")
                .AddCancelButton(LocalizedStrings.no)
                .AddButton(LocalizedStrings.yes, FadeOn)
                .Show();
        }
        else
        {
            FadeOn();
        }
    }

    void FadeOn()
    {
        if (FadeScreen.Instance != null)
        {
            FadeScreen.Instance.OnFadeOn += LoadLevel;
            FadeScreen.Instance.FadeOn(0.5f);
        }
        else
            LoadLevel();
    }

    void LoadLevel()
    {
        switch (scene)
        {
            case GameManager.SceneIndex.MAIN_MENU:
                GameManager.Instance.LoadMainMenu();
                break;
            case GameManager.SceneIndex.MARKET:
                GameManager.Instance.LoadMarket();
                break;
            case GameManager.SceneIndex.LEVEL:
                GameManager.Instance.LoadNextLevel();
                break;
            case GameManager.SceneIndex.LOADING_SCREEN:
                Debug.Log("Не ну это перебор уже. Сюда незя пепеходить напрямую!");
                break;
            case GameManager.SceneIndex.LEVEL_TUTORIAL_1:
                GameManager.Instance.LoadTutorialLevel();
                break;
        }
    }
}
