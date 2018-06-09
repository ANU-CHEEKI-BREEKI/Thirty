using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMarketInventoryUI : AInventoryUI
{
    public static SkillMarketInventoryUI Instance { get; private set; }

    [SerializeField] GameObject skillItemOriginal;

    [Header("Skills containers")]
    [SerializeField] Transform skillsContainer;
    [SerializeField] Color disabledColor;

    /// <summary>
    /// Сами скилы и их прокачанные статы
    /// </summary>
    SkillStack[] skills;
    /// <summary>
    /// Ячейки для сколов (Слоты магазина)
    /// </summary>
    DropToSkillMarket[] skillCells;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int cnt = skillsContainer.childCount;
        skillCells = new DropToSkillMarket[cnt];
        skills = new SkillStack[cnt];
        for (int i = 0; i < cnt; i++)
        {
            var t = skillsContainer.GetChild(i).GetComponent<DropToSkillMarket>();
            skillCells[i] = t;
            if(t.transform.childCount > 0)
                skills[i] = t.transform.GetChild(0).GetComponent<DragSkill>().SkillStack;
        }

        RefreshUI();
    }

    override public void RefreshUI()
    {
        if (!gameObject.activeSelf)
            return;

        var savedSkills = GameManager.Instance.PlayerProgress.skills.skills;

        int cnt = skillCells.Length;
        for (int i = 0; i < cnt; i++)
        {
            skills[i].SkillStats = skills[i].Skill.CalcUpgradedStats(
                    savedSkills.Find((t)=> { return t.Id == skills[i].Skill.Id; }).Upgrades
            );

            Image img = null;
            GameObject item = null;
            DragSkill drag = null;
            if(skillCells[i].transform.childCount == 0)
            {
                item = Instantiate(skillItemOriginal, skillCells[i].transform);
                drag = item.GetComponent<DragSkill>();
                drag.SkillStack.Skill = skills[i].Skill;
                drag.SkillStack.SkillStats = skills[i].SkillStats;
            }
            else
            {
                drag = skillCells[i].transform.GetChild(0).GetComponent<DragSkill>();
                drag.SkillStack.Skill = skills[i].Skill;
                drag.SkillStack.SkillStats = skills[i].SkillStats;
            }
            img = drag.GetComponent<Image>();
            img.sprite = skills[i].Skill.MainPropertie.Icon;

            bool unlocked = true;
            ISkillLockable sl = skills[i].SkillStats as ISkillLockable;
            if (sl != null)
                unlocked = sl.Unlocked;


            Skill first = Squad.playerSquadInstance.Inventory.FirstSkill.Skill;
            Skill second = Squad.playerSquadInstance.Inventory.SecondSkill.Skill;
            //  если такой скилл уже взят как первый скилл       или     если такой скилл уже взят как второй скилл    или   самый первый апгрейд не вкачан
            if ((first!= null && skills[i].Skill.Id == first.Id) || (second != null && skills[i].Skill.Id == second.Id) || !unlocked)
            {
                drag.CanDrag = false;
                img.color = disabledColor;

                if(first != null && skills[i].Skill.Id == first.Id)
                {
                    Squad.playerSquadInstance.Inventory.FirstSkill.SkillStats = skills[i].SkillStats;
                }
                else if(second != null && skills[i].Skill.Id == second.Id)
                {
                    Squad.playerSquadInstance.Inventory.SecondSkill.SkillStats = skills[i].SkillStats;
                }

                if (SquadSkillsInventoryUI.Instance != null)
                    SquadSkillsInventoryUI.Instance.RefreshUI();
            }
            else
            {
                drag.CanDrag = true;
                img.color = Color.white;
            }
        }
    }

    //public override GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    //{
    //    var go = base.SetImage(origin, cell, stack, canDrag);
    //    if (go != null)
    //    {
    //        var st = stack as SkillStack;
    //        var drag = go.GetComponent<DragSkill>();
    //        var img = go.GetComponent<Image>();

    //    }
    //    return go;
    //}
}
