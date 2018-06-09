using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ConfirmPanel : MonoBehaviour
{
    [SerializeField] Text header;
    [SerializeField] Text info;

    [SerializeField] Button buttonYes;
    [SerializeField] Button buttonNo;

    static public ConfirmPanel Instance { get; private set; }

    CanvasGroup cg;
    Action OnYes;

    private void Awake()
    {
        Instance = this;

        cg = GetComponent<CanvasGroup>();

        cg.alpha = 0;
        cg.blocksRaycasts = false;

        buttonYes.onClick.AddListener(OnPresYes);
        buttonNo.onClick.AddListener(OnPresNo);

        ((RectTransform)transform).anchoredPosition = Vector2.zero;
    }

    void OnPresYes()
    {
        if (OnYes != null)
            OnYes();

        Hide();
    }

    void OnPresNo()
    {
        Hide();
    }

    public void Show(string header, string info, Action onPressYes = null)
    {
        this.header.text = header;
        this.info.text = info;

        OnYes = onPressYes;

        cg.alpha = 1;
        cg.blocksRaycasts = true;
    }

    public void Hide()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }
}
