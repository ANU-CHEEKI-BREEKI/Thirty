using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameTipsPanel tipsPanel;
    [SerializeField] Button buttonOk;
    [SerializeField] CanvasGroup panelCanvasGroup;
    [SerializeField] TextMeshProUGUI currentOrderText;

    [Header("Script")]
    [SerializeField] TriggerTip[] triggerTips;

    TriggerTip lastTip;

    List<int> anotherTipTriggersId;

    void Start ()
    {
        anotherTipTriggersId = new List<int>();

        buttonOk.onClick.AddListener(()=> 
        {
            tipsPanel.ResetTipPanelLayout();
            GameManager.Instance.Resume();
            Hide();            
        });

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
                }
            }
        }

        tipsPanel.ResetTipPanelLayout();
        Hide();       
    }
	
    bool Action(TriggerTip ttip)
    {
        bool res = true;
        if(ttip.requiresExequtePreviousTrigger)
            res = triggerTips[ttip.previousTriggerIndex].executed;

        if (res)
        {
            if (!ttip.isLastTrigger)
            {
                SetTip(ttip);

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

                Show();
                GameManager.Instance.Pause();
            }
            else
            {
                GameManager.Instance.LoadMainMenu();
            }
        }

        triggerTips[ttip.id].executed = res;
        return res;
    }

    void Hide()
    {
        panelCanvasGroup.alpha = 0;
        panelCanvasGroup.blocksRaycasts = false;
        currentOrderText.enabled = true;

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
            catch(Exception ex)
            {
                Debug.Log("Tакого индекса нет!!!!!!!");
            }
        }
    }

    void Show()
    {
        panelCanvasGroup.alpha = 1;
        panelCanvasGroup.blocksRaycasts = true;
        currentOrderText.enabled = false;
    }

    void SetTip(TriggerTip ttip)
    {
        if (ttip.screenTextDublicate)
        {
            if (ttip.tip.IsLocalisedText)
                currentOrderText.text = Localization.GetString(ttip.tip.TipText);
            else
                currentOrderText.text = ttip.tip.TipText;
        }
        else
        {
            currentOrderText.text = string.Empty;
        }

        tipsPanel.SetTip(ttip.tip);

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
