using System;
using UnityEngine.EventSystems;

public class DropToEquipmentMarket : ADropToMe
{
    override public void OnDrop(PointerEventData eventData)
    {
        DragEquipment drag = eventData.pointerDrag.GetComponent<DragEquipment>();

        if (drag != null && drag.CanDrag)
        {
            ADropToMe oldParentDrop = drag.OldParent.GetComponent<ADropToMe>();

            //если перетащили откуда либо кроме самого магазина
            if (!(oldParentDrop is DropToEquipmentMarket) && !(oldParentDrop is DropToConsumableMarket))
            {
                EquipmentStack stack = new EquipmentStack(drag.EquipStack.EquipmentMainProperties, drag.EquipStack.EquipmentStats, drag.EquipStack.Count);
                if (oldParentDrop.CanGetFromThisIventory(stack))
                    if (AddToThisInventory(stack))
                        oldParentDrop.RemoveFromThisInventory(stack);

                Destroy(drag.gameObject);
                RefreshUI();
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

        GameManager.Instance.PlayerProgress.score.EarnMoney(summ, stack.EquipmentMainProperties.Currency);

        // а тут уже добавояем
        MarketInventoryUI.Instance.AddToInventory(stack);

        return true;
    }

    public override bool RemoveFromThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        float summ = stack.EquipmentStats.Cost * stack.Count;
        summ = (float)Math.Truncate(summ);

        GameManager.Instance.PlayerProgress.score.SpendMoney(summ, stack.EquipmentMainProperties.Currency);

        // а тут уже удаляем
        MarketInventoryUI.Instance.RemoveFtomInventory(stack);

        return true;
    }

    public override bool CanGetFromThisIventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        float summ = stack.EquipmentStats.Cost * stack.Count;

        if (GameManager.Instance.PlayerProgress.score.EnoughtMoney(summ, stack.EquipmentMainProperties.Currency))
            return true;
        else
        {
            Toast.Instance.Show(GameManager.Instance.PlayerProgress.score.NotEoughtMoveyWarningString(stack.EquipmentMainProperties.Currency));
            return false;
        }
    }
}
