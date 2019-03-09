using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToAnotherSceneButton : MonoBehaviour
{
    Button btn;
    [SerializeField] bool needConfirm;
    [SerializeField] GameManager.SceneIndex scene;
    public GameManager.SceneIndex SceneIndex { get { return scene; } }

    //КОСТЫЛЬ ЕБАННЫЙ
    [SerializeField] string commandToGameManager;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        bool confirm = needConfirm;
        if (needConfirm && commandToGameManager == "new_game")
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
        DialogBox.Instance.Hide();
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
                if (commandToGameManager == "new_game")
                {
                    title = LocalizedStrings.start_new_game_title;
                    mes = LocalizedStrings.start_new_game_assert;
                }
                break;
        }
    }

    void LoadLevel()
    {
        GameManager.Instance.command = commandToGameManager;

        switch (scene)
        {
            case GameManager.SceneIndex.MAIN_MENU:
                GameManager.Instance.LoadMainMenu();
                break;
            case GameManager.SceneIndex.MARKET:
                if (commandToGameManager == "new_game")
                {
                    Action actOnYes = null;
                    Action actOnNo = null;
                    string title = string.Empty;
                    string message = string.Empty;

                    if (!GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.IsTrainingStartedAtLeastOnce.IsOlreadySet &&
                    !GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.IsTrainingStartedAtLeastOnce.Flag)

                    {
                        //тут предлагаем туториал.                        
                        title = "[nl]Похоже, вы запустили игру вперые.";
                        message = "[nl]Похоже, вы запустили игру вперые. Желаете пройти обучение основам?";
                        actOnYes = () =>
                        {
                            DialogBox.Instance.Hide();
                            GameManager.Instance.LoadTutorialLevel(GameManager.SceneIndex.LEVEL_TUTORIAL_1);
                            GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.IsTrainingStartedAtLeastOnce.Flag = true;
                        };
                        actOnNo = () =>
                        {
                            DialogBox.Instance.Hide();
                            GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.IsTrainingStartedAtLeastOnce.Flag = false;
                            LoadLevel();
                        };
                    }
                    else if(!GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.IsTutorialCompleted.IsOlreadySet &&
                        !GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.IsTutorialCompleted.Flag)
                    {
                        //тут предлагаем допройти туториал.                        
                        title = "[nl]Похоже, вы не закончили обучение.";
                        message = "[nl]Похоже, вы не закончили обучение. Желаете допройти обучение основам?";
                        actOnYes = () =>
                        {
                            DialogBox.Instance.Hide();
                            GameManager.Instance.LoadTutorialLevel(GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.AvalaibleTutorialLevel);                            
                        };
                        actOnNo = () =>
                        {
                            DialogBox.Instance.Hide();
                            GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.IsTutorialCompleted.Flag = false;
                            LoadLevel();
                        };
                    }

                    if (actOnYes != null)
                    {
                        DialogBox.Instance
                           .SetTitle(title)
                           .SetPrefButtonHeight(80)
                           .SetText(message)
                           .AddButton(LocalizedStrings.yes, () =>
                           {
                               if (FadeScreen.Instance != null)
                               {
                                   FadeScreen.Instance.OnFadeOn += actOnYes;
                                   FadeScreen.Instance.FadeOn(0.5f);
                               }
                               else
                                   actOnYes.Invoke();
                           })
                           .AddButton(LocalizedStrings.no, () =>
                           {
                               if (FadeScreen.Instance != null)
                               {
                                   FadeScreen.Instance.OnFadeOn += actOnNo;
                                   FadeScreen.Instance.FadeOn(0.5f);
                               }
                               else
                                   actOnNo.Invoke();
                           })
                           .Show();
                    }
                    else
                    {
                        GameManager.Instance.LoadMarket();
                    }
                }
                else
                {
                    GameManager.Instance.LoadMarket();
                }
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
