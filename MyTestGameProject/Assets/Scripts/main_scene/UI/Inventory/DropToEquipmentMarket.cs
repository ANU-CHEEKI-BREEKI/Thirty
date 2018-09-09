using System;
using UnityEngine.EventSystems;

public class DropToEquipmentMarket : ADropToMe
{
    override public void OnDrop(PointerEventData eventData)
    {
        if (CanDrop)
        {
            DragEquipment drag = eventData.pointerDrag.GetComponent<DragEquipment>();

            if (drag != null && drag.CanDrag)
            {
                ADropToMe oldParentDrop = drag.OldParent.GetComponent<ADropToMe>();

                //если перетащили откуда либо кроме самого магазина
                if (!(oldParentDrop is DropToEquipmentMarket) && !(oldParentDrop is DropToConsumableMarket))
                {
                    EquipmentStack stack = new EquipmentStack(drag.EquipStack.EquipmentMainProperties, drag.EquipStack.EquipmentStats, drag.EquipStack.Count);
                    if (oldParentDrop.CanGetFromThisIventory(stack, null))
                        if (AddToThisInventory(stack))
                            oldParentDrop.RemoveFromThisInventory(stack);

                    Destroy(drag.gameObject);
                    RefreshUI();
                }
            }
        }
    }

    public override bool AddToThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        if (MarketInventoryUI.Instance.Fill)
            return false;

        float summ = stack.EquipmentStats.Cost * stack.Count;
        summ = (float)Math.Truncate(summ);

        GameManager.Instance.SavablePlayerData.PlayerProgress.Score.EarnMoney(summ, stack.EquipmentMainProperties.Currency);

        // а тут уже добавояем
        MarketInventoryUI.Instance.AddToInventory(stack);

        return true;
    }

    public override bool RemoveFromThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        float summ = stack.EquipmentStats.Cost * stack.Count;
        summ = (float)Math.Truncate(summ);

        GameManager.Instance.SavablePlayerData.PlayerProgress.Score.SpendMoney(summ, stack.EquipmentMainProperties.Currency);

        // а тут уже удаляем
        MarketInventoryUI.Instance.RemoveFtomInventory(stack);

        return true;
    }

    public override bool CanGetFromThisIventory(AStack aStack, AStack stackForReplacement)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        float summ = stack.EquipmentStats.Cost * stack.Count;

        EquipmentStack stackForReplace;
        if (stackForReplacement != null)
        {
            stackForReplace = stackForReplacement as EquipmentStack;
            if (!stackForReplace.EquipmentStats.Empty && stack.EquipmentMainProperties.Currency == stackForReplace.EquipmentMainProperties.Currency)
                summ -= stackForReplace.EquipmentStats.Cost * stackForReplace.Count;

            if (summ < 0) summ = 0;
        }

        if (GameManager.Instance.SavablePlayerData.PlayerProgress.Score.EnoughtMoney(summ, stack.EquipmentMainProperties.Currency))
            return true;
        else
        {
            Toast.Instance.Show(GameManager.Instance.SavablePlayerData.PlayerProgress.Score.NotEoughtMoveyWarningString(stack.EquipmentMainProperties.Currency));
            return false;
        }
    }
}
