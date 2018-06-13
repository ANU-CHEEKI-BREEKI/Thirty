using System;
using UnityEngine.EventSystems;

public class DropToConsumableMarket : ADropToMe
{
    override public void OnDrop(PointerEventData eventData)
    {
        var drag = eventData.pointerDrag.GetComponent<DragConsumable>();

        if (drag != null && drag.CanDrag)
        {
            ADropToMe oldParentDrop = drag.OldParent.GetComponent<ADropToMe>();

            //если перетащили откуда либо кроме самого магазина
            if (!(oldParentDrop is DropToConsumableMarket))
            {
                if (oldParentDrop.CanGetFromThisIventory(drag.ConsumableStack))
                    if (AddToThisInventory(drag.ConsumableStack))
                        oldParentDrop.RemoveFromThisInventory(drag.ConsumableStack);

                Destroy(drag.gameObject);
                RefreshUI();
            }
        }
    }

    public override bool AddToThisInventory(AStack stack)
    {
        bool res = false;
        if (MarketConsumablesUI.Instance != null)
        {
            if (!MarketConsumablesUI.Instance.Fill)
            {
                MarketConsumablesUI.Instance.AddToInventory(stack as ConsumableStack);
                res = true;
            }
        }
        return res;
    }

    public override bool RemoveFromThisInventory(AStack stack)
    {
        if(MarketConsumablesUI.Instance != null)
            MarketConsumablesUI.Instance.RemoveFtomInventory(stack as ConsumableStack);
        return true;
    }

    public override bool CanGetFromThisIventory(AStack stack)
    {
        bool res = false;
        var st = stack as ConsumableStack;
        if (st != null)
        {
            res = true;
            var score = GameManager.Instance.PlayerProgress.Score;

            if (st.ConsumableStats is ISkillCostable)
            {
                int cost = (st.ConsumableStats as ISkillCostable).Cost * st.Count;

                if (st.ConsumableStats is IStackCountConstraintable
                && st.Count != (st.ConsumableStats as IStackCountConstraintable).MaxCount
                && st.Consumable.MainPropertie.Currency != DSPlayerScore.Currency.GOLD)
                    cost = 0;
                    
                if (!score.EnoughtMoney(cost, st.Consumable.MainPropertie.Currency))
                {
                    res = false;
                    Toast.Instance.Show(score.NotEoughtMoveyWarningString(st.Consumable.MainPropertie.Currency));
                }
            }           
        }
        return res;
    }
}
