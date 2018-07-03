using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSpawner : MonoBehaviour
{
    [SerializeField] Transform lookTarget;
    //----------------------
    //Все эти ScriptableObject'ы нужны лишь потому что сейчас нет вложенных префабов. И когда они будут я хз. Пускай уж так, чем никак.
    //----------------------
    [Space]
    [Space]
    [SerializeField] SOSquadSpawnerSquadPropertiesResourse squadProperties;
    [Space]
    [SerializeField] SOSquadSpawnerEquipmentResourse helmetEquipment;
    [SerializeField] SOSquadSpawnerEquipmentResourse bodyEquipment;
    [SerializeField] SOSquadSpawnerEquipmentResourse weaponEquipment;
    [SerializeField] SOSquadSpawnerEquipmentResourse shieldEquipment;
    [Space]
    [SerializeField] SOSquadSpawnerSkillsResourse skills;
    [SerializeField] SOSquadSpawnerConsumablesResourse consumables;
    [Space]
    [SerializeField] SOSquadSpawnerAISettingsResourse aiSettings;
    //----------------------
    Quaternion rotation;    

    void Start ()
    {
        lookTarget = transform.GetChild(0);
        rotation = Quaternion.LookRotation(Vector3.forward, lookTarget.position - transform.position);

        //squad prop
        Squad origin = squadProperties.SquadOrigin;
        Squad squad = Instantiate(origin) as Squad;
        if(!squadProperties.UseDefaultUnitCount)
            squad.FULL_SQUAD_UNIT_COUNT = squadProperties.UnitCount;
        squad.fraction = squadProperties.Fraction;
        squad.PositionsTransform.position = transform.position;
        squad.PositionsTransform.rotation = rotation;
        squad.EndLookRotation = rotation;

        var inv = squad.Inventory;
        //equipment
        if (helmetEquipment != null)
            inv.Helmet = helmetEquipment.EquipmentByLevel;
        if (bodyEquipment != null)
            inv.Body = bodyEquipment.EquipmentByLevel;
        if (weaponEquipment != null)
            inv.Weapon = weaponEquipment.EquipmentByLevel;
        if (shieldEquipment != null)
            inv.Shield = shieldEquipment.EquipmentByLevel;

        //skills
        if (skills != null)
        {
            if(skills.AllowOwnSkill)
                inv.FirstSkill = skills.SkillByLevel;
            else
                inv.FirstSkill = null;

            if (skills.AllowOwnSkill)
            {
                inv.SecondSkill = skills.SkillByLevel;
                if (inv.SecondSkill.Skill == inv.FirstSkill.Skill)
                    inv.SecondSkill = null;
            }
            else
                inv.SecondSkill = null;
        }

        //consumables
        if (consumables != null)
        {
            if (consumables.AllowOwnConsumable)
            {
                inv.FirstConsumable = consumables.ConsumableByLevel;
                inv.FirstConsumable.Count = squad.FULL_SQUAD_UNIT_COUNT;
            }
            else
                inv.FirstConsumable = null;

            if (consumables.AllowOwnConsumable)
            {
                inv.SecondConsumable = consumables.ConsumableByLevel;
                inv.SecondConsumable.Count = squad.FULL_SQUAD_UNIT_COUNT;
            }
            else
                inv.SecondConsumable = null;
        }

        //AI
        squad.CurrentFormation = aiSettings.StartFormation;
        AiSquadController controller = squad.GetComponent<AiSquadController>();
        if (controller != null)
        {
            controller.Mode = aiSettings.Mode;
            if(!aiSettings.UseDefaultDistancesOptions)
                controller.DistancesOptions = aiSettings.DistancesOptions;
            if (!aiSettings.UseDefaultReformOptions)
                controller.ReformOptions = aiSettings.ReformOptions;
        }

        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, lookTarget.position);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lookTarget.position, 1);

        if (aiSettings != null && !aiSettings.UseDefaultDistancesOptions)
        {
            var dOpt = aiSettings.DistancesOptions;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, dOpt.DistanceToActivateSquad);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, dOpt.RadiusOfDefendArea);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, dOpt.RadiusOfAttackArea);
        }
    }
}
