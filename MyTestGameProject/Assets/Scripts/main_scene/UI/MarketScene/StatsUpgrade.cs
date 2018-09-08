using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class StatsUpgrade : MonoBehaviour
{
    enum StatName { HEALTH, ATTACK, DEFENCE, SPEED, ACELERATION, ROTATION_SPEED }

    [Header("UI")]
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI currentValue;
    [SerializeField] TextMeshProUGUI additionalValue;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] Slider sliderToNextLevel;
    [SerializeField] Slider sliderToHightestLevel;

    [Header("Script")]
    [SerializeField] StatName statName;
    [SerializeField] AnimationCurve upgradeCost;
    [Space]
    [SerializeField] int uprgadeLevelsCount;
    [Space]
    [SerializeField] float minCost;
    [SerializeField] float maxCost;

    Button btn;

    DSUnitStats.Stat stat;

    float nextLevelCost;
    float nextLevelCostWhenProgressZero;
    float additionnalVal;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        SetValues();
    }

    private void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    void SetValues()
    {
        DSUnitStats stats = GameManager.Instance.SavablePlayerData.PlayerProgress.Stats;
        int roundDig = 0;
        string percentFormat = "##0.##%";
        string addValPercevtFormat = "+##0.##%";
        string valFormat = "##0.##";
        string addValFormat = "+##0.##";

        switch (statName)
        {
            case StatName.HEALTH:
                name.text = LocalizedStrings.health;
                stat = stats.Health;
                currentValue.text = stat.Value.ToString(valFormat);
                roundDig = 0;
                break;

            case StatName.ATTACK:
                name.text = LocalizedStrings.attack;
                stat = stats.Attack;
                currentValue.text = (stat.Value * 1).ToString(percentFormat);
                roundDig = 2;
                addValFormat = addValPercevtFormat;
                break;

            case StatName.DEFENCE:
                name.text = LocalizedStrings.defence;
                stat = stats.Defence;
                currentValue.text = (stat.Value * 1).ToString(percentFormat);
                roundDig = 2;
                addValFormat = addValPercevtFormat;
                break;

            case StatName.SPEED:
                name.text = LocalizedStrings.speed;
                stat = stats.Speed;
                currentValue.text = stat.Value.ToString(valFormat);
                roundDig = 2;
                break;

            case StatName.ACELERATION:
                name.text = "УСКОРЕНИЕ";
                stat = stats.Aceleration;
                currentValue.text = stat.Value.ToString(valFormat);
                roundDig = 2;
                break;

            case StatName.ROTATION_SPEED:
                name.text = LocalizedStrings.rotationSpeed;
                stat = stats.RotationSpeed;
                currentValue.text = stat.Value.ToString(valFormat);
                roundDig = 0;
                break;
        }

        float addVal = (stat.MaxValue - stat.MinValue) / uprgadeLevelsCount;
        additionnalVal = (float)Math.Round(addVal, roundDig);
        additionalValue.text = additionnalVal.ToString(addValFormat);

        int currentLevel = (int)((stat.Value - stat.MinValue) / addVal);
        float x = (float)(currentLevel)/ uprgadeLevelsCount;
        sliderToHightestLevel.value = x;
        sliderToNextLevel.value = stat.Progress;

        nextLevelCostWhenProgressZero = (int)Math.Round(Mathf.Lerp(minCost, maxCost, upgradeCost.Evaluate(x)));
        nextLevelCost = (int)Math.Round((1 - stat.Progress) * nextLevelCostWhenProgressZero);

        cost.text = nextLevelCost.ToString("# ### ##0");
    }

    void OnClick()
    {
        DSPlayerScore score = GameManager.Instance.SavablePlayerData.PlayerProgress.Score;
        if (stat.Value < stat.MaxValue && score.expirience.Value > 0)
        {
            if (nextLevelCost <= score.expirience.Value)
            {
                score.expirience.Value -= nextLevelCost;
                stat.Value += additionnalVal;
                stat.Progress = 0;
            }
            else
            {
                nextLevelCost -= score.expirience.Value;
                score.expirience.Value = 0;
                stat.Progress = 1 - nextLevelCost / nextLevelCostWhenProgressZero;
            }
            SetValues();
        }
        else
        {
            if(score.expirience.Value <= 0)
                Toast.Instance.Show(LocalizedStrings.toast_not_enough_expirience);
            else
                Toast.Instance.Show(LocalizedStrings.toast_stat_max_upgrade);
        }
    }

    [ContextMenu("ResetThisStat")]
    void ResetStat()
    {
        DSUnitStats stats = GameManager.Instance.SavablePlayerData.PlayerProgress.Stats;
        switch (statName)
        {
            case StatName.HEALTH:
                stats.Health.Value = stats.Health.MinValue;
                break;

            case StatName.ATTACK:
                stats.Attack.Value = stats.Attack.MinValue;
                break;

            case StatName.DEFENCE:
                stats.Defence.Value = stats.Defence.MinValue;
                break;

            case StatName.SPEED:
                stats.Speed.Value = stats.Speed.MinValue;
                break;

            case StatName.ACELERATION:
                stats.Aceleration.Value = stats.Aceleration.MinValue;
                break;

            case StatName.ROTATION_SPEED:
                stats.RotationSpeed.Value = stats.RotationSpeed.MinValue;
                break;
        }
        SetValues();
    }
}
