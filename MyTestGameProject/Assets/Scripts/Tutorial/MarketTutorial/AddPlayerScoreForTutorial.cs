using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Это скрипт-костыль для обучения в магазине
/// </summary>
public class AddPlayerScoreForTutorial : MonoBehaviour
{
    [SerializeField] float scoreValue;
    [SerializeField] DSPlayerScore.Currency currency;

    void Start()
    {
        var score = GameManager.Instance?.SavablePlayerData?.PlayerProgress?.Score;
        if (score != null)
        {
            switch (currency)
            {
                case DSPlayerScore.Currency.SILVER:
                    score.silver.Value += scoreValue;
                    break;
                case DSPlayerScore.Currency.GOLD:
                    score.gold.Value += scoreValue;
                    break;
                case DSPlayerScore.Currency.EXPIRIENCE:
                    score.expirience.Value += scoreValue;
                    break;
            }
        }
    }
}
