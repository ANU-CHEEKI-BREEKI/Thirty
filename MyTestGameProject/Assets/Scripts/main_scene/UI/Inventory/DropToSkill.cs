using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToSkill : ADropToMe
{
    public enum SkillNum { FIRS, SECOND }
    [SerializeField] SkillNum skillNum;

    public override bool AddToThisInventory(AStack aStack)
    {
        SkillStack stack = aStack as SkillStack;

        var skillSaves = GameManager.Instance.PlayerProgress.skills;
        switch (skillNum)
        {
            case SkillNum.FIRS:
                Squad.playerSquadInstance.Inventory.FirstSkill.Skill = stack.Skill;
                Squad.playerSquadInstance.Inventory.FirstSkill.SkillStats = stack.SkillStats;
                skillSaves.firstSkill = Squad.playerSquadInstance.Inventory.FirstSkill.Skill;
                break;
            case SkillNum.SECOND:
                Squad.playerSquadInstance.Inventory.SecondSkill.Skill = stack.Skill;
                Squad.playerSquadInstance.Inventory.SecondSkill.SkillStats = stack.SkillStats;
                skillSaves.secondSkill = Squad.playerSquadInstance.Inventory.SecondSkill.Skill;
                break;
        }
        GameManager.Instance.PlayerProgress.skills.Save();

        return true;
    }

    public override bool CanGetFromThisIventory(AStack aStack)
    {
        //SkillStack stack = aStack as SkillStack;
        return true;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        DragSkill drag = eventData.pointerDrag.GetComponent<DragSkill>();

        if (drag != null && drag.CanDrag)
        {
            var oldDrop = drag.OldParent.GetComponent<ADropToMe>();

            oldDrop.RemoveFromThisInventory(null);

            //если в инвентаре есть стак
            if (transform.childCount > 0)
                RemoveFromThisInventory(null);
            AddToThisInventory(drag.SkillStack);

            Destroy(drag.gameObject);
            RefreshUI();
        }
    }

    public override bool RemoveFromThisInventory(AStack aStack)
    {
        SkillStack stack = aStack as SkillStack;

        switch (skillNum)
        {
            case SkillNum.FIRS:
                Squad.playerSquadInstance.Inventory.FirstSkill.Skill = null;
                break;
            case SkillNum.SECOND:
                Squad.playerSquadInstance.Inventory.SecondSkill.Skill = null;
                break;
        }

        return true;
    }
}
