using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MarketReminder : MonoBehaviour
{
    enum ReminderType { SKILL, CONSUMABLE, EXPIRIENCE }
    enum WhichOf { ANY, FIRST, SECOND }

    [SerializeField] ReminderType type;
    [SerializeField] WhichOf whichOf;
    Image icon;

    #region API
    void Awake()
    {
        icon = GetComponent<Image>();    
    }

    void Start()
    {
        switch (type)
        {
            case ReminderType.SKILL:
                SetupSkill();
                break;
            case ReminderType.CONSUMABLE:
                SetupConsumable();
                break;
            case ReminderType.EXPIRIENCE:
                SetupExpirience();
                break;
        }
    }

    void OnDestroy()
    {
        switch (type)
        {
            case ReminderType.SKILL:
                ShutDownSkill();
                break;
            case ReminderType.CONSUMABLE:
                ShutDownConsumable();
                break;
            case ReminderType.EXPIRIENCE:
                ShutDownExpirience();
                break;
        }
    }
    #endregion

    #region USER
    
    #region Skill
    void SetupSkill()
    {
        var playerInv = Squad.playerSquadInstance.Inventory;
        playerInv.FirstSkill.OnSkillChanged += OnSkillChanged;
        playerInv.SecondSkill.OnSkillChanged += OnSkillChanged;

        OnSkillChanged(null);
    }

    void ShutDownSkill()
    {
        var playerInv = Squad.playerSquadInstance.Inventory;
        playerInv.FirstSkill.OnSkillChanged -= OnSkillChanged;
        playerInv.SecondSkill.OnSkillChanged -= OnSkillChanged;
    }

    void OnSkillChanged(Executable ex)
    {
        var playerInv = Squad.playerSquadInstance.Inventory;
        bool first = playerInv.FirstSkill.Skill == null;
        bool second = playerInv.SecondSkill.Skill == null;

        Hide();

        switch (whichOf)
        {
            case WhichOf.ANY:
                if (first || second) Show();
                break;
            case WhichOf.FIRST:
                if (first) Show();
                break;
            case WhichOf.SECOND:
                if (second) Show();
                break;
        }
    }
    #endregion 

    #region Consumable
    void SetupConsumable()
    {
        var playerInv = Squad.playerSquadInstance.Inventory;
        playerInv.FirstConsumable.OnConsumableChanged += OnConsumableChanged;
        playerInv.SecondConsumable.OnConsumableChanged += OnConsumableChanged;

        OnConsumableChanged(null);
    }

    void ShutDownConsumable()
    {
        var playerInv = Squad.playerSquadInstance.Inventory;
        playerInv.FirstConsumable.OnConsumableChanged -= OnConsumableChanged;
        playerInv.SecondConsumable.OnConsumableChanged -= OnConsumableChanged;
    }

    void OnConsumableChanged(Executable ex)
    {
        var playerInv = Squad.playerSquadInstance.Inventory;
        bool first = playerInv.FirstConsumable.Consumable == null;
        bool second = playerInv.SecondConsumable.Consumable == null;

        Hide();

        switch (whichOf)
        {
            case WhichOf.ANY:
                if (first || second) Show();
                break;
            case WhichOf.FIRST:
                if (first) Show();
                break;
            case WhichOf.SECOND:
                if (second) Show();
                break;
        }
    }
    #endregion 

    #region Expirience
    void SetupExpirience()
    {

    }

    void ShutDownExpirience()
    {

    }
    #endregion 

    void Show()
    {
        icon.enabled = true;
    }

    void Hide()
    {
        icon.enabled = false;
    }
    #endregion
}
