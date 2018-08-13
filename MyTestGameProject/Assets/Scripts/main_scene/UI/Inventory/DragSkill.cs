using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSkill : Drag
{
    [SerializeField] SkillStack skillStack;
    public SkillStack SkillStack { get { return skillStack; } set { skillStack = value; } }

    public override AStack Stack
    {
        get
        {
            return SkillStack;
        }
        set
        {
            if (value is SkillStack)
                SkillStack = value as SkillStack;
            else
                throw new ArgumentException("не тот стак засунуть пытаешься.");
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (CanCallClick)
            TipsPanel.Instance.Show(skillStack.GetDescription(), thisTransform.position);
    }

    protected override void OnCantDrag()
    {
        base.OnCantDrag();
    }
}
