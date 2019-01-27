using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Это скрипт-костыль для обучения в магазине
/// </summary>
public class DisableOnPlayerInvSkillChanged : MonoBehaviour
{
    void Start()
    {
        try
        {
            Squad.playerSquadInstance.Inventory.FirstSkill.OnSkillChanged += OnSkillChanged;
            Squad.playerSquadInstance.Inventory.SecondSkill.OnSkillChanged += OnSkillChanged;
        }
        catch { }
    }

    void OnDestroy()
    {
        try
        {
            Squad.playerSquadInstance.Inventory.FirstSkill.OnSkillChanged -= OnSkillChanged;
            Squad.playerSquadInstance.Inventory.SecondSkill.OnSkillChanged -= OnSkillChanged;
        }
        catch { }
    }

    private void OnSkillChanged(Executable obj)
    {
        if (obj != null)
            gameObject.SetActive(false);
    }
}
