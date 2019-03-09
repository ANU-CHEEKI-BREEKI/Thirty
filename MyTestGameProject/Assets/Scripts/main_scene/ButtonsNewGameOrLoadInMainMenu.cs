using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsNewGameOrLoadInMainMenu : MonoBehaviour
{
    [SerializeField] GameObject[] buttonContinueGame;

    private void Awake()
    {
        var progres = GameManager.Instance.SavablePlayerData.PlayerProgress;

        foreach (var item in buttonContinueGame)
            if(item != null)
                item.SetActive(progres.Squad.IsEmpty == false);

        progres.OnLoaded += Progres_OnLoaded;
    }

    private void Progres_OnLoaded()
    {
        var progres = GameManager.Instance.SavablePlayerData.PlayerProgress;
        progres.OnLoaded -= Progres_OnLoaded;

        foreach (var item in buttonContinueGame)
            if (item != null)
                item.SetActive(progres.Squad.IsEmpty == false);
    }

    private void OnDestroy()
    {
        var progres = GameManager.Instance.SavablePlayerData.PlayerProgress;
        progres.OnLoaded -= Progres_OnLoaded;
    }
}
