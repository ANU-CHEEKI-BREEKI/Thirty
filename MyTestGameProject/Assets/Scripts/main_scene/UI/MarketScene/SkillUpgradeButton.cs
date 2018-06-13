using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Description;

[ExecuteInEditMode]
public class SkillUpgradeButton : MonoBehaviour, IDescriptionable, IPointerClickHandler, IComparable<SkillUpgradeButton>
{
    public enum UpgradeValueType { UNIT, PERCENT}

    [Header("UI")]
	[SerializeField] Sprite iconSource;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI currentLevelText;
    [SerializeField] TextMeshProUGUI maximumLevelText;
    [Space]
    [SerializeField] GameObject costPanel;
    [Space]
    [SerializeField] string nameResourceString;
    [SerializeField] string descriptionResourceString;
    [SerializeField] string upgradeStatDescriptionResourceString;
    [Space]
    [SerializeField] Image background;
    [SerializeField] Color lockedColor;
    [SerializeField] Color unlockedColor;
    [SerializeField] Color doneColor;
    [Header("Script")]
    [SerializeField] SkillsUpgrade parent;
    [Space]
    [SerializeField] SkillUpgradeButton[] previousButton;
    [SerializeField] SkillUpgradeButton[] nextButton;
    [Space]
    [SerializeField] DSPlayerSkill.SkillUpgrade upgradeStats;
    public DSPlayerSkill.SkillUpgrade UpgradeStats { get { return upgradeStats; } }
    [SerializeField] UpgradeValueType displayAs;
    [Space]
    [SerializeField] AnimationCurve costDependency;
    [SerializeField] int minCost;
    [SerializeField] int maxCost;
    [SerializeField] int maxLevel;
    [SerializeField] bool isPositiveUpgrade;

    public int Id { get { return upgradeStats.Id; } }
    int currentCost;

    List<Action> actions;
    List<string> actionsNames;

    Button btn;

    UILineConnector connector;
    bool locked = true;

    void Awake()
    {
        upgradeStats.Id = transform.GetSiblingIndex();
        parent.Buttons.Add(this);
        connector = GetComponent<UILineConnector>();

		icon.sprite = iconSource;
		
        if (connector != null && previousButton.Length > 0)
            connector.PrevObj = previousButton[0].transform;
    }

#if UNITY_EDITOR

    void Update()
    {
		icon.sprite = iconSource;
		
        if (connector != null && previousButton.Length > 0)
            connector.PrevObj = previousButton[0].transform;
    }

#endif

    /// <summary>
    /// 
    /// </summary>
    /// <param name="upgrade"></param>
    /// <param name="load">передаем false и этот метод переприсвоит переданный в первом параметре апгрейд. если передадим true то будет загружен переданный в первом параметре апгрейд</param>
    public void Initiate(DSPlayerSkill.SkillUpgrade upgrade, bool load)
    {
        if (!load)
        {
            upgrade.AdditionalValuePerLevel = upgradeStats.AdditionalValuePerLevel;
            upgrade.FieldName = upgradeStats.FieldName;
            upgrade.level = upgradeStats.level;
            upgrade.isUpgradeToUnlock = upgradeStats.isUpgradeToUnlock;
        }
        upgradeStats = upgrade;

        btn = GetComponent<Button>();
        //btn.onClick.AddListener(OnClick); через это сыбытие, почему то, оооочень сильно лагает... ватаФАК??? НЕ УДАЛАТЬ КОМЕНТАРИЙ!! ОБ ЭТОМ НУЖНО ЗНАТЬ
        actions = new List<Action>();
        actionsNames = new List<string>();
        Action ok = Upgrade;
        ok += TipsPanel.Instance.Hide;
        actions.Add(ok);
        actionsNames.Add(Localization.upgrade);
        actions.Add(TipsPanel.Instance.Hide);
        actionsNames.Add(Localization.cancel);

        maximumLevelText.text = "/" + maxLevel.ToString();

        Refresh();
    }

    int CalcLevelCost(int level)
    {
        float v;
        if (maxLevel > 1)
        {
            float t = (float)(level - 1) / (float)(maxLevel - 1);
            v = Mathf.Lerp(minCost, maxCost, costDependency.Evaluate(t));
        }
        else
            v = minCost;

        return (int)v;
    }

    void Refresh()
    {
        currentLevelText.text = upgradeStats.level.ToString();
        currentCost = CalcLevelCost(upgradeStats.level + 1);

        string cost = currentCost.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
        if (cost.Length > 8)
            cost = (currentCost * 0.000001).ToString(StringFormats.intSeparatorNumber, StringFormats.nfi) + "m";
        else if (cost.Length > 5)
            cost = (currentCost * 0.001).ToString(StringFormats.intSeparatorNumber, StringFormats.nfi) + "k";

        costText.text = cost;

        bool lvlZero = false;
        if (previousButton.Length > 0)
        {
            foreach (var item in previousButton)
            {
                if (item != null && item.upgradeStats.level == 0)
                {
                    lvlZero = true;
                    break;
                }
            }
        }

        locked = lvlZero;

        if(upgradeStats.isUpgradeToUnlock)
            locked = false;

        btn.interactable = !locked;
        if (connector != null)
            connector.Enabled = !locked;

        if(locked)
            background.color = lockedColor;
        else
            background.color = unlockedColor;

        if (upgradeStats.level == maxLevel)
        {
            btn.interactable = false;
            costPanel.SetActive(false);
            background.color = doneColor;
        }
    }
    
    void Upgrade()
    {
        var score = GameManager.Instance.PlayerProgress.Score;
        if (score.EnoughtMoney(currentCost, DSPlayerScore.Currency.EXPIRIENCE))
        {
            score.SpendMoney(currentCost, DSPlayerScore.Currency.EXPIRIENCE);
            upgradeStats.level++;

            GameManager.Instance.PlayerProgress.Skills.Save();

            Refresh();
            foreach (var item in nextButton)
                item.Refresh();

            SkillMarketInventoryUI.Instance.RefreshUI();
        }
        else
        {
            Toast.Instance.Show(Localization.toast_not_enough_expirience);
        }
    }

    public Description GetDescription()
    {
        Description res = new Description();

        res.Icon = icon.sprite;
        res.Name = Localization.GetString(nameResourceString);
        res.Desc = Localization.GetString(descriptionResourceString);
        res.Cost = new Description.CostInfo() { CostPerOne = currentCost, CostCurrency = DSPlayerScore.Currency.EXPIRIENCE };

        if (upgradeStats.AdditionalValuePerLevel != 0)
        {
            string format;
            if (displayAs == UpgradeValueType.UNIT)
                format = StringFormats.floatSignNumber;
            else
                format = StringFormats.floatSignNumberPercent;

            res.Stats = new DescriptionItem[]
            {
               new DescriptionItem()
               {
                   Name = Localization.GetString(upgradeStatDescriptionResourceString),
                   Description = upgradeStats.AdditionalValuePerLevel.ToString(format),
                   ItPositiveDesc = isPositiveUpgrade
               }
            };
        }

        return res;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!btn.interactable)
            TipsPanel.Instance.Show(GetDescription(), transform.position);
        else
            TipsPanel.Instance.Show(GetDescription(), transform.position, actions, actionsNames);
    }

    public int CompareTo(SkillUpgradeButton other)
    {
        return upgradeStats.Id - other.upgradeStats.Id;
    }
}
