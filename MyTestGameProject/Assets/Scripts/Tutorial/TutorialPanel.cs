using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [Header("UI", order = 0)]
    [Header("Main tips UI", order = 1)]
    [SerializeField] GameTipsPanel tipsPanel;
    [SerializeField] Button buttonOk;
    [SerializeField] CanvasGroup panelCanvasGroup;
    [SerializeField] TextMeshProUGUI currentOrderText;

    [Header("ALL tips review UI", order = 2)]
    [SerializeField] Button buttonOpenReview;
    [SerializeField] Button buttonCloseReview;
    [SerializeField] Button buttonPrev;
    [SerializeField] Button buttonNext;
    [SerializeField] GameObject textNumbersPanel;
    [SerializeField] TextMeshProUGUI currentNumber;
    [SerializeField] TextMeshProUGUI allNumber;

    [Header("Script", order = 3)]
    [Header("Tips values", order = 4)]
    [SerializeField] TriggerTip[] triggerTips;

    TriggerTip lastTip;

    List<int> anotherTipTriggersId;
    List<TriggerTip> tipsToReview;
    int reviewIndex;

    void Start ()
    {
        anotherTipTriggersId = new List<int>();
        tipsToReview = new List<TriggerTip>();

        buttonOpenReview?.onClick.AddListener(()=>
            {
                reviewIndex = tipsToReview.Count - 1;
                if (reviewIndex >= 0)
                {
                    SetTip(tipsToReview[reviewIndex], true);
                    Show();
                    buttonOk.gameObject.SetActive(false);

                    buttonPrev.gameObject.SetActive(true);
                    buttonNext.gameObject.SetActive(true);
                    buttonPrev.interactable = reviewIndex > 0;
                    buttonNext.interactable = reviewIndex < tipsToReview.Count - 1;

                    textNumbersPanel.gameObject.SetActive(true);
                    currentNumber.text = (tipsToReview.Count).ToString(StringFormats.intNumber);
                    allNumber.text = (tipsToReview.Count).ToString(StringFormats.intNumber);

                    GameManager.Instance.PauseGame();
                }
            }
        );

        buttonCloseReview?.onClick.AddListener(() =>
            {
                Hide();
                GameManager.Instance.ResumeGame();
            }
        );

        buttonPrev?.onClick.AddListener(() =>
            {
                if(reviewIndex > 0)
                {
                    reviewIndex--;
                    SetTip(tipsToReview[reviewIndex], true);
                    currentNumber.text = (reviewIndex + 1).ToString(StringFormats.intNumber);
                }
                buttonPrev.interactable = reviewIndex > 0;
                buttonNext.interactable = reviewIndex < tipsToReview.Count - 1;
            }
        );

        buttonNext?.onClick.AddListener(() =>
            {
                if (reviewIndex < tipsToReview.Count - 1)
                {
                    reviewIndex++;
                    SetTip(tipsToReview[reviewIndex], true);
                    currentNumber.text = (reviewIndex + 1).ToString(StringFormats.intNumber);

                    buttonPrev.interactable = reviewIndex > 0;
                    buttonNext.interactable = reviewIndex < tipsToReview.Count - 1;
                    
                }
            }
        );

        buttonOk?.onClick.AddListener(() =>
            {
                GameManager.Instance.ResumeGame();
                Hide();
            }
        );

        int cnt = triggerTips.Length;
        for (int i = 0; i < cnt; i++)
        {
            triggerTips[i].id = i;

            if (triggerTips[i].needExequteAnotherTriggerAftarThat)
                anotherTipTriggersId.Add(triggerTips[i].anotherTriggerIndex);

            var ttip = triggerTips[i];

            if (ttip.trigger != null)
            {
                Func<bool> act = () => { return Action(ttip); };
                switch (ttip.triggerType)
                {
                    case GOTriggerEvent.TriggerType.STAY:
                        ttip.trigger.OnPlayerTriggerStay += act;
                        break;
                    case GOTriggerEvent.TriggerType.ENTER:
                        ttip.trigger.OnPlayerTriggerEnter += act;
                        break;
                    case GOTriggerEvent.TriggerType.EXIT:
                        ttip.trigger.OnPlayerTriggerExit += act;
                        break;
                    case GOTriggerEvent.TriggerType.DESTROY:
                        ttip.trigger.OnPlayerTriggerDestroy += act;
                        break;
                    case GOTriggerEvent.TriggerType.PDOWN:
                        ttip.trigger.OnPlayerTriggerPoinderDown += act;
                        break;
                    case GOTriggerEvent.TriggerType.PUP:
                        ttip.trigger.OnPlayerTriggerPoinderUp += act;
                        break;
                    case GOTriggerEvent.TriggerType.PCLICK:
                        ttip.trigger.OnPlayerTriggerPointerClick += act;
                        break;
                    case GOTriggerEvent.TriggerType.PDROP:
                        ttip.trigger.OnPlayerTriggerPointerDrop += act;
                        break;
                    case GOTriggerEvent.TriggerType.DISABLE:
                        ttip.trigger.OnPlayerTriggerDisable += act;
                        break;
                    case GOTriggerEvent.TriggerType.ENABLE:
                        ttip.trigger.OnPlayerTriggerEnable += act;
                        break;
                }
            }
        }

        Hide();       
    }

    bool Action(TriggerTip ttip)
    {
        bool res = true;
        if (ttip.requiresExequtePreviousTrigger)
            res = triggerTips[ttip.previousTriggerIndex].executed;

        if (res)
        {
            if (!ttip.isLastTrigger)
            {
                if (tipsPanel != null)// костыль
                {
                    SetTip(ttip);
                }

                var arr = ttip.toHide;
                var size = arr.Length;
                for (int j = 0; j < size; j++)
                {
                    if (arr[j] != null)
                    {
                        arr[j].alpha = 0;
                        arr[j].blocksRaycasts = false;
                    }
                }
                arr = ttip.toShow;
                size = arr.Length;
                for (int j = 0; j < size; j++)
                {
                    if (arr[j] != null)
                    {
                        arr[j].alpha = 1;
                        arr[j].blocksRaycasts = true;
                    }
                }
                arr = ttip.toEnable;
                size = arr.Length;
                for (int j = 0; j < size; j++)
                    if (arr[j] != null)
                        arr[j].interactable = true;
                arr = ttip.toDisable;
                size = arr.Length;
                for (int j = 0; j < size; j++)
                    if (arr[j] != null)
                        arr[j].interactable = false;
                var arrGO = ttip.toActivate;
                size = arrGO.Length;
                for (int j = 0; j < size; j++)
                    if (arrGO[j] != null)
                        arrGO[j].SetActive(true);
                arrGO = ttip.toDeactivate;
                size = arrGO.Length;
                for (int j = 0; j < size; j++)
                    if (arrGO[j] != null)
                        arrGO[j].SetActive(false);

                if(ttip.dublicateToReview)
                    tipsToReview?.Add(ttip);

                if (ttip.tip != null)
                {
                    Show();
                    GameManager.Instance.PauseGame();
                }                
            }
            else
            {
                var scInd = (GameManager.SceneIndex)SceneManager.GetActiveScene().buildIndex;
                if(scInd < GameManager.SceneIndex.LEVEL_TUTORIAL_3)
                {
                    var nextLevel = (GameManager.SceneIndex)((int)scInd + 1);
                    if(GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.AvalaibleTutorialLevel < nextLevel)
                        GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.AvalaibleTutorialLevel = nextLevel;
                    GameManager.Instance.LoadTutorialLevel(nextLevel);
                }
                else
                    GameManager.Instance.LoadMainMenu();
            }
        }

        triggerTips[ttip.id].executed = res;
        return res;
    }

    void Hide()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0;
            panelCanvasGroup.blocksRaycasts = false;
        }
        
        if (lastTip.screenTextDublicate)
        {
            if (lastTip.tip.IsLocalisedText)
                currentOrderText.text = Localization.GetString(lastTip.tip.TipText);
            else
                currentOrderText.text = lastTip.tip.TipText;

            currentOrderText.text = "<font=\"PFHellenicaSerifPro-Regular SDF\"><mark=#000000>" + currentOrderText.text;
            currentOrderText.enabled = true;
        }

        ExequteAnotherTipTrigger();
    }

    void ExequteAnotherTipTrigger()
    {
        int cnt = anotherTipTriggersId.Count;
        for (int i = 0; i < cnt; i++)
        {
            try
            {
                if (anotherTipTriggersId[i] == lastTip.anotherTriggerIndex)
                {
                    Action(triggerTips[lastTip.anotherTriggerIndex]);
                    anotherTipTriggersId.RemoveAt(i);
                    break;
                }
            }
            catch
            {
                Debug.Log("Tакого индекса нет!!!!!!!");
            }
        }
    }

    void Show()
    {
        buttonOk.gameObject.SetActive(true);

        buttonPrev.gameObject.SetActive(false);
        buttonNext.gameObject.SetActive(false);

        textNumbersPanel.gameObject.SetActive(false);

        panelCanvasGroup.alpha = 1;
        panelCanvasGroup.blocksRaycasts = true;

        currentOrderText.enabled = false;
    }
    
    void SetTip(TriggerTip ttip, bool itsReview = false)
    {
        if(ttip.tip != null)
            tipsPanel.SetTip(ttip.tip);

        if (!itsReview)
            lastTip = ttip;
    }

    [ContextMenu("RemoveUselessItems")]
    void RemoveUselessItems()
    {
        int cntToRemove = 0;
        for (int i = 0; i < triggerTips.Length; i++)
        {
            if(triggerTips[i].tip == null)
            {
                cntToRemove++;
                for (int j = i; j < triggerTips.Length - 1; j++)
                {
                    triggerTips[j] = triggerTips[j + 1];
                    triggerTips[j].id--;

                    if (triggerTips[j].needExequteAnotherTriggerAftarThat && triggerTips[j].anotherTriggerIndex >= i)
                        triggerTips[j].anotherTriggerIndex--;
                    else
                        triggerTips[j].anotherTriggerIndex = 0;

                    if (triggerTips[j].requiresExequtePreviousTrigger && triggerTips[j].previousTriggerIndex >= i)
                        triggerTips[j].previousTriggerIndex--;
                    else
                        triggerTips[j].previousTriggerIndex = 0;
                }
            }
        }
        Array.Resize(ref triggerTips, triggerTips.Length - cntToRemove);
        Debug.Log("Было удалено " + cntToRemove + " штук бесполезных итемов массива.");
    }

    [Serializable]
    struct TriggerTip
    {
        [HideInInspector] public int id;
        [HideInInspector] public bool executed;
        public bool isLastTrigger;
        [Space]
        public GOTriggerEvent trigger;
        public GOTriggerEvent.TriggerType triggerType;
        public SOTip tip;
        public bool screenTextDublicate;
        public bool dublicateToReview;
        [Space]
        public bool needExequteAnotherTriggerAftarThat;
        public int anotherTriggerIndex;
        [Space]
        public bool requiresExequtePreviousTrigger;
        public int previousTriggerIndex;
        [Space]
        public CanvasGroup[] toHide;
        public CanvasGroup[] toShow;
        [Space]
        public CanvasGroup[] toEnable;
        public CanvasGroup[] toDisable;
        [Space]
        public GameObject[] toActivate;
        public GameObject[] toDeactivate;
    }
}
