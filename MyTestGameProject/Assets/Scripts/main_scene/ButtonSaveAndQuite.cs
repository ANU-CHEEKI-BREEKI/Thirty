using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSaveAndQuite : MonoBehaviour
{
    Button btn;
    [SerializeField] bool needConfirm;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(QuitApp);

        GameManager.Instance.OnBackButtonPressed += QuitApp;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnBackButtonPressed -= QuitApp;
    }

    void QuitApp()
    {
        if (needConfirm)
        {
            DialogBox.Instance
                .SetTitle(LocalizedStrings.save_and_quit_title)
                .SetText(LocalizedStrings.save_and_quit_assert)
                .AddButton(LocalizedStrings.yes, GameManager.Instance.SaveAndQuit)
                .AddCancelButton(LocalizedStrings.no)
                .Show();
        }
        else
            GameManager.Instance.SaveAndQuit();
    }
}
