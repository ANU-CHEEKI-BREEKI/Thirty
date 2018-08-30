using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/Money", fileName = "SO_SSR_Money")]
public class SOSquadSpawnerMoneyResourse : ScriptableObject
{
    [SerializeField] AnimationCurve moneyLevelDependency;
    [SerializeField] MoneyContainer[] moneyByLevel;
    public Money MoneyByLevel { get { return GetMoney(moneyByLevel); } }

    Money GetMoney(MoneyContainer[] moneys)
    {
        Money res = null;

        if (moneys.Length > 0)
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            float val = moneyLevelDependency.Evaluate(t);
            int index = Mathf.RoundToInt(moneyLevelDependency.Evaluate(t) * (moneys.Length - 1));
            int l2 = moneys[index].randomItems.Length;
            if (l2 > 0)
                res = moneys[index].randomItems[UnityEngine.Random.Range(0, l2)];
        }

        return res;
    }

    [Serializable]
    public class MoneyContainer
    {
        public Money[] randomItems;
    }
}
