using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsUpgrade : MonoBehaviour
{
    [SerializeField] int skillId;
    public int SkillId { get { return skillId; } }

    List<DSPlayerSkill.SkillUpgrade> upgrades;
    public List<SkillUpgradeButton> Buttons { get; } = new List<SkillUpgradeButton>();
    SkillUpgradeButton startButton;

    void Start ()
    {
        var skills = GameManager.Instance.SavablePlayerData.PlayerProgress.Skills.skills;

        //ищем в сохранениях информацию о прокачке данного скила
        var skill = skills.Find(s => s.Id == skillId);

        //если в сохранениях нет прокачки для этого скилла, то добавляем новую информацию,
        //о непрокачанном скиле
        if (skill == null)
        {
            var newSkillUps = new DSPlayerSkill(skillId);
            skills.Add(newSkillUps);
            upgrades = newSkillUps.Upgrades;
        }
        else
        {
            upgrades = skill.Upgrades;
        }

        //находим начальную кнопку
        startButton = Buttons.Find(b => b.UpgradeStats.isUpgradeToUnlock);
        if (startButton == null)
            throw new System.Exception("Тут короч нет начальной кнопки. Скорее всего, в редакторе что то испортилось... не не факт");

        //проходим по соответствующим кнопкам и инициализируем их               
        bool loaded = true;
        var upgrade = upgrades.Find((u) => { return u.Id == startButton.Id; });
        if (upgrade == null)
        {
            upgrade = new DSPlayerSkill.SkillUpgrade(startButton.Id);
            upgrades.Add(upgrade);
            loaded = false;
        }
        IntButtonsReursive(startButton, upgrade, loaded);
	}

    void IntButtonsReursive(SkillUpgradeButton btn, DSPlayerSkill.SkillUpgrade upgrade, bool isLoadedData)
    {
        btn.Initiate(upgrade, isLoadedData);
        foreach (var nextB in btn.nextButton)
        {
            bool loaded = true;
            var up = upgrades.Find((u) => { return u.Id == nextB.Id; });
            if (up == null)
            {
                up = new DSPlayerSkill.SkillUpgrade(nextB.Id);
                upgrades.Add(up);
                loaded = false;
            }
            IntButtonsReursive(nextB, up, loaded);
        }
    }

}
