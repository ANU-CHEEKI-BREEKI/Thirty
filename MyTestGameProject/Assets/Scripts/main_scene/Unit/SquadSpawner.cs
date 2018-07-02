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
    [SerializeField] SOSquadSpawnerEquipmentResourse equipment;
    [SerializeField] SOSquadSpawnerSkillsResourse skills;
    [SerializeField] SOSquadSpawnerConsumablesResourse consumables;
    [Space]
    [SerializeField] SOSquadSpawnerAISettingsResourse aiSettings;
    //----------------------
    Quaternion rotation;    

    void Start ()
    {
        //lookTarget = transform.GetChild(0);

        //rotation = Quaternion.LookRotation(Vector3.forward, lookTarget.position - transform.position);

        //if (origin == null)
        //    origin = (Resources.Load("Prefabs/Squads/EnemySquad") as GameObject).GetComponent<Squad>();

        //Squad squad = Instantiate(origin) as Squad;

        //squad.fullSquadUnitCount = unitCount;
        //squad.fraction = fraction;
        //squad.PositionsTransform.position = transform.position;
        //squad.PositionsTransform.rotation = rotation;
        //squad.EndLookRotation = rotation;


        //пока  что код убрал, так как в префабах все ссылки похерились. при пересоздании уровней опять откоментирую код.

        //if (inventory.Helmet != null)
        //   squad.Inventory.Helmet = inventory.Helmet;
        //if (inventory.Body != null)
        //    squad.Inventory.Body = inventory.Body;
        //if (inventory.Shield != null)
        //    squad.Inventory.Shield = inventory.Shield;
        //if (inventory.Weapon != null)
        //    squad.Inventory.Weapon = inventory.Weapon;

        //squad.CurrentFormation = formation;             
       
        //AiSquadController controller = squad.GetComponent<AiSquadController>();
        //if (controller != null)
        //{
        //    controller.mode = mode;

        //    if (!useDefaultDistancesValues)
        //    {
        //        controller.distanceToActivateSquad = distanceToActivateSquad;
        //        controller.radiusOfDefendArea = radiusOfDefendArea;
        //        controller.radiusOfAttackArea = radiusOfAttackArea;
        //    }

        //    if (!useDefaultTimeValues)
        //    {
        //        controller.slowApdateDeltaTime = slowApdateDeltaTime;
        //        controller.attackDeltaTime = attackDeltaTime;
        //    }
        //}

        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, lookTarget.position);

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, 2);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(lookTarget.position, 1);

        //if (!aiSettings.UseDefaultDistancesOptions)
        //{
        //    var dOpt = aiSettings.DistancesOptions;

        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, dOpt.DistanceToActivateSquad);

        //    Gizmos.color = Color.green;
        //    Gizmos.DrawWireSphere(transform.position, dOpt.RadiusOfDefendArea);

        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawWireSphere(transform.position, dOpt.RadiusOfAttackArea);
        //}
    }
}
