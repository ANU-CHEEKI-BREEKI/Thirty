using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSpawner : MonoBehaviour
{
    
    Quaternion rotation;

    [SerializeField] Squad origin;

    [Header("Squad properties")]
    [SerializeField] Transform lookTarget;

    [Space]
    [SerializeField] FormationStats.Formations formation;
    [SerializeField] Inventory inventory;

    [Space]
    [SerializeField] Squad.UnitFraction fraction;

    [Header("AI properties")]
    [SerializeField] AiSquadController.AiSquadBehaviour mode;

    [Space]
    [SerializeField] bool useDefaultDistancesValues = true;
    [SerializeField] [Range(1, 200)] float distanceToActivateSquad = 50;
    [SerializeField] [Range(1, 50)] float radiusOfDefendArea = 15;
    [SerializeField] [Range(1, 50)] float radiusOfAttackArea = 30;

    [Space]
    [SerializeField] bool useDefaultTimeValues = true;
    [SerializeField] [Range(0, 1)] float slowApdateDeltaTime = 0.2f;
    [SerializeField] [Range(0, 5)] float attackDeltaTime = 1f;
    

    void Start ()
    {
        lookTarget = transform.GetChild(0);

        rotation = Quaternion.LookRotation(Vector3.forward, lookTarget.position - transform.position);

        Squad squad = Instantiate(origin) as Squad;

        squad.fraction = fraction;
        squad.PositionsTransform.position = transform.position;
        squad.PositionsTransform.rotation = rotation;
        squad.endLookRotation = rotation;

        if (inventory.Helmet != null)
            squad.Inventory.Helmet = inventory.Helmet;
        if (inventory.Body != null)
            squad.Inventory.Body = inventory.Body;
        if (inventory.Shield != null)
            squad.Inventory.Shield = inventory.Shield;
        if (inventory.Weapon != null)
            squad.Inventory.Weapon = inventory.Weapon;

        squad.CurrentFormation = formation;             
       
        AiSquadController controller = squad.GetComponent<AiSquadController>();
        if (controller != null)
        {
            controller.mode = mode;

            if (!useDefaultDistancesValues)
            {
                controller.distanceToActivateSquad = distanceToActivateSquad;
                controller.radiusOfDefendArea = radiusOfDefendArea;
                controller.radiusOfAttackArea = radiusOfAttackArea;
            }

            if (!useDefaultTimeValues)
            {
                controller.slowApdateDeltaTime = slowApdateDeltaTime;
                controller.attackDeltaTime = attackDeltaTime;
            }
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

        if (!useDefaultDistancesValues)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, distanceToActivateSquad);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radiusOfDefendArea);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radiusOfAttackArea);
        }
    }
}
