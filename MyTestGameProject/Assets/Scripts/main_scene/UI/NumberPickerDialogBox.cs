using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberPickerDialogBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] TMP_InputField inField;
    [SerializeField] Button ok;
    [SerializeField] Button cancel;

    Action<string> onClickOk;

    static public NumberPickerDialogBox Instance { get; private set; }
    StringBuilder b;

    CanvasGroup canvas;

    private void Awake()
    {
        Instance = this;
        canvas = GetComponent<CanvasGroup>();
        b = new StringBuilder();
    }

    private void Start()
    {
        ok.onClick.AddListener(OnClickOk);
        cancel.onClick.AddListener(OnClickCancel);
        Hide();
    }
    
    public void Show(string title, string message, Action<string> onClickOk, InputField.ContentType type)
    {
        this.title.text = title;
        this.message.text = message;
        this.onClickOk = onClickOk;
        inField.contentType = (TMP_InputField.ContentType)type;

        canvas.blocksRaycasts = true;
        canvas.alpha = 1;
    }

    public void Hide()
    {
        inField.text = string.Empty;

        canvas.blocksRaycasts = false;
        canvas.alpha = 0;
    }

    void OnClickOk()
    {
        if (onClickOk != null)
            onClickOk(inField.text);
        Hide();
    }

    void OnClickCancel()
    {
        Hide();
    }
}

