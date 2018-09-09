using System;
using UnityEngine.EventSystems;

public class DropToInventory : ADropToMe
{
    public int cellIndex;
    public int CellIndex { get { return cellIndex; } }
    
    public override void OnDrop(PointerEventData eventData)
    {
        if (CanDrop)
        {
            DragEquipment drag = eventData.pointerDrag.GetComponent<DragEquipment>();

            if (drag != null && drag.CanDrag)
            {
                var oldParentDrop = drag.OldParent.GetComponent<ADropToMe>();

                //если в инвентаре есть стак
                if (transform.childCount > 0)
                {
                    DragEquipment thisDrag = transform.GetChild(0).GetComponent<DragEquipment>();

                    //если перетаскиваем на такой же объект, но перетаскиваем не с экипировки
                    if ((drag.EquipStack.EquipmentStats.Type == thisDrag.EquipStack.EquipmentStats.Type
                        && drag.EquipStack.EquipmentStats.Id == thisDrag.EquipStack.EquipmentStats.Id
                        && drag.EquipStack.EquipmentStats.ItemDurability == thisDrag.EquipStack.EquipmentStats.ItemDurability) && !(oldParentDrop is DropToEquipment))
                    {
                        //высчитываем кол-во ппедметов, которые можем состакать
                        int cnt = Squad.playerSquadInstance.UnitCount - thisDrag.EquipStack.Count;
                        cnt = cnt <= drag.EquipStack.Count ? cnt : drag.EquipStack.Count;

                        //заносим предметы в инвентарь отряда
                        EquipmentStack stack = new EquipmentStack(thisDrag.EquipStack.EquipmentMainProperties, thisDrag.EquipStack.EquipmentStats, cnt);
                        if (oldParentDrop.CanGetFromThisIventory(stack, thisDrag.EquipStack))
                            if (AddToThisInventory(stack))
                                oldParentDrop.RemoveFromThisInventory(stack);
                    }
                }
                //если в инвентаре нет стака
                else
                {
                    //высчитываем кол-во ппедметов, которые можем поднять
                    int cnt = drag.EquipStack.Count;
                    cnt = Squad.playerSquadInstance.UnitCount < cnt ? Squad.playerSquadInstance.UnitCount : cnt;
                    EquipmentStack stack = new EquipmentStack(drag.EquipStack.EquipmentMainProperties, drag.EquipStack.EquipmentStats, cnt);

                    if (oldParentDrop.CanGetFromThisIventory(stack, null))
                        if (AddToThisInventory(stack))
                            oldParentDrop.RemoveFromThisInventory(stack);
                }

                Destroy(drag.gameObject);
                RefreshUI();
            }
        }
    }

    public override bool AddToThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        if (stack.Count == 0)
            return false;

        //если был пустой, то заносим этот стак
        if (Squad.playerSquadInstance.Inventory[CellIndex] == null)
            Squad.playerSquadInstance.Inventory[CellIndex] = new EquipmentStack(stack.EquipmentMainProperties, stack.EquipmentStats, stack.Count);
        //если был НЕ пустой, то прибавлем
        else
            Squad.playerSquadInstance.Inventory[CellIndex].PushItems(stack);

        return true;
    }

    public override bool RemoveFromThisInventory(AStack aStack)
    {
        EquipmentStack stack = aStack as EquipmentStack;

        //очищаем инвентарь
        if (stack.EquipmentStats.Type == Squad.playerSquadInstance.Inventory[CellIndex].EquipmentStats.Type
            && stack.EquipmentStats.Id == Squad.playerSquadInstance.Inventory[CellIndex].EquipmentStats.Id
            && stack.EquipmentStats.ItemDurability == Squad.playerSquadInstance.Inventory[CellIndex].EquipmentStats.ItemDurability)
        {
            if (stack.Count == Squad.playerSquadInstance.Inventory[CellIndex].Count)
                Squad.playerSquadInstance.Inventory[CellIndex] = null;
            else
                Squad.playerSquadInstance.Inventory[CellIndex].PopItems(stack.Count);
        }
        else
            throw new System.Exception("Нельзя удалить из ячейки предметы, которые там не лежат");

        return true;
    }

    public override bool CanGetFromThisIventory(AStack aStack, AStack stackForReplacement)
    {
        return true;
    }
}
