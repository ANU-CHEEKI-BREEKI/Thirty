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

    

    void Start ()
    {
        buttonOk.onClick.AddListener(()=> 
        {
            Hide();
            tipsPanel.ResetTipPanelLayout();
            GameManager.Instance.Resume();
        });

        int cnt = triggerTips.Length;
        for (int i = 0; i < cnt; i++)
        {

            var ttip = triggerTips[i];
            var arrH = ttip.toHide;
            var sizeH = arrH.Length;
            var arrS = ttip.toShow;
            var sizeS = arrS.Length;
            var tip = ttip.tip;

            Action act = () =>
            {
                SetTip(ttip);

                for (int j = 0; j < sizeH; j++)
                {
                    arrH[j].alpha = 0;
                    arrH[j].blocksRaycasts = false;
                }

                for (int j = 0; j < sizeS; j++)
                {
                    arrS[j].alpha = 1;
                    arrS[j].blocksRaycasts = true;
                }

                Show();
                GameManager.Instance.Pause();
            };


            switch (triggerTips[i].triggerType)
            {
                case GOTriggerEvent.TriggerType.STAY:
                    triggerTips[i].trigger.OnPlayerTriggerStay += act;
                    break;
                case GOTriggerEvent.TriggerType.ENTER:
                    triggerTips[i].trigger.OnPlayerTriggerEnter += act;
                    break;
                case GOTriggerEvent.TriggerType.EXIT:
                    triggerTips[i].trigger.OnPlayerTriggerExit += act;
                    break;
            }
            
        }

        tipsPanel.ResetTipPanelLayout();
        Hide();       
    }
	
    void Hide()
    {
        panelCanvasGroup.alpha = 0;
        panelCanvasGroup.blocksRaycasts = false;
        currentOrderText.enabled = true;
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
    }

    [Serializable]
    struct TriggerTip
    {
        public GOTriggerEvent trigger;
        public GOTriggerEvent.TriggerType triggerType;
        public SOTip tip;
        public bool screenTextDublicate;
        [Space]
        public bool needExequteAnotherTriggerAftarThat;
        public int anotherTriggerIndex;
        [Space]
        public CanvasGroup[] toHide;
        public CanvasGroup[] toShow;
    }
}
