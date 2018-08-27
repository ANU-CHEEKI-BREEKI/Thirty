using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AiSquadController : MonoBehaviour
{
    public enum AiSquadBehaviour { HOLD, ATTACK, DEFEND }

    [SerializeField] AiSquadBehaviour mode = AiSquadBehaviour.DEFEND;
    public AiSquadBehaviour Mode { get { return mode; } set { mode = value; } }

    [Space]
    [ContextMenuItem("Reset values", "ResetDistansesSettings")]
    [SerializeField] DistancesSettings distancesOptions;
    public DistancesSettings DistancesOptions { get { return distancesOptions; } set { distancesOptions = value; } }

    [Space]
    [ContextMenuItem("Reset values", "ResetReformSettings")]
    [SerializeField] ReformSettings reformOptions;
    public ReformSettings ReformOptions { get { return reformOptions; } set { reformOptions = value; } }

    
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

    float timerBehaviour = 0;
    float timerAttack = 0;
    Coroutine formationChangedCoroutine = null;

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

    
    
    void Update()
    {
        float delta = Time.deltaTime;
        timerBehaviour += delta;
        timerAttack += delta;


        if (timerBehaviour >= distancesOptions.SlowApdateDeltaTime)
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

        if (timerAttack >= distancesOptions.AttackDeltaTime)
        {
            timerAttack = 0;
            SquadAttack();
        }

        SetFormation();
    }
      
    void SetFormation()
    {
        //if (canGo)
        //{

        if (mode == AiSquadBehaviour.ATTACK)
        {
            if (distanceToPlayer > squad.Inventory.Weapon.EquipmentStats.AttackDistance + 3)
            {
                if (reformOptions.CanReformRanks && formationChangedCoroutine == null && squad.CurrentFormation != FormationStats.Formations.RANKS)
                    formationChangedCoroutine = StartCoroutine(SetFormationLate(reformOptions.ReformLatency, FormationStats.Formations.RANKS));
            }
            else
            {
                if (reformOptions.CanReformPhalanx && formationChangedCoroutine == null && squad.CurrentFormation != FormationStats.Formations.PHALANX)
                    formationChangedCoroutine = StartCoroutine(SetFormationLate(reformOptions.ReformLatency, FormationStats.Formations.PHALANX));
            }
        }
        else if(mode == AiSquadBehaviour.DEFEND)
        {
            if(attackPlayer)
            {
                if (reformOptions.CanReformPhalanx && formationChangedCoroutine == null && squad.CurrentFormation != FormationStats.Formations.PHALANX)
                    formationChangedCoroutine = StartCoroutine(SetFormationLate(reformOptions.ReformLatencyInDefendMode, FormationStats.Formations.PHALANX));
            }
            else
            {
                if (reformOptions.CanReformRanks && formationChangedCoroutine == null && squad.CurrentFormation != FormationStats.Formations.RANKS)
                    formationChangedCoroutine = StartCoroutine(SetFormationLate(reformOptions.ReformLatencyInDefendMode, FormationStats.Formations.RANKS));
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
                cooldown = reformOptions.CooldownAfterReformToRanks;
                break;
            case FormationStats.Formations.PHALANX:
                cooldown = reformOptions.CooldownAfterReformToPhalanx;
                break;
            case FormationStats.Formations.RISEDSHIELDS:
                cooldown = reformOptions.CooldownAfterReformToShields;
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
        if (distanceToPlayer <= distancesOptions.DistanceToActivateSquad)
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
        if (distanceToPlayer <= distancesOptions.RadiusOfAttackArea)
            attackPlayer = true;
    }

    private void BehaviorOnDefend()
    {
        if (distPlayerSquadToStartPos > distancesOptions.RadiusOfAttackArea)
            attackPlayer = false;

        if (distanceToStartPosition <= distancesOptions.RadiusOfDefendArea)
        {
            if (distPlayerSquadToStartPos <= distancesOptions.RadiusOfDefendArea)
                attackPlayer = true;
        }
        else if (distanceToStartPosition <= distancesOptions.RadiusOfAttackArea)
        {
            if (distPlayerSquadToStartPos <= distancesOptions.RadiusOfAttackArea)
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
                Gizmos.DrawWireSphere(startPosition, distancesOptions.RadiusOfDefendArea);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(startPosition, distancesOptions.RadiusOfAttackArea);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(squad.PositionsTransform.position, distancesOptions.DistanceToActivateSquad);
            }
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, distancesOptions.RadiusOfDefendArea);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, distancesOptions.RadiusOfAttackArea);
        }

        if(allExeBehaviour != null)
            foreach (var b in allExeBehaviour)
                if (b is IGizmosDrawable)
                    (b as IGizmosDrawable).OnDrawGizmos();
    }
    
    void ResetDistansesSettings()
    {
        var d = new DistancesSettings();
        d.Reset();
        distancesOptions = d;
    }

    void ResetReformSettings()
    {
        var r = new ReformSettings();
        r.Reset();
        reformOptions = r;
    }

    [Serializable]
    public struct DistancesSettings
    {
        [Range(0, 1)] [SerializeField] float slowApdateDeltaTime;
        public float SlowApdateDeltaTime { get { return slowApdateDeltaTime; } }

        [Range(0, 5)] [SerializeField] float attackDeltaTime;
        public float AttackDeltaTime { get { return attackDeltaTime; } }

        [Range(1, 200)] [SerializeField] float distanceToActivateSquad;
        public float DistanceToActivateSquad { get { return distanceToActivateSquad; } }

        [Range(1, 50)] [SerializeField] float radiusOfDefendArea;
        public float RadiusOfDefendArea { get { return radiusOfDefendArea; } }

        [Range(1, 50)] [SerializeField] float radiusOfAttackArea;
        public float RadiusOfAttackArea { get { return radiusOfAttackArea; } }
        
        public void Reset()
        {
            slowApdateDeltaTime = 0.2f;
            attackDeltaTime = 1f;
            distanceToActivateSquad = 50;
            radiusOfDefendArea = 15;
            radiusOfAttackArea = 30;
        }
    }

    [Serializable]
    public struct ReformSettings
    {
        [Tooltip("Задержка перед перестроением")]
        [SerializeField] float reformLatency;
        public float ReformLatency { get { return reformLatency; } }
        [Tooltip("Задержка перед перестроением в режиме защиты")]
        [SerializeField] float reformLatencyInDefendMode;
        public float ReformLatencyInDefendMode { get { return reformLatencyInDefendMode; } }

        [Space]
        [SerializeField] bool canReformRanks;
        public bool CanReformRanks { get { return canReformRanks; } }

        [Tooltip("Время невозможности перестроения после перестроения свободным строем")]
        [SerializeField] float cooldownAfterReformToRanks;
        public float CooldownAfterReformToRanks { get { return cooldownAfterReformToRanks; } }

        [Space]
        [SerializeField] bool canReformPhalanx;
        public bool CanReformPhalanx { get { return canReformPhalanx; } }

        [Tooltip("Время невозможности перестроения после перестроения фалангой")]
        [SerializeField] float cooldownAfterReformToPhalanx;
        public float CooldownAfterReformToPhalanx { get { return cooldownAfterReformToPhalanx; } }

        [Space]
        [SerializeField] bool canReformShields;
        public bool CanReformShields { get { return canReformShields; } }

        [Tooltip("Время невозможности перестроения после перестроения черепахой")]
        [SerializeField] float cooldownAfterReformToShields;
        public float CooldownAfterReformToShields { get { return cooldownAfterReformToShields; } }

        public void Reset()
        {
            reformLatency = 0.06f;
            canReformRanks = true;
            cooldownAfterReformToRanks = 1f;
            canReformPhalanx = true;
            cooldownAfterReformToPhalanx = 1f;
            canReformShields = true;
            cooldownAfterReformToShields = 1f;
        }
    }
}
