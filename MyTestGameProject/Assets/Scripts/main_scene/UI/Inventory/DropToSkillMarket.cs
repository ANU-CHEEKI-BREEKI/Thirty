using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToSkillMarket : ADropToMe
{
    public override bool AddToThisInventory(AStack aStack)
    {
        return true;
    }

    public override bool CanGetFromThisIventory(AStack aStack)
    {
        return true;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (CanDrop)
        {
            DragSkill drag = eventData.pointerDrag.GetComponent<DragSkill>();

            if (drag != null && drag.CanDrag)
            {
                var oldParentDrop = drag.OldParent.GetComponent<ADropToMe>();
                oldParentDrop.RemoveFromThisInventory(null);
                AddToThisInventory(drag.SkillStack);

                Destroy(drag.gameObject);
                RefreshUI();
            }
        }
    }

    public override bool RemoveFromThisInventory(AStack aStack)
    {
        return true;
    }
}
