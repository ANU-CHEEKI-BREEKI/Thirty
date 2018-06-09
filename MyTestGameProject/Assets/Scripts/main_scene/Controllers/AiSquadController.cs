using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AiSquadController : MonoBehaviour
{
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

    List<Vector3> path;
    Vector2 startFindPath;
    Vector2 endFindPath;

    Vector2 movePosition;
    Quaternion lookRotation;

    bool canGo;
    
    void Start()
    {
        ground = GameObject.FindWithTag("Ground").GetComponent<Ground>();

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
            lookRotation = Quaternion.LookRotation(
                Vector3.forward,
                playerSquad.CenterSquad - squad.CenterSquad
            );

            canGo = true;
        }
        else
        {
            if (Vector2.Distance(squad.PositionsTransform.position, startPosition) > 3f)
            {
                movePosition = startPosition;
                lookRotation = Quaternion.LookRotation(
                    Vector3.forward,
                    startPosition - squad.PositionsTransform.position
                );

                canGo = true;
            }
            else
                canGo = false;
        }

        GoTo();
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

    void GoTo()
    {
        if (canGo)
        {
            if (!pathfindingIsRunning)
            {
                SetStartPosition();

                startFindPath = new Vector2(
                        Mathf.Round(squad.PositionsTransform.position.x / MapBlock.BLOCK_SCALE),
                        Mathf.Round(squad.PositionsTransform.position.y / MapBlock.BLOCK_SCALE)
                    );
                endFindPath = new Vector2(
                    Mathf.Round(movePosition.x / MapBlock.BLOCK_SCALE),
                    Mathf.Round(movePosition.y / MapBlock.BLOCK_SCALE)
                );

                if (endFindPath.y >= 0 && endFindPath.y < ground.RowCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE / MapBlock.BLOCK_SCALE &&
                    endFindPath.x >= 0 && endFindPath.x < ground.ColCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE / MapBlock.BLOCK_SCALE)
                {
                    if (!ground.Grid[(int)endFindPath.y][(int)endFindPath.x])
                    {
                        RaycastHit2D rhit = Physics2D.Linecast(squad.PositionsTransform.position, movePosition, directFindPathLayers.value);

                        if (rhit.collider == null)
                        {
                            path = new List<Vector3>();
                            path.Add(squad.transform.position);
                            path.Add(playerSquad.PositionsTransform.position);

                            squad.SetEndMovePositions(movePosition, lookRotation);

                            squad.GoTo(path);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(FindPath);
                        }
                    }
                }
            }
        }
    }

    private void SetStartPosition()
    {
        //
        var angle = Quaternion.LookRotation(Vector3.forward, movePosition - squad.CenterSquad).eulerAngles.z;

        float angleLook = squad.PositionsTransform.rotation.eulerAngles.z;
        if (squad.FlipRotation) angleLook += 180;

        var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        var sin2 = Mathf.Sin(angleLook * Mathf.Deg2Rad);

        if (sin * sin2 < 0)
        {
            RaycastHit2D rhit = Physics2D.CircleCast(
                squad.CenterSquad,
                0.1f,
                Vector2.zero,
                0,
                1 << LayerMask.NameToLayer("MAP")
            );

            if (!rhit)
                squad.PositionsTransform.position = squad.CenterSquad;
        }
    }

    void FindPath(System.Object o)
    {
        pathfindingIsRunning = true;

        path = Labirinth.FindPathLee(ground.Grid, startFindPath, endFindPath, MapBlock.BLOCK_SCALE);

        if (path != null)
            squad.SetEndMovePositions(movePosition, lookRotation);

        squad.GoTo(path);

        pathfindingIsRunning = false;
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
    }

    public enum AiSquadBehaviour { HOLD_POSITION, ATTACK, DEFEND, }
}
