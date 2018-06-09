using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsUpgrade : MonoBehaviour
{
    [SerializeField] int skillId;

    List<DSPlayerSkill.SkillUpgrade> upgrades;
    List<SkillUpgradeButton> buttons = new List<SkillUpgradeButton>();
    public List<SkillUpgradeButton> Buttons { get { return buttons; } }

    void Start ()
    {
        //для правильной инициализации каждой кнопки, нужно инициализировать от начала дерева до листьев
        //по этому в иерархии трансформа кони должны идти в порядке: сначала родительский апгрейд, потом дочерний
        //соответственно надо отсортировать кнопки в списке по id апгрейда
        //так как дальше будет цикл от начала списка до конца
        Buttons.Sort();

        var skills = GameManager.Instance.PlayerProgress.skills.skills;

        bool hasSkill = false;
        foreach (var item in skills)
        {
            if(item.Id == skillId)
            {
                upgrades = item.Upgrades;
                hasSkill = true;
                break;
            }
        }
        if (!hasSkill)
            skills.Add(new DSPlayerSkill(skillId));

        int cnt = Buttons.Count;
        for(int i = 0; i < cnt; i++)
        {
            bool load = true;
            var upgrade = upgrades.Find((u) => { return u.Id == Buttons[i].Id; });
            if (upgrade == null)
            {
                upgrade = new DSPlayerSkill.SkillUpgrade(Buttons[i].Id);
                upgrades.Add(upgrade);
                load = false;
            }
            Buttons[i].Initiate(upgrade, load);
        }
	}

}
