using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class TipsPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public const int COUNT_OF_DESC_ITEM = 4;
    public const int DESCRIPTION = 0;
    public const int COST = 1;
    public const int CONSTRAINTS = 2;
    public const int STATS = 3;

    [Header("Header")]
    [SerializeField] Image itemIcon;
    [Space]
    [SerializeField] CanvasGroup costPanel;
    [SerializeField] TextMeshProUGUI costPerOneText;
    [SerializeField] TextMeshProUGUI costAllText;
    [SerializeField] Image currencyIcon;
    [SerializeField] Sprite[] currency;
    [Space]
    [SerializeField] TextMeshProUGUI conditionText;
    [Space]
    [SerializeField] StatsBlock howToUsePanel;
    [Header("Description")]
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI description;
    [Header("Stats")]
    [SerializeField] Transform constraintsPanel;
    [SerializeField] Transform statsPanel;
    [SerializeField] GameObject statsBlockOriginal;    
    [Space]
    [SerializeField] Transform actionButtonsPanel;
    [SerializeField] GameObject actionButtonOriginal;

    public RectTransform ThisTransform { get; private set; }

    public static TipsPanel Instance { get; private set; }
    public bool Showed { get { return gameObject.activeSelf; } }

    Vector2 startSwipe;
    Vector2 startSwipePosition;

    Vector2 mouseDownStartPosition;
    Vector2 mouseDownPosition;

    void Awake()
    {
        Instance = this;
        ThisTransform = (RectTransform)transform;
        gameObject.SetActive(false);
    }

    void SetDescription(Description desc)
    {
        for (int i = 0; i < constraintsPanel.childCount; i++)
            Destroy(constraintsPanel.GetChild(i).gameObject);
        for (int i = 0; i < statsPanel.childCount; i++)
            Destroy(statsPanel.GetChild(i).gameObject);

        if (desc.Icon != null)
        {
            itemIcon.sprite = desc.Icon;
            itemIcon.gameObject.SetActive(true);
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
        }

        if(desc.Name != null)
            name.text = desc.Name;

        if (desc.Desc != null)
            description.text = desc.Desc;

        if(desc.Constraints != null && desc.Constraints.Length > 0)
        {
            foreach (var d in desc.Constraints)
            {
                GameObject go = Instantiate(statsBlockOriginal, constraintsPanel);
                StatsBlock sb = go.GetComponent<StatsBlock>();
                sb.SetName(d.Name);
                sb.SetValue(d.Description, d.ItPositiveDesc);
            }
            constraintsPanel.gameObject.SetActive(true);
        }
        else
            constraintsPanel.gameObject.SetActive(false);

        if (desc.Stats != null && desc.Stats.Length > 0)
        {
            foreach (var d in desc.Stats)
            {
                GameObject go = Instantiate(statsBlockOriginal, statsPanel);
                StatsBlock sb = go.GetComponent<StatsBlock>();
                sb.SetName(d.Name);
                sb.SetValue(d.Description, d.ItPositiveDesc);
            }
            statsPanel.gameObject.SetActive(true);
        }
        else
            statsPanel.gameObject.SetActive(false);

        if (desc.Cost != null)
        {
            costPanel.alpha = 1;
            if (desc.Cost.Value.CostPerOne != null)
            {
                costPerOneText.gameObject.SetActive(true);
                costPerOneText.text = desc.Cost.Value.CostPerOne.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
            }
            else
            {
                costPerOneText.gameObject.SetActive(false);
            }

            if (desc.Cost.Value.CostAll != null)
            {
                costAllText.gameObject.SetActive(true);
                costAllText.text = desc.Cost.Value.CostAll.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
            }
            else
            {
                costAllText.gameObject.SetActive(false);
            }
            
            if (desc.Cost.Value.CostCurrency != null)
                currencyIcon.sprite = currency[(int)desc.Cost.Value.CostCurrency];
            else
                currencyIcon.sprite = currency[(int)DSPlayerScore.Currency.SILVER];
        }
        else
        {
            costPanel.alpha = 0;
        }

        if (desc.Condition != null)
        {
            conditionText.text = desc.Condition.Value.Name;
            conditionText.color = desc.Condition.Value.Value.GetColor();
            conditionText.gameObject.SetActive(true);
            name.alignment = TextAlignmentOptions.BottomLeft;
        }
        else
        {
            conditionText.gameObject.SetActive(false);
            name.alignment = TextAlignmentOptions.MidlineLeft;
        }

        if (!string.IsNullOrEmpty(desc.UseType))
        {
            howToUsePanel.SetName("How to use:");
            howToUsePanel.SetValue(desc.UseType);
        }
        howToUsePanel.gameObject.SetActive(!string.IsNullOrEmpty(desc.UseType));
    }

    public void Show(Description desc, Vector2 worldPosOnScreen, List<Action> actions = null, List<string> actionsNames = null)
    {
        SetDescription(desc);

        if (actions != null && actionsNames != null && actions.Count > 0 && actions.Count == actionsNames.Count)
        {
            int cnt = actionButtonsPanel.childCount;
            for (int i = 0; i < cnt; i++)
                Destroy(actionButtonsPanel.GetChild(i).gameObject);

            cnt = actions.Count;
            for (int i = 0; i < cnt; i++)
            {
                Button b = Instantiate(actionButtonOriginal, actionButtonsPanel).GetComponent<Button>();
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = actionsNames[i];
                b.onClick.AddListener(new UnityEngine.Events.UnityAction(actions[i]));
            }
            actionButtonsPanel.gameObject.SetActive(true);
        }
        else
        {
            actionButtonsPanel.gameObject.SetActive(false);
        }

        GameManager.Instance.StartCoroutine(WaitForeRebuildPanel(worldPosOnScreen));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    IEnumerator WaitForeRebuildPanel(Vector2 worldPosOnScreen)
    {
        bool active = gameObject.activeSelf;

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(ThisTransform);
        gameObject.SetActive(false);
        gameObject.SetActive(active);

        yield return new WaitForEndOfFrame();

        if (!gameObject.activeInHierarchy)
        {
            ThisTransform.position = GetPosOnScreen(worldPosOnScreen);
            gameObject.SetActive(true);
        }
        else
        {
            ThisTransform.position = GetPosOnScreen(ThisTransform.position);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        int count = Input.touchCount;
        Touch touch;
        if (count > 0)
        {
            touch = Input.GetTouch(Input.touchCount - 1);
        }
        else
        {
            touch = new Touch();
            touch.position = Input.mousePosition;
        }

        startSwipe = Camera.main.ScreenToWorldPoint(touch.position);

        startSwipePosition = ThisTransform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        int count = Input.touchCount;
        Touch touch;
        if (count > 0)
        {
            touch = Input.GetTouch(Input.touchCount - 1);
        }
        else
        {
            touch = new Touch();
            touch.position = Input.mousePosition;
        }

        Vector2 swipe = (Vector2)Camera.main.ScreenToWorldPoint(touch.position) - startSwipe;

        (ThisTransform).position = GetPosOnScreen(startSwipePosition + swipe);

        //для закрытия окна по клику на него. но только если небыло движенияэтого  окна
        if (touch.position != mouseDownStartPosition)
            mouseDownPosition = touch.position;
    }

    Vector3 GetPosOnScreen(Vector2 worldPosOnScreen)
    {
        Vector2 t = Camera.main.WorldToScreenPoint(worldPosOnScreen);

        Vector2 newPos = new Vector2(
            Mathf.Clamp(t.x, 0 + ThisTransform.rect.width / 2, Camera.main.pixelWidth - ThisTransform.rect.width / 2),
            Mathf.Clamp(t.y, 0 + ThisTransform.rect.height / 2, Camera.main.pixelHeight - ThisTransform.rect.height / 2)
        );

        newPos = Camera.main.ScreenToWorldPoint(newPos);

        return new Vector3(
            newPos.x,
            newPos.y,
            ThisTransform.position.z
        );
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        int count = Input.touchCount;
        Touch touch;
        if (count > 0)
        {
            touch = Input.GetTouch(Input.touchCount - 1);
        }
        else
        {
            touch = new Touch();
            touch.position = Input.mousePosition;
        }

        mouseDownStartPosition = touch.position;
        mouseDownPosition = mouseDownStartPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (mouseDownStartPosition == mouseDownPosition)
            Hide();
    }    
}
