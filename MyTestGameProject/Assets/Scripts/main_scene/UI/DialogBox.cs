using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DialogBox : MonoBehaviour, IDialogBox
{
    #region dialogdata;

    class ButtonData
    {
        public string Text { get; set; }
        public Action Action { get; set; }
    }

    class DialogData
    {
        public Sprite Icon { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsTextScrolable { get; set; }
        public List<ButtonData> ButtonsData { get; private set; }
        public ButtonData CancelButtonData { get; set; } = null;

        public Color? TitleColor { get; set; }
        public Color? TextColor { get; set; }
        public float? PrefButtonHeight { get; set; }

        public float? Wifth { get; set; }
        public float? Height { get; set; }

        public DialogData()
        {
            ButtonsData = new List<ButtonData>();
        }

        public void Reset()
        {
            Icon = null;
            Title = LocalizedStrings.title;
            Text = LocalizedStrings.text;
            IsTextScrolable = true;

            ButtonsData.Clear();
            CancelButtonData = null;
        }
    }

    #endregion;
    [SerializeField] GameObject originalButton;
    [Space]
    [SerializeField] TextMeshProUGUI title;
    [Space]
    [SerializeField] GameObject iconContent;
    [SerializeField] Image icon;
    [Space]
    [SerializeField] GameObject scrolableTextContent;
    [SerializeField] TextMeshProUGUI scrolableText;
    [Space]
    [SerializeField] GameObject unscrolableTextContent;
    [SerializeField] TextMeshProUGUI unscrolableText;
    [Space]
    [SerializeField] Transform buttonsPanel;
    [Space]
    [SerializeField] string defaultTitle = "Title";
    [SerializeField] string defaultText = "Text";
    [Space]
    [SerializeField] Color defaultTitleColor;
    [SerializeField] Color defaultTextColor;

    static DialogBox instance;
    static public DialogBox Instance
    {
        get
        {
            if(instance == null)
            {
                var go = Resources.Load<GameObject>(@"Prefabs\DialogBox");
                instance = Instantiate(go, MainCanvas.Instance.transform).GetComponent<DialogBox>();
                instance.transform.SetSiblingIndex(instance.transform.parent.childCount -2);
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    public bool Showned { get; private set; }
    public object Owner { get; private set; }

    DialogData dialogData;

    Vector2 startPosition;
    RectTransform thisTansform;
    CanvasGroup cg;

    private void Awake()
    {
        Instance = this;

        thisTansform = transform as RectTransform;
        startPosition = thisTansform.anchoredPosition;

        cg = GetComponent<CanvasGroup>();

        dialogData = new DialogData();

        Hide();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    void Reset()
    {
        int size = buttonsPanel.childCount;
        for (int i = 0; i < size; i++)
            Destroy(buttonsPanel.GetChild(i).gameObject);

        title.text = defaultTitle;
        scrolableText.text = defaultText;
        unscrolableText.text = defaultText;
        icon.sprite = null;
    }

    public void Show(object owner = null)
    {
        Show(Vector2.zero, owner);
    }

    public void Show(Vector2 positionOnScreen, object owner = null)
    {
        Reset();

        //set title
        {
            if (dialogData.Title != null)
                title.text = dialogData.Title;

            if (dialogData.TitleColor != null)
                title.color = dialogData.TitleColor.Value;
            else
                title.color = defaultTitleColor;
        }

        //set text
        {
            if (dialogData.IsTextScrolable)
            {
                if (dialogData.Text != null)
                    scrolableText.text = dialogData.Text;

                if (dialogData.TextColor != null)
                    scrolableText.color = dialogData.TextColor.Value;
                else
                    scrolableText.color = defaultTextColor;

                //scrolableText.fontSize = title.fontSize;

                unscrolableTextContent.SetActive(false);
                scrolableTextContent.SetActive(true);
            }
            else
            {
                if (dialogData.Text != null)
                    unscrolableText.text = dialogData.Text;

                if (dialogData.TextColor != null)
                    unscrolableText.color = dialogData.TextColor.Value;
                else
                    unscrolableText.color = defaultTextColor;

                //unscrolableText.fontSize = title.fontSize;

                scrolableTextContent.SetActive(false);
                unscrolableTextContent.SetActive(true);
            }
        }

        // set buttons
        {
            foreach (var item in dialogData.ButtonsData)
                AddButton(item);

            if (dialogData.CancelButtonData != null)
                AddButton(dialogData.CancelButtonData);
        }

        //set icon
        {
            if (dialogData.Icon != null)
            {
                icon.sprite = dialogData.Icon;
                iconContent.SetActive(true);
            }
            else
            {
                iconContent.SetActive(false);
            }
        }

        //set size
        {
            Vector2 prefSize = new Vector2(0, 0);

            if (dialogData.Wifth != null)
                prefSize.x = dialogData.Wifth.Value;

            if (dialogData.Height != null)
                prefSize.y = dialogData.Height.Value;
            else
                // set pref size
                ;
            thisTansform.sizeDelta = prefSize;
        }

        //set position
        thisTansform.anchoredPosition = positionOnScreen;

        cg.blocksRaycasts = true;
        cg.alpha = 1;

        Showned = true;
        Owner = owner;

        dialogData = new DialogData();
    }

    void AddButton(ButtonData data)
    {
        var go = Instantiate(originalButton, buttonsPanel);
        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.Text;
        go.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(data.Action));
        if (dialogData.PrefButtonHeight != null)
            go.GetComponent<LayoutElement>().preferredHeight = dialogData.PrefButtonHeight.Value;
    }

    public void Hide()
    {
        thisTansform.anchoredPosition = startPosition;
        cg.blocksRaycasts = false;
        cg.alpha = 0;
        dialogData.Reset();

        Showned = false;
    }

    public IDialogBox SetTitle(string title)
    {
        dialogData.Title = title;
        return this;
    }

    public IDialogBox SetText(string text, bool isScrolable = false)
    {
        dialogData.Text = text;
        dialogData.IsTextScrolable = isScrolable;
        return this;
    }

    public IDialogBox AddButton(string text, Action action)
    {
        dialogData.ButtonsData.Add(new ButtonData() { Text = text, Action = action });
        return this;
    }

    public IDialogBox AddCancelButton(string text)
    {
        dialogData.CancelButtonData = new ButtonData() { Text = text, Action = () => { Hide(); } };
        return this;
    }

    public IDialogBox SetIcon(Sprite sprite)
    {
        dialogData.Icon = sprite;
        return this;
    }

    public IDialogBox SetTitleColor(Color color)
    {
        dialogData.TitleColor = color;
        return this;
    }

    public IDialogBox SetTextColor(Color color)
    {
        dialogData.TextColor = color;
        return this;
    }

    public IDialogBox SetPrefButtonHeight(float prefHeight)
    {
        dialogData.PrefButtonHeight = prefHeight;
        return this;
    }

    public IDialogBox SetSize(float? width = null, float? height = null)
    {
        dialogData.Wifth = width;
        dialogData.Height = height;
        return this;
    }
}

