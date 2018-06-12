using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadSkillsInventoryUI : AInventoryUI
{
    public static SquadSkillsInventoryUI Instance { get; private set; }

    [SerializeField] GameObject skillItemOriginal;

    [Header("Skills containers")]
    [SerializeField] Transform firstSkillCell;
    [SerializeField] Transform secondSkillCell;
    [SerializeField] bool canDrag = true;
    
    void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        RefreshUI();
    }

    override public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy)
            return;

        var squadSkillStack = Squad.playerSquadInstance.Inventory.FirstSkill;
        SetImage(skillItemOriginal, firstSkillCell, squadSkillStack, canDrag);

        squadSkillStack = Squad.playerSquadInstance.Inventory.SecondSkill;
        SetImage(skillItemOriginal, secondSkillCell, squadSkillStack, canDrag);
    }

    public override GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    {
        var go = base.SetImage(origin, cell, stack, canDrag);
        if (go != null)
        {
            var st = stack as SkillStack;
            var drag = go.GetComponent<DragSkill>();

            if (st.Skill != null)
            {
                drag.SkillStack = new SkillStack();
                drag.SkillStack.Skill = st.Skill;
                drag.SkillStack.SkillStats = st.SkillStats;
            }
        }
        return go;
    }
}
