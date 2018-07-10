using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSkill : Drag
{
    [SerializeField] SkillStack skillStack;
    public SkillStack SkillStack { get { return skillStack; } set { skillStack = value; } }

    public override void OnPointerClick(PointerEventData eventData)
    {
        TipsPanel.Instance.Show(skillStack.GetDescription(), thisTransform.position);
      
    }

    protected override void OnCantDrag()
    {
        base.OnCantDrag();
    }
}
