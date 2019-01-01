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
    public GameManager.SceneIndex SceneIndex { get { return scene; } }

    //КОСТЫЛЬ ЕБАННЫЙ
    [SerializeField] string commandToGamaManager;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        bool confirm = needConfirm;
        if (needConfirm && commandToGamaManager == "new_game")
            confirm = !GameManager.Instance.SavablePlayerData.PlayerProgress.Squad.IsEmpty;

        if (confirm)
        {
            string title;
            string mes;

            GetStrings(out title, out mes);

            DialogBox.Instance
                .SetTitle(title)
                .SetPrefButtonHeight(80)
                .SetText(mes)
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

    void GetStrings(out string title, out string mes)
    {
        title = LocalizedStrings.are_you_sure;
        mes = LocalizedStrings.assert_choise;

        switch (scene)
        {
            case GameManager.SceneIndex.MAIN_MENU:
                title = LocalizedStrings.quit_to_main_menu_title;
                mes = LocalizedStrings.quit_to_main_menu_assert;
                break;
            case GameManager.SceneIndex.MARKET:
                if (commandToGamaManager == "new_game")
                {
                    title = LocalizedStrings.start_new_game_title;
                    mes = LocalizedStrings.start_new_game_assert;
                }
                break;
        }
    }

    void LoadLevel()
    {
        GameManager.Instance.command = commandToGamaManager;

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
            case GameManager.SceneIndex.LEVEL_TUTORIAL_2:
            case GameManager.SceneIndex.LEVEL_TUTORIAL_3:        
                GameManager.Instance.LoadTutorialLevel(scene);
                break;
            default:
                GameManager.Instance.LoadTestingLevel(scene);
                break;
        }
    }
}
