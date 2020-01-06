using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyTheGold : MonoBehaviour
{
    [SerializeField] float gold;
    [SerializeField] float cost;
    [SerializeField] bool removeAds;
    Button btn;
    Text text;

    void Start ()
    {
        btn=GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
        if (btn.transform.childCount > 0)
            text=btn.transform.GetChild(0).GetComponent<Text>();
        if (text != null)
            text.text="+" + gold.ToString();
    }

    void OnBtnClick()
    {
        if (cost <= 0)
        {
            var showed=GADWrapper.ShowRewardedAd(
                GADWrapper.RewardedAdId.ID_FREE_GOLD,
                (reward) => MainThreadDispatcher.Instance.Enqueue(() => AddGold(gold)),
                true,
                false,
                2
            );
            if(!showed)
            {
                GADWrapper.LoadRewardedAd(
                    GADWrapper.RewardedAdId.ID_FREE_GOLD, 
                    2
                );
            }
        }
        else
        {
            var id="";
            
            Action onSuccess=() => { MainThreadDispatcher.Instance.Enqueue(() => AddGold(gold)); };
            switch (cost)
            {
                case 1:
                    id=IAPWrapper.Const.Consumable.ID_GOLD_1;
                    break;
                case 2:
                    id=IAPWrapper.Const.Consumable.ID_GOLD_2;
                    break;
                default:
                    id=IAPWrapper.Const.Consumable.ID_GOLD_3;
                    break;
            }
            IAPWrapper.Initiate(false);
            IAPWrapper.BuyProduct(id, onSuccess, disableAds: removeAds);
        }

    }

    void AddGold(float gold)
    {
        GameManager.Instance.SavablePlayerData.PlayerProgress.Score.gold.Value += gold;
        GameManager.Instance.SavablePlayerData.PlayerProgress.Save();
    }
}