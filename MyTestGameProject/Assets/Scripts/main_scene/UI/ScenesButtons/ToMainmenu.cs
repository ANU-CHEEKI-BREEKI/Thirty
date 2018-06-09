using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToMainmenu : MonoBehaviour
{
    Button btn;
    [SerializeField] bool needConfirm = true;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        GameObject go = GameObject.Find("PauseToggle");
        PauseToggle togle = null;
        if (go != null)
            togle = go.GetComponent<PauseToggle>();

        if (needConfirm)
        {
            DialogBox.Instance
                .SetTitle(Localization.goToMainMenu_header)
                .SetPrefButtonHeight(80)
                .SetText(Localization.goToMainMenu_info)
                .AddCancelButton(Localization.no)
                .AddButton(Localization.yes, FadeOn)
                .Show(togle);
        }
        else
        {
            FadeOn();
        }
    }

    void FadeOn()
    {
        if (!GameManager.Instance.GamePaused)
        {
            FadeScreen ds = GameObject.FindWithTag("DarkScreen").GetComponent<FadeScreen>();
            ds.OnFadeOn += GameManager.Instance.LoadMainMenu;

            ds.FadeOn(0.5f);
        }
        else
            GameManager.Instance.LoadMainMenu();
    }
}
