using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsUpgrade : MonoBehaviour
{
    [SerializeField] int skillId;
    public int SkillId { get { return skillId; } }

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

        var skills = GameManager.Instance.SavablePlayerData.PlayerProgress.Skills.skills;


        bool hasSkill = false;
        //ищем в созранениях информацию о прокачке скиллов
        foreach (var item in skills)
        {
            if(item.Id == skillId)
            {
                upgrades = item.Upgrades;
                hasSkill = true;
                break;
            }
        }
        //если в сохранениях нет прокачки для этого скилла, то добавляем новую информацию,
        //о непрокачанном скиле
        if (!hasSkill)
        {
            var newSkillUps = new DSPlayerSkill(skillId);
            skills.Add(newSkillUps);
            upgrades = newSkillUps.Upgrades;
        }

        //проходим по соответствующим кнопкам и инициализируем их
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
