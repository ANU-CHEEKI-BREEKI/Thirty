using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/Skills", fileName = "SO_SSR_Skills")]
public class SOSquadSpawnerSkillsResourse : ScriptableObject
{
    [SerializeField] AnimationCurve skillChanseLevelDependency;
    public bool AllowOwnSkill
    {
        get
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            float val = skillChanseLevelDependency.Evaluate(t);
            return val > UnityEngine.Random.value;
        }
    }
    [Space]
    [SerializeField] AnimationCurve skillLevelDependency;
    [SerializeField] SkillContainer[] skillsByLevel;
    public SkillStack SkillByLevel { get { return GetConsumable(skillsByLevel); } }

    SkillStack GetConsumable(SkillContainer[] equipments)
    {
        SkillStack res = null;

        if (equipments.Length > 0)
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            float val = skillLevelDependency.Evaluate(t);
            int index = Mathf.RoundToInt(skillLevelDependency.Evaluate(t) * (equipments.Length - 1));
            int l2 = equipments[index].randomSkills.Length;
            var skill = equipments[index].randomSkills[UnityEngine.Random.Range(0, l2)];
            res = new SkillStack(skill, skill.DefaultStats);
        }

        return res;
    }

    [Serializable]
    public class SkillContainer
    {
        public Skill[] randomSkills;
    }
}
