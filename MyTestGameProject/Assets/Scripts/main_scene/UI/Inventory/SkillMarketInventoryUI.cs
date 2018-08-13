using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillMarketInventoryUI : AInventoryUI
{
    public static SkillMarketInventoryUI Instance { get; private set; }

    [SerializeField] GameObject skillItemOriginal;

    [Header("Skills containers")]
    [SerializeField] Transform skillsContainer;
    [SerializeField] Color disabledColor;
    [SerializeField] Color selectedSkillCellColor;
    [Space]
    [SerializeField] Transform selectedSkillUpgradeContainer;
    [Space]
    [SerializeField] TextMeshProUGUI selectedSkillUpgradesPage;

    Color cellDefaultColor;

    /// <summary>
    /// Сами скилы и их прокачанные статы
    /// </summary>
    SkillStack[] skills;
    /// <summary>
    /// Ячейки для скилов (Слоты магазина)
    /// </summary>
    DropToSkillMarket[] skillCells;

    List<SkillsUpgrade> allUpgrades;

    DragSkill[] drags;

    int currentSelectedSkill;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var savedSkills = GameManager.Instance.PlayerProgress.Skills.skills;

        int cnt = skillsContainer.childCount;
        skillCells = new DropToSkillMarket[cnt];
        skills = new SkillStack[cnt];
        for (int i = 0; i < cnt; i++)
        {
            var t = skillsContainer.GetChild(i).GetComponent<DropToSkillMarket>();
            skillCells[i] = t;
            if (t.transform.childCount > 0)
            {
                var drag = t.transform.GetChild(0).GetComponent<DragSkill>();
                skills[i] = drag.SkillStack;

                if (savedSkills.Find(s => s.Id == skills[i].Skill.Id) == null)
                    savedSkills.Add(new DSPlayerSkill(skills[i].Skill.Id));

                drag.BeforeClick += BeforeDragClick;
            }
        }

        drags = new DragSkill[cnt];
        allUpgrades = new List<SkillsUpgrade>(cnt);

        cnt = selectedSkillUpgradeContainer.childCount;
        for (int i = 0; i < cnt; i++)
        {
            var su = selectedSkillUpgradeContainer.GetChild(i).GetComponent<SkillsUpgrade>();
            if (su != null)
            {
                allUpgrades.Add(su);
                su.gameObject.SetActive(false);
            }
        }

        cellDefaultColor = skillCells[0].GetComponent<Image>().color;

        currentSelectedSkill = allUpgrades[0].SkillId;

        RefreshUI();
    }

    override public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy)
            return;

        var savedSkills = GameManager.Instance.PlayerProgress.Skills.skills;

        int cnt = skillCells.Length;
        for (int i = 0; i < cnt; i++)
        {
            //загружаес сохранённые статы
            skills[i].SkillStats = skills[i].Skill.CalcUpgradedStats(
                    savedSkills.Find((t)=> { return t.Id == skills[i].Skill.Id; }).Upgrades
            );

            //
            Image img = null;
            GameObject item = null;
            DragSkill drag = null;
            //если в ячейке нет скила, то добавляем
            if(skillCells[i].transform.childCount == 0)
            {
                item = Instantiate(skillItemOriginal, skillCells[i].transform);
                drag = item.GetComponent<DragSkill>();
                drag.SkillStack.Skill = skills[i].Skill;
                drag.SkillStack.SkillStats = skills[i].SkillStats;

                drag.BeforeClick += BeforeDragClick;
            }
            //если уже есть скилл в ячейке, то вставляем туда нужную инфу
            else
            {
                drag = skillCells[i].transform.GetChild(0).GetComponent<DragSkill>();
                drag.SkillStack.Skill = skills[i].Skill;
                drag.SkillStack.SkillStats = skills[i].SkillStats;
            }
            img = drag.GetComponent<Image>();
            img.sprite = skills[i].Skill.MainPropertie.Icon;
            
            drags[i] = drag;

            if (drag.SkillStack.Skill.Id == currentSelectedSkill)
            {
                if (skillCells[i] != null)
                    skillCells[i].GetComponent<Image>().color = selectedSkillCellColor;

                if (selectedSkillUpgradesPage != null)
                    selectedSkillUpgradesPage.text = Localization.GetString(drag.SkillStack.Skill.MainPropertie.StringResourceName);
            }
            else
            {
                if (skillCells[i] != null)
                    skillCells[i].GetComponent<Image>().color = cellDefaultColor;
            }

            if (i < allUpgrades.Count && allUpgrades[i] != null)
                allUpgrades[i].gameObject.SetActive(false);

            bool unlocked = true;
            ISkillLockable sl = skills[i].SkillStats as ISkillLockable;
            if (sl != null)
                unlocked = sl.Unlocked;

            Executable first = Squad.playerSquadInstance.Inventory.FirstSkill.Skill;
            Executable second = Squad.playerSquadInstance.Inventory.SecondSkill.Skill;
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

        SkillsUpgrade su = allUpgrades.Find(s => s.SkillId == currentSelectedSkill);

        if (su != null)
        {
            su.gameObject.SetActive(true);

            var tr = su.transform as RectTransform;
            int q = tr.childCount;

            var vr = tr.GetChild(0) as RectTransform;
            Bounds b = new Bounds(vr.position, MainCanvas.Instance.ScreenToWorldPoint(vr.rect.size));
            for (int i = 0; i < q; i++)
            {
                var tc = tr.GetChild(i) as RectTransform;
                b.Encapsulate(new Bounds(
                    tc.position,
                    MainCanvas.Instance.ScreenToWorldPoint(tc.rect.size)
                ));
            }
            var l = selectedSkillUpgradeContainer.GetComponent<LayoutElement>();
            Vector2 size = MainCanvas.Instance.WorldToScreenPoint(b.size);
            l.preferredHeight = size.y + 100;
            l.preferredWidth = size.x + 100;           
        }
        else
        {
            var l = selectedSkillUpgradeContainer.GetComponent<LayoutElement>();
            l.preferredHeight = 0;
            l.preferredWidth = 0;
        }

    }

    bool BeforeDragClick(Drag drag)
    {
        bool res = false;
        int id = drag.GetComponent<DragSkill>().SkillStack.Skill.Id;

        if (id != currentSelectedSkill)
        {
            currentSelectedSkill = id;
            res = false;
            RefreshUI();
        }
        else
            res = true;

        return res;
    }
}
