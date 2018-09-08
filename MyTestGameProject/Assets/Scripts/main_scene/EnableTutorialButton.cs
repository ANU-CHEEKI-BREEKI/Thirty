using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(ToAnotherSceneButton))]
public class EnableTutorialButton : MonoBehaviour
{
    Button btn;
    ToAnotherSceneButton b;
    PlayerProgress progres;

    private void Awake()
    {
        btn = GetComponent<Button>();
        b = GetComponent<ToAnotherSceneButton>();
        progres = GameManager.Instance.SavablePlayerData.PlayerProgress;
    }

    private void OnEnable()
    {
        btn.interactable = progres.Flags.AvalaibleTutorialLevel >= b.SceneIndex;
    }
}
