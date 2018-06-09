using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToLevel : MonoBehaviour
{
    Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        FadeScreen ds = GameObject.FindWithTag("DarkScreen").GetComponent<FadeScreen>();
        ds.OnFadeOn += GameManager.Instance.LoadNextLevel;

        ds.FadeOn(0.5f);
    }
}
