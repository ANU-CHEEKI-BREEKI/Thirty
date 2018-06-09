using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DSPlayerSkill
{
    [SerializeField] int id;
    /// <summary>
    /// Идентификатор скила. Чтобы скил мог найти свои сохранения и загрузится.
    /// </summary>
    public int Id { get { return id; } }
    [SerializeField] List<SkillUpgrade> upgrades;
    public List<SkillUpgrade> Upgrades { get { return upgrades; } }

    public DSPlayerSkill(int id)
    {
        this.id = id;
        upgrades = new List<SkillUpgrade>();
    }

    [Serializable]
    public class SkillUpgrade : IComparable<DSPlayerSkill>
    {
        public SkillUpgrade(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// для UI. чтобы соответствующая кнопка знала за какой апгрейд она отвечает
        /// </summary>
        [SerializeField] int id;
        public int Id { get { return id; } set { id = value; } }
        public string FieldName;
        public float AdditionalValuePerLevel;
        public byte level;
        public bool isUpgradeToUnlock;
        public float AdditionalValue { get { return AdditionalValuePerLevel * level; } }

        public int CompareTo(DSPlayerSkill other)
        {
            return other.id - id;
        }
    }

   
}