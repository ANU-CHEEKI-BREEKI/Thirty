using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ModalInfoPanel : MonoBehaviour
{
    static public ModalInfoPanel Instance { get; private set; }

    [SerializeField] TextMeshProUGUI text;

    List<string> messages = new List<string>();
    StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        Instance = this;
    }

    public void Add(string message)
    {
        messages.Add(message);
        SetText();
        Show();
    }

    public void Remove(string message)
    {
        messages.RemoveAll((m) => { return m == message; });

        if (messages.Count == 0)
        {
            Hide();
        }
        else
        {
            SetText();
        }
    }

    void SetText()
    {
        sb.Clear();
        foreach (var item in messages)
            sb.Append(item + "\r\n");

        text.text = sb.ToString();
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
