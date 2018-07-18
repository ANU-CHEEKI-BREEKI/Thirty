using UnityEngine.EventSystems;

public class DropToGround : ADropToMe
{
    public override void OnDrop(PointerEventData eventData)
    {
        if (CanDrop)
        {
            DragEquipment drag = eventData.pointerDrag.GetComponent<DragEquipment>();

            if (drag != null && drag.CanDrag)
            {
                ADropToMe oldParentDrop = drag.OldParent.GetComponent<ADropToMe>();

                //если перетащили откуда либо на пол
                if (!(oldParentDrop is DropToGround))
                {
                    //если передащили на заполненную клетку
                    EquipmentStack stack = new EquipmentStack(drag.EquipStack.EquipmentMainProperties, drag.EquipStack.EquipmentStats, drag.EquipStack.Count);
                    if (oldParentDrop.CanGetFromThisIventory(drag.EquipStack))
                        if (AddToThisInventory(drag.EquipStack))
                            oldParentDrop.RemoveFromThisInventory(drag.EquipStack);

                    Destroy(drag.gameObject);
                    RefreshUI();
                }
            }
        }
    }

    public override bool AddToThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        if (stack.Count == 0)
            return false;

        //возможно, надо переписать так, чтобы дропал этот скрипт, а не скрипт отряда
        Squad.playerSquadInstance.DropEquipment(stack);

        return true;
    }

    public override bool RemoveFromThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        EnvirinmantInventoryUI.Instance.PickUpEquipment(stack);

        return true;
    }

    public override bool CanGetFromThisIventory(AStack aStack)
    {
        return true;
    }
}
