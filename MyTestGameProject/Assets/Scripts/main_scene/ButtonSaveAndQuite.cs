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
    }

    void QuitApp()
    {
        if (needConfirm)
        {
            DialogBox.Instance
                .SetTitle("[non loc] Сохранить и выйти?")
                .SetText("[non loc] Вы уверены что хотите сохранить и выйти?")
                .AddButton(LocalizedStrings.yes, GameManager.Instance.SaveAndQuit)
                .AddCancelButton(LocalizedStrings.no)
                .Show();
        }
        else
            GameManager.Instance.SaveAndQuit();
    }
}
