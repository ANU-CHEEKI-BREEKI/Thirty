using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/Skills", fileName = "SO_SSR_Skills")]
public class SOSquadSpawnerSkillsResourse : ScriptableObject
{
    [SerializeField] AnimationCurve skillLevelDependency;
    [SerializeField] SkillContainer[] skillsByLevel;
    public SkillStack SkillByLevel { get { return GetConsumable(skillsByLevel); } }

    SkillStack GetConsumable(SkillContainer[] equipments)
    {
        throw new NotImplementedException();
    }

    [Serializable]
    public class SkillContainer
    {
        [SerializeField] Skill[] randomConsumables;
    }
}
