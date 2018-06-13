using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToConsumable : ADropToMe
{
    public enum SkillNum { FIRS, SECOND }
    [SerializeField] SkillNum consumableCellNum;

    public override bool AddToThisInventory(AStack aStack)
    {
        var stack = aStack as ConsumableStack;

        switch (consumableCellNum)
        {
            case SkillNum.FIRS:
                Squad.playerSquadInstance.Inventory.FirstConsumable.Consumable = stack.Consumable;
                Squad.playerSquadInstance.Inventory.FirstConsumable.ConsumableStats = stack.ConsumableStats;
                Squad.playerSquadInstance.Inventory.FirstConsumable.Count = stack.Count;
                break;
            case SkillNum.SECOND:
                Squad.playerSquadInstance.Inventory.SecondConsumable.Consumable = stack.Consumable;
                Squad.playerSquadInstance.Inventory.SecondConsumable.ConsumableStats = stack.ConsumableStats;
                Squad.playerSquadInstance.Inventory.SecondConsumable.Count = stack.Count;
                break;
        }

        var score = GameManager.Instance.PlayerProgress.Score;
        int cost = -1;
        //проверяем стоит ли оно денег
        if (stack.ConsumableStats is ISkillCostable)
            cost = (stack.ConsumableStats as ISkillCostable).Cost * stack.Count;

        //если оно уже ипользовано, то бесплатно добавляем (но ток если это не голдовый расходник)
        if (stack.ConsumableStats is IStackCountConstraintable
        && stack.Count != (stack.ConsumableStats as IStackCountConstraintable).MaxCount
        && stack.Consumable.MainPropertie.Currency != DSPlayerScore.Currency.GOLD)
            cost = 0;
        if(cost > 0)
            score.SpendMoney(cost, stack.Consumable.MainPropertie.Currency);

        return true;
    }

    public override bool CanGetFromThisIventory(AStack aStack)
    {
        //var stack = aStack as ConsumableStack;
        return true;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        var drag = eventData.pointerDrag.GetComponent<DragConsumable>();

        if (drag != null && drag.CanDrag)
        {
            var oldDrop = drag.OldParent.GetComponent<ADropToMe>();

            //если ячейка свободна
            if (transform.childCount == 0)
            {
                if (oldDrop.CanGetFromThisIventory(drag.ConsumableStack))
                    if (AddToThisInventory(drag.ConsumableStack))
                        oldDrop.RemoveFromThisInventory(drag.ConsumableStack);
            }

            Destroy(drag.gameObject);
            RefreshUI();
        }
    }

    public override bool RemoveFromThisInventory(AStack aStack)
    {
        var stack = aStack as ConsumableStack;

        switch (consumableCellNum)
        {
            case SkillNum.FIRS:
                Squad.playerSquadInstance.Inventory.FirstConsumable.Consumable = null;
                break;
            case SkillNum.SECOND:
                Squad.playerSquadInstance.Inventory.SecondConsumable.Consumable = null;
                break;
        }

        //если уже использовал, то за продажу денег не дадут!!! но если это за голду, то дадут :)
        if (stack.ConsumableStats is ISkillCostable)
        {
            bool t = true;
            if (stack.ConsumableStats is IStackCountConstraintable)
            {
                if ((stack.ConsumableStats as IStackCountConstraintable).MaxCount != stack.Count 
                && stack.Consumable.MainPropertie.Currency != DSPlayerScore.Currency.GOLD)
                    t = false;
            }

            if (t)
            {
                int cost = (stack.ConsumableStats as ISkillCostable).Cost * stack.Count;
                var score = GameManager.Instance.PlayerProgress.Score;
                score.EarnMoney(cost, stack.Consumable.MainPropertie.Currency);
            }
        }

        return true;
    }

}
