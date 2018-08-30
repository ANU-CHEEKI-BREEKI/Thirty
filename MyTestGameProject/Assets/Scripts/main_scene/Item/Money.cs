using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Money", fileName = "Money")]
public class Money : ScriptableObject
{
    [SerializeField] DSPlayerScore.Currency currency;
    public DSPlayerScore.Currency Currency { get { return currency; } }

    public void Use(int count, bool asTempValue = true)
    {
        var ps = GameManager.Instance.PlayerProgress.Score;
        if (asTempValue)
        {
            switch (currency)
            {
                case DSPlayerScore.Currency.SILVER:
                    ps.tempSilver.Value += count;
                    break;
                case DSPlayerScore.Currency.GOLD:
                    ps.tempGold.Value += count;
                    break;
                case DSPlayerScore.Currency.EXPIRIENCE:
                    ps.tempExpirience.Value += count;
                    break;
            }
        }
        else
        {
            switch (currency)
            {
                case DSPlayerScore.Currency.SILVER:
                    ps.silver.Value += count;
                    break;
                case DSPlayerScore.Currency.GOLD:
                    ps.gold.Value += count;
                    break;
                case DSPlayerScore.Currency.EXPIRIENCE:
                    ps.expirience.Value += count;
                    break;
            }
        }
    }
}
