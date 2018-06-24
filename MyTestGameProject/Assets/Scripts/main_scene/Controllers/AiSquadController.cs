using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AiSquadController : MonoBehaviour
{
    public enum AiSquadBehaviour { HOLD, ATTACK, DEFEND }

    [Range(0, 1)] public float slowApdateDeltaTime = 0.2f;
    [Range(0, 5)] public float attackDeltaTime = 1f;    
    [Range(1, 200)] public float distanceToActivateSquad = 50;
    [Range(1, 50)] public float radiusOfDefendArea = 15;
    [Range(1, 50)] public float radiusOfAttackArea = 30;
    public  AiSquadBehaviour mode = AiSquadBehaviour.DEFEND;

    [Space]
    [Tooltip("Задержка перед перестроением")]
    [SerializeField] float reformLatency = 0.07f;
    [SerializeField] bool canReformRanks = true;
    [Tooltip("Время невозможности перестроения после перестроения свободным строем")]
    [SerializeField] float cooldownAfterReformToRanks = 1f;
    [SerializeField] bool canReformPhalanx = true;
    [Tooltip("Время невозможности перестроения после перестроения фалангой")]
    [SerializeField] float cooldownAfterReformToPhalanx = 1f;
    [SerializeField] bool canReformShields = true;
    [Tooltip("Время невозможности перестроения после перестроения черепахой")]
    [SerializeField] float cooldownAfterReformToShields = 1f;

    [Space]
    [SerializeField] [Range(0, 20)] float distToResetStartPosBeforeFindPath = 5f;

    Ground ground;

    Squad squad;
    Squad playerSquad;
    public Squad ConstrolledSquad { get { return squad; } }
    public Squad TargetSquad { get { return playerSquad; } }

    GameObject squadPositions;
    GameObject squadUnits;

    float distanceToPlayer;
    float distanceToStartPosition;
    float distPlayerSquadToStartPos;
    public float DistanceToPlayer
    {
        get
        {
            return distanceToPlayer;
        }
    }
    public float DistanceToStartPosition
    {
        get
        {
            return distanceToStartPosition;
        }
    }
    public float DistPlayerSquadToStartPos
    {
        get
        {
            return distPlayerSquadToStartPos;
        }
    }

    Vector3 startPosition;

    bool attackPlayer = false;
    public bool AttackPlayer { get { return attackPlayer; } }

    bool pathfindingIsRunning = false;

    Vector2 movePosition;
    Vector2 lookPosition;

    bool canGo;

    List<AExecutableBehaviour> allExeBehaviour;
    List<AExecutableBehaviour> exeBehaviourToRemove;

    void Start()
    {
        ground = Ground.Instance;

        squad = gameObject.GetComponent<Squad>();
        startPosition = squad.PositionsTransform.position;

        squadPositions = squad.PositionsTransform.gameObject;
        squadUnits = squad.UnitsContainer;

        playerSquad = GameObject.FindWithTag("Player").GetComponent<Squad>();

        // StartCoroutine(UpdateBehaviour());
        //StartCoroutine(UpdateAttack());

        squad.OnSquadDestroy += Squad_OnSquadDestroy;

        allExeBehaviour = new List<AExecutableBehaviour>();
        exeBehaviourToRemove = new List<AExecutableBehaviour>();
        var inv = squad.Inventory;
        allExeBehaviour.Add(ExecutableBehaviourFactory.GetBehaviour(inv.FirstSkill, this));
        allExeBehaviour.Add(ExecutableBehaviourFactory.GetBehaviour(inv.SecondSkill, this));
        allExeBehaviour.Add(ExecutableBehaviourFactory.GetBehaviour(inv.FirstConsumable, this));
        allExeBehaviour.Add(ExecutableBehaviourFactory.GetBehaviour(inv.SecondConsumable, this));
        allExeBehaviour.RemoveAll((e)=> { return e == null; });

        AExecutableBehaviour.DisposeMePlease += AExecutableBehaviour_DisposeMePlease;
    }

    private void AExecutableBehaviour_DisposeMePlease(AExecutableBehaviour exec)
    {
        exeBehaviourToRemove.Add(exec);
    }

    private void Squad_OnSquadDestroy()
    {
        Destroy(this);
    }

    void OnDestroy()
    {
        squad.OnSquadDestroy -= Squad_OnSquadDestroy;
    }

    float timerBehaviour = 0;
    float timerAttack = 0;
    Coroutine formationChangedCoroutine = null;



    void Update()
    {
        float delta = Time.deltaTime;
        timerBehaviour += delta;
        timerAttack += delta;


        if (timerBehaviour >= slowApdateDeltaTime)
        {
            timerBehaviour = 0;
            if (playerSquad != null)
            {
                CalcDistances();
                ActivateSquad();
                SquadBehaviour();

                foreach (var b in allExeBehaviour)
                    b.Behave();

                foreach (var b in exeBehaviourToRemove)
                    allExeBehaviour.Remove(b);
                exeBehaviourToRemove.Clear();
            }
        }

        if (timerAttack >= attackDeltaTime)
        {
            timerAttack = 0;
            SquadAttack();
        }

        SetFormation();
    }
      

    void SetFormation()
    {
        if (canGo)
        {
            if (distanceToPlayer > squad.Inventory.Weapon.EquipmentStats.AttackDistance + 3)
            {
                if (canReformRanks && formationChangedCoroutine == null && squad.CurrentFormation != FormationStats.Formations.RANKS)
                    formationChangedCoroutine = StartCoroutine(SetFormationLate(reformLatency, FormationStats.Formations.RANKS));
            }
            else
            {
                if (canReformPhalanx && formationChangedCoroutine == null && squad.CurrentFormation != FormationStats.Formations.PHALANX)
                    formationChangedCoroutine = StartCoroutine(SetFormationLate(reformLatency, FormationStats.Formations.PHALANX));
            }
        }
    }

    IEnumerator SetFormationLate(float latency, FormationStats.Formations formation)
    {
        yield return new WaitForSeconds(latency);
        squad.CurrentFormation = formation;

        float cooldown = 0;
        switch (formation)
        {
            case FormationStats.Formations.RANKS:
                cooldown = cooldownAfterReformToRanks;
                break;
            case FormationStats.Formations.PHALANX:
                cooldown = cooldownAfterReformToPhalanx;
                break;
            case FormationStats.Formations.RISEDSHIELDS:
                cooldown = cooldownAfterReformToShields;
                break;
        }

        formationChangedCoroutine = StartCoroutine(ColldownAfterReform(cooldown));
    }

    IEnumerator ColldownAfterReform(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        formationChangedCoroutine = null;
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
        distanceToPlayer = Vector2.Distance(squad.CenterSquad, playerSquad.CenterSquad);
        distanceToStartPosition = Vector2.Distance(squad.CenterSquad, startPosition);

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
                case AiSquadBehaviour.HOLD:
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
