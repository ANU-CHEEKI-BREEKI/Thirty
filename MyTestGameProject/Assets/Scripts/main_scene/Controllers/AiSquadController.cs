using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AiSquadController : MonoBehaviour
{
    public enum AiSquadBehaviour { HOLD_POSITION, ATTACK, DEFEND }

    //слои, через которые не будет искаться путь напрямую
    [SerializeField] LayerMask directFindPathLayers;

    [Range(0, 1)] public float slowApdateDeltaTime = 0.2f;
    [Range(0, 5)] public float attackDeltaTime = 1f;
    [Range(1, 200)] public float distanceToActivateSquad = 50;
    [Range(1, 50)] public float radiusOfDefendArea = 15;
    [Range(1, 50)] public float radiusOfAttackArea = 30;
    public  AiSquadBehaviour mode = AiSquadBehaviour.DEFEND;

    [Space]
    [SerializeField] [Range(0, 20)] float distToResetStartPosBeforeFindPath = 5f;

    Ground ground;

    Squad squad;
    Squad playerSquad;

    GameObject squadPositions;
    GameObject squadUnits;

    float distanceToPlayer;
    float distanceToStartPosition;
    float distPlayerSquadToStartPos;
    

    Vector3 startPosition;

    bool attackPlayer = false;

    bool pathfindingIsRunning = false;

    Vector2 movePosition;
    Vector2 lookPosition;

    bool canGo;
    
    void Start()
    {
        ground = Ground.Instance;

        squad = gameObject.GetComponent<Squad>();
        startPosition = squad.PositionsTransform.position;

        squadPositions = squad.PositionsTransform.gameObject;
        squadUnits = squad.UnitsContainer;

        playerSquad = GameObject.FindWithTag("Player").GetComponent<Squad>();

        StartCoroutine(SlowUpdate());
        StartCoroutine(Attack());

        squad.OnSquadDestroy += Squad_OnSquadDestroy;
    }

    private void Squad_OnSquadDestroy()
    {
        Destroy(this);
    }

    void OnDestroy()
    {
        squad.OnSquadDestroy -= Squad_OnSquadDestroy;
    }

    IEnumerator SlowUpdate()
    {
        while (true)
        {
            if (playerSquad != null)
            {
                CalcDistances();
                ActivateSquad();
                SquadBehaviour();
            }

            yield return new WaitForSeconds(slowApdateDeltaTime);
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            if (playerSquad != null)
                SquadAttack();

            yield return new WaitForSeconds(attackDeltaTime);
        }
    }

    private void ActivateSquad()
    {
        if (distanceToPlayer <= distanceToActivateSquad)
            SetActiveSquad(true);
        else
            SetActiveSquad(false);
    }

    private void CalcDistances()
    {
        distanceToPlayer = Vector2.Distance(squad.PositionsTransform.position, playerSquad.CenterSquad);
        distanceToStartPosition = Vector2.Distance(squad.PositionsTransform.position, startPosition);

        distPlayerSquadToStartPos = Vector2.Distance(startPosition, playerSquad.CenterSquad);
    }

    private void SquadAttack()
    {
        if (attackPlayer)
        {
            movePosition = playerSquad.CenterSquad;
            lookPosition = playerSquad.CenterSquad;
            canGo = true;
        }
        else
        {
            if (Vector2.Distance(squad.PositionsTransform.position, startPosition) > 3f)
            {
                movePosition = startPosition;
                lookPosition = startPosition;

                canGo = true;
            }
            else
                canGo = false;
        }

        Move();
    }

    private void SquadBehaviour()
    {
        if (squad.enabled)
        {
            switch (mode)
            {
                case AiSquadBehaviour.HOLD_POSITION:
                    BehaviorOnHold();
                    break;
                case AiSquadBehaviour.ATTACK:
                    BehaviorOnAttack();
                    break;
                case AiSquadBehaviour.DEFEND:
                    BehaviorOnDefend();
                    break;
            }
        }
    }

    void SetActiveSquad(bool newValue)
    {
        if (squad.enabled != newValue)
        {
            squad.enabled = newValue;

            squadPositions.SetActive(newValue);
            squadUnits.SetActive(newValue);
        }
    }

    void Move()
    {
        if (canGo)
        {
            squad.Controller.MoveToPoint(movePosition);
            squad.Controller.RotateAfterMoving(Quaternion.LookRotation(Vector3.forward, (Vector3)(playerSquad.CenterSquad - squad.CenterSquad)));
        }
    }
       
    private void BehaviorOnHold()
    {

    }

    private void BehaviorOnAttack()
    {
        if (distanceToPlayer <= radiusOfAttackArea)
            attackPlayer = true;
    }

    private void BehaviorOnDefend()
    {
        if (distPlayerSquadToStartPos > radiusOfAttackArea)
            attackPlayer = false;

        if (distanceToStartPosition <= radiusOfDefendArea)
        {
            if (distPlayerSquadToStartPos <= radiusOfDefendArea)
                attackPlayer = true;
        }
        else if (distanceToStartPosition <= radiusOfAttackArea)
        {
            if (distPlayerSquadToStartPos <= radiusOfAttackArea)
                attackPlayer = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (squad != null)
        {
            if (squad.enabled)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(startPosition, radiusOfDefendArea);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(startPosition, radiusOfAttackArea);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(squad.PositionsTransform.position, distanceToActivateSquad);
            }
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radiusOfDefendArea);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radiusOfAttackArea);
        }
    }
}
