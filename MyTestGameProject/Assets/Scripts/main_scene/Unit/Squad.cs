using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Squad : MonoBehaviour
{
    public enum UnitFraction { ALLY, ENEMY, NEUTRAL }

    static public Squad playerSquadInstance;

    [SerializeField] Transform minimapMark;

    [Header("Default units properties in this squad")]
    [SerializeField] Unit unitOriginal;
    [Space]
    [SerializeField] UnitStats unitStats;//задавать статы ТОЛЬКО с инспектора. 
    public UnitStats DefaultUnitStats { get { return unitStats; } set { unitStats = value; } }
    /// <summary>
    /// При каждом вызове свойства происходит расчет. Будте аккуратны.
    /// </summary>
    public UnitStats UnitStats
    {
        get
        {
            EquipmentStats[] equipStats = new EquipmentStats[]
            {
                inventory.Helmet.EquipmentStats,
                inventory.Body.EquipmentStats,
                inventory.Shield.EquipmentStats,
                inventory.Weapon.EquipmentStats
            };
            var stats = UnitStats.CalcStats(unitStats, equipStats, currentFormationModifyers);

            foreach (var mod in statsModifyers)
                stats = UnitStats.ModifyStats(stats, mod);

            foreach (var mod in terrainStatsModifyers)
                stats = UnitStats.ModifyStats(stats, mod.GetModifierByEquipmentMass(stats.EquipmentMass));

            return stats;
        }
    }

    [Header("Squad default properties")]
    [SerializeField] [Range(1, 100)] int fullSquadUnitCount = 30;
    public int FULL_SQUAD_UNIT_COUNT { get { return fullSquadUnitCount; } set { fullSquadUnitCount = value; } }
    [SerializeField] [Range(1, 100)] int squadLength = 10;
    public int SQUAD_LENGTH { get { return squadLength; } set { squadLength = value; } }
    int squadRowsMax;
    public int SQUAD_ROWS_MAX {  get { return squadRowsMax; } }
    /// <summary>
    /// Радиус вогруг центра отряда, юниты за пределами которого не будут учтены в расчете положения центра отряда.
    /// </summary>
    [SerializeField] float distanceToCenterSquad = 10;
    [Space]
    [SerializeField] [Range(1, 10)] float maxSpeed = 4;
    public float MaxSpeed { get { return maxSpeed; } }
    [SerializeField] [Range(1, 180)] float maxRotationSpeed = 50;
    public float MaxRotationSpeed { get { return maxRotationSpeed; } }
    [SerializeField] [Range(1, 180)] float minRotationSpeed = 5;
    [Space]
    [SerializeField] [Range(0, 90)] float rotationAcuracy = 5;
    public float RotationAcuracy { get { return rotationAcuracy; } }
    [SerializeField] [Range(0, 1)] float positionAcuracy = 0.1f;
    public float PositionAcuracy { get { return positionAcuracy; } }
    [Space]
    [SerializeField] FormationStats.Formations formation = FormationStats.Formations.RANKS;
    public FormationStats.Formations CurrentFormation
    {
        get { return formation; }
        set
        {
            FormationStats.Formations lastFormation = formation;

            formation = value;

            //проверяем построение на которое пытаемся переключится
            switch (value)
            {
                case FormationStats.Formations.PHALANX:

                    if (!(inventory.Weapon).EquipmentStats.CanReformToPhalanx)
                    {
                        formation = lastFormation;

                        if (Squad.playerSquadInstance == this)
                            Toast.Instance.Show("[non local] с этим оружием нельзя перестроится в фалангу ");
                    }
                    else if (!inventory.Weapon.EquipmentStats.CanReformToPhalanxInFight && InFight)
                    {
                        formation = lastFormation;

                        if (Squad.playerSquadInstance == this)
                            Toast.Instance.Show("[non local] с этим оружием нельзя перестроится в фалангу в бою");
                    }
                    break;

                case FormationStats.Formations.RISEDSHIELDS:

                    if (inventory.Shield.EquipmentStats.Empty)
                    {
                        formation = lastFormation;

                        if(Squad.playerSquadInstance == this)
                            Toast.Instance.Show("[non local] без щитов нельзя перестроится \"черепахой\"");
                    }

                    break;
            }


            switch (formation)
            {
                case FormationStats.Formations.RANKS:
                    currentFormationModifyers = new FormationStats.Ranks();
                    break;

                case FormationStats.Formations.PHALANX:
                    currentFormationModifyers = new FormationStats.Phalanx();
                    break;

                case FormationStats.Formations.RISEDSHIELDS:
                    currentFormationModifyers = new FormationStats.RisedShields();
                    break;
            }
            
            SetProp();
           
            ReformSquad(FlipRotation);

            if(lastFormation != formation)
                if (OnFormationChanged != null)
                    OnFormationChanged(currentFormationModifyers);

            if (tag == "Player" && formation == FormationStats.Formations.PHALANX && PlayerSquadController.Instance != null)
                PlayerSquadController.Instance.DeselectEnemyes();
        }
    }
    FormationStats currentFormationModifyers = new FormationStats.Ranks();
    public FormationStats CurrentFormationModifyers { get { return currentFormationModifyers; } }
    public UnitFraction fraction = UnitFraction.ALLY;
    [Space]
    [SerializeField] [Range(0, 10)] float delayToGetOutOfTheFight = 1f;
    [Space]
    [SerializeField] [Range(0, 20)] float distanceToUnionWithSquad;
    public float DistanceToUnionWithSquad { get { return distanceToUnionWithSquad; } }
    [Space]
    [SerializeField] Inventory inventory;
    public Inventory Inventory { get { return inventory; } }
    [Header("Squad current properties")]
    [SerializeField] float currentSpeed = 4;
    public float CurrentSpeed { get { return currentSpeed; } }
    [SerializeField] float currentRotationSpeed = 50;
    public float CurrentRotationSpeed { get { return currentRotationSpeed; } }
    [Space]
    bool flipRotation = false;
    public bool FlipRotation
    {
        get { return flipRotation; }
        set
        {
            if (flipRotation != value)
            {
                flipRotation = value;
                FlipPhalanx();
            }
        }

    }

    bool hiding;
    public bool Hiding { get { return hiding; } }

    int attakingUnitsCount = 0;
    public int AttakingUnitsCount
    {
        get { return attakingUnitsCount; }
        set
        {
            attakingUnitsCount = value;
            if (attakingUnitsCount < 0) attakingUnitsCount = 0;
            if (attakingUnitsCount > UnitCount) attakingUnitsCount = UnitCount;
            SetFlagFight();
        }
    }
    int targettedUnitsCount = 0;
    public int TargettedUnitsCount
    {
        get { return targettedUnitsCount; }
        set
        {
            targettedUnitsCount = value;
            if (targettedUnitsCount < 0) targettedUnitsCount = 0;
            if (targettedUnitsCount > UnitCount) targettedUnitsCount = UnitCount;
            SetFlagFight();
        }
    }
    public event Action<bool> OnInFightFlagChanged;
    bool inFight;
    public bool InFight
    {
        get { return inFight; }
        private set
        {
            inFight = value;
            if (OnInFightFlagChanged != null)
                OnInFightFlagChanged(value);
        }
    }

    [Space]
    Vector2 centerSquad;
    public Vector2 CenterSquad { get { return centerSquad; } }
    [Space]
    [SerializeField] List<UnitPosition> unitPositions;
    /// <summary>
    /// Ни в коем случае не надо ничкго менять в позициях! Понял, йопта?!
    /// </summary>
    public List<UnitPosition> UnitPositions { get { return unitPositions; } }
    
    public GameObject UnitsContainer { get; private set; }
    public int UnitCount { get { return unitPositions.Count; } }

    float squadHelth;
    public float SquadHealth
    {
        get { return squadHelth; }
        set { squadHelth = value; if (OnSumHealthChanged != null) OnSumHealthChanged(value); }
    }

    List<Vector3> path = null;
    /// <summary>
    /// EndMovePosition нужно задавать ПОСЛЕ задания пути
    /// </summary>
    public List<Vector3> Path
    {
        get
        {
            return path;
        }

        set
        {
            path = value;
            pathStep = 1;
        }
    }
    public Quaternion EndLookRotation { get; set; }
    /// <summary>
    /// Нужно задавать ПОСЛЕ задания пути
    /// </summary>
    public Vector3 EndMovePosition
    {
        get
        {
            Vector3 res = Vector3.zero;
            if (path != null)
                res = path[path.Count - 1];
            return res;
        }
        set
        {
            if (path != null)
                path[path.Count - 1] = value;
        }
    }

    LineRenderer lineRenderer;

    int pathStep = 1;

    public event Action<int> OnUitCountChanged;
    public event Action<float> OnSumHealthChanged;
    public event Action<FormationStats> OnFormationChanged;
    public event Action<EquipmentStack> OnDropStackFromInventory;
    public event Action<FormationStats.Formations> OnReformSquad;
    public event Action OnInitiateUnitPositions;
    public event Action<UnitStatsModifier> OnBeginCharge;
    public event Action<UnitStatsModifier> OnEndCharge;

    /// <summary>
    /// старые статы и новые статы
    /// </summary>
    public event Action<UnitStats, UnitStats> OnUnitStatsChanged;

    public event Action<bool> OnDrawUnitHpChanged;

    public event Action OnSquadDestroy;

    public Transform PositionsTransform { get; private set; }

    bool drawUnitHp = false;
    public bool DrawUnitHp
    {
        get { return drawUnitHp; }
        set
        {
            bool old = drawUnitHp;
            drawUnitHp = value;
            if (OnDrawUnitHpChanged != null && old != drawUnitHp)
                OnDrawUnitHpChanged(value);
        }
    }

    bool charging = false;
    public bool Charging { get { return charging; } }

    float sumX = 0;
    float sumY = 0;
    int countSumUnit = 0;

    List<UnitStatsModifier> statsModifyers = new List<UnitStatsModifier>();
    public UnitStatsModifier[] StatsModifiers { get { return statsModifyers.ToArray(); } }
    List<SOTerrainStatsModifier> terrainStatsModifyers = new List<SOTerrainStatsModifier>();
    public SOTerrainStatsModifier[] TerrainStatsModifiers { get { return terrainStatsModifyers.ToArray(); } }
    Dictionary<UnitStatsModifier, int> statsDictionary = new Dictionary<UnitStatsModifier, int>();
    Dictionary<SOTerrainStatsModifier, int> terrainStatsDictionary = new Dictionary<SOTerrainStatsModifier, int>();
    public event Action<UnitStatsModifier> OnCallApplyModifierToAllUnit;
    public event Action<UnitStatsModifier> OnCallRejectModifierToAllUnit;
    public event Action<SOTerrainStatsModifier> OnCallApplyTerrainModifierToAllUnit;
    public event Action<SOTerrainStatsModifier> OnCallRejectTerrainModifierToAllUnit;
    public event Action<UnitStatsModifier[]> OnModifiersListChanged;
    public event Action<SOTerrainStatsModifier[]> OnTerrainModifiersListChanged;
    /// <summary>
    /// Процент юнитов в отряде от общего кол-ва, которые должны иметь модификатор, чтобы он применился ко всему отряду
    /// </summary>
    public const float percentUnitCountToApplyModifier = 0.2f;
    int UnitCountToApplyModifier
    {
        get
        {
            var val = Mathf.RoundToInt(percentUnitCountToApplyModifier * UnitCount);
            if (val < 1) val = 1;
            return val;
        }
    }

    /// <summary>
    /// Контроллер перемещения отряда. Ищет путь и задает его. И т.п.
    /// </summary>
    public SquadController Controller { get; private set; }

    void Awake()
    {
        if (this.tag == "Player")
        {
            if (playerSquadInstance == null)
            {
                playerSquadInstance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.sortingLayerName = "Ground";

        UnitsContainer = transform.Find("Units").gameObject;
        UnitsContainer.layer = LayerMask.NameToLayer(fraction.ToString());
        PositionsTransform = transform.Find("UnitPositions");
        inventory.Squad = this;
        squadRowsMax = fullSquadUnitCount / squadLength;        
        unitPositions = new List<UnitPosition>(FULL_SQUAD_UNIT_COUNT);
        squadHelth = 0;
    }

    void Start()
    {
        Controller = new SquadController(this);

        EndLookRotation = PositionsTransform.rotation;
        LookWithoutFullRotation(EndLookRotation);

        for (int i = 0; i < FULL_SQUAD_UNIT_COUNT; i++)
        {
            Unit unit = Instantiate(
                unitOriginal,
                new Vector3(PositionsTransform.position.x, PositionsTransform.position.y, PositionsTransform.position.z),
                Quaternion.identity
            );
            unit.name = "Unit " + (i + 1);
            unit.delayToFindTargetAndAttack = (float)i / FULL_SQUAD_UNIT_COUNT;
            AddUnitWithoutReformSquad(unit);
        }

        CurrentFormation = CurrentFormation;
        if (OnFormationChanged != null)
            OnFormationChanged(CurrentFormationModifyers);

        if (minimapMark != null)
            minimapMark.gameObject.SetActive(true);

        inventory.OnEquipmentChanged += SetProp;
        OnModifiersListChanged += Squad_OnModifiersListChanged;
        OnTerrainModifiersListChanged += Squad_OnTerrainModifiersListChanged;

        SetProp(inventory.Weapon);
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #region Stats Modifiers

    void UnitModifierAdded(UnitStatsModifier modifyer)
    {
        if (statsDictionary.ContainsKey(modifyer))
            statsDictionary[modifyer]++;
        else
            statsDictionary.Add(modifyer, 1);

        if (statsDictionary[modifyer] >= UnitCountToApplyModifier && !statsModifyers.Contains(modifyer))
            AddStatsModifier(modifyer);
    }
    
    void UnitModifierRemoved(UnitStatsModifier modifyer)
    {
        if (!statsDictionary.ContainsKey(modifyer))
            return;

        statsDictionary[modifyer]--;

        if (statsDictionary[modifyer] < UnitCountToApplyModifier && statsModifyers.Contains(modifyer))
            RemoveStatsModifier(modifyer);

        if (statsDictionary.ContainsKey(modifyer) && statsDictionary[modifyer] <= 0)
            statsDictionary.Remove(modifyer);
    }

    public void AddStatsModifier(UnitStatsModifier modifier)
    {
        if (!statsModifyers.Contains(modifier))
        {
            statsModifyers.Add(modifier);

            if(OnCallApplyModifierToAllUnit != null)
                OnCallApplyModifierToAllUnit(modifier);

            if (OnModifiersListChanged != null)
                OnModifiersListChanged(statsModifyers.ToArray());

            Debug.Log("было вызвано событие для применения модиф. ко всему отряду");
        }
        else
            Debug.Log("Модификатор уже существует!");
    }

    /// <summary>
    /// Удаляет модификаток, эквивалентный переданому, и отменяэт его действия с юнитoB
    /// </summary>
    /// <param name="modifier"></param>
    public void RemoveStatsModifier(UnitStatsModifier modifier)
    {
        if (statsModifyers.Remove(modifier))
        {
            // ТУТ НАДО БЫЛО БЫ НЕ У ВСЕХ ОТНИМАТЬ У ТОЛЬКО У ТЕХ, НА КОГО НЕ ДОЛЖНЫ ДЕЙСТВОВАТЬ. НО Я ПОКА ХЗ КАК, ЧТОБЫ ОПТИМИЗИРОВАННО БЫЛО
            if (OnCallRejectModifierToAllUnit != null)
                OnCallRejectModifierToAllUnit(modifier);

            if (OnModifiersListChanged != null)
                OnModifiersListChanged(statsModifyers.ToArray());

            Debug.Log("было вызвано событие для отмены модиф. ко всему отряду");
        }
        else
        {
            Debug.Log("такого модиф. отряда нет");
        }
    }

    #endregion

    #region Terrain Stats Modifiers

    void UnitTerrainModifierAdded(SOTerrainStatsModifier modifyer)
    {
        if (terrainStatsDictionary.ContainsKey(modifyer))
            terrainStatsDictionary[modifyer]++;
        else
            terrainStatsDictionary.Add(modifyer, 1);

        if (terrainStatsDictionary[modifyer] >= UnitCountToApplyModifier && !terrainStatsModifyers.Contains(modifyer))
            AddTerrainStatsModifier(modifyer);
    }

    void UnitTerrainModifierRemoved(SOTerrainStatsModifier modifyer)
    {
        if (!terrainStatsDictionary.ContainsKey(modifyer))
            return;

        terrainStatsDictionary[modifyer]--;

        if (terrainStatsDictionary[modifyer] < UnitCountToApplyModifier && terrainStatsModifyers.Contains(modifyer))
            RemoveTerrainStatsModifier(modifyer);

        if (terrainStatsDictionary.ContainsKey(modifyer) && terrainStatsDictionary[modifyer] <= 0)
            terrainStatsDictionary.Remove(modifyer);
    }

    public void AddTerrainStatsModifier(SOTerrainStatsModifier modifier)
    {
        if (!terrainStatsModifyers.Contains(modifier))
        {
            terrainStatsModifyers.Add(modifier);

            if (OnCallApplyTerrainModifierToAllUnit != null)
                OnCallApplyTerrainModifierToAllUnit(modifier);

            if (OnTerrainModifiersListChanged != null)
                OnTerrainModifiersListChanged(terrainStatsModifyers.ToArray());

            if (terrainStatsModifyers.Count > 0 && modifier.NeedMask)
            {
                if (this == playerSquadInstance && SquadMask.Instance != null)
                    SquadMask.Instance.Active = true;
                hiding = true;
            }

            Debug.Log("было вызвано событие для применения модиф.ландшафта ко всему отряду");
        }
        else
            Debug.Log("Модификатор ландшафта отряда уже существует!");
    }

    /// <summary>
    /// Удаляет модификаток, эквивалентный переданому, и отменяэт его действия с юнитoB
    /// </summary>
    /// <param name="modifier"></param>
    public void RemoveTerrainStatsModifier(SOTerrainStatsModifier modifier)
    {
        if (terrainStatsModifyers.Remove(modifier))
        {
            // ТУТ НАДО БЫЛО БЫ НЕ У ВСЕХ ОТНИМАТЬ У ТОЛЬКО У ТЕХ НА КОГО НЕ ДОЛЖНЫ ДЕЙСТВОВАТЬ. НО Я ПОКА ХЗ КАК, ЧТОБЫ ОПТИМИЗИРОВАННО БЫЛО
            if (OnCallRejectTerrainModifierToAllUnit != null)
                OnCallRejectTerrainModifierToAllUnit(modifier);

            if (OnTerrainModifiersListChanged != null)
                OnTerrainModifiersListChanged(terrainStatsModifyers.ToArray());

            if (terrainStatsModifyers.Find(mod => mod.NeedMask) == null)
            {
                if (this == playerSquadInstance && SquadMask.Instance != null)
                    SquadMask.Instance.Active = false;
                hiding = false;
            }

            Debug.Log("было вызвано событие для отмены модиф.ландшафта ко всему отряду");
        }
        else
        {
            Debug.Log("такого модиф.ландшафта отряда нет");
        }
    }

    #endregion
        

    private void Squad_OnTerrainModifiersListChanged(SOTerrainStatsModifier[] obj)
    {
        CalcSpeed();
    }

    private void Squad_OnModifiersListChanged(UnitStatsModifier[] obj)
    {
        CalcSpeed();
    }


    public void SetUnitsStats(DSUnitStats stats)
    {
        UnitStats oldStats = unitStats;

        unitStats = UnitStats.RewriteStats(unitStats, stats);

        maxSpeed = unitStats.Speed / 10;
        CalcSpeed();

        if(OnUnitStatsChanged != null)
            OnUnitStatsChanged(oldStats, unitStats);

        squadHelth = 0;
        foreach (var unitpos in unitPositions)
            squadHelth += unitpos.Unit.Health;
        SquadHealth = SquadHealth;
    }

    void Update()
    {
        DrawPath();

        CalcCenterSquad();

        Moving();
    }

    private void CalcCenterSquad()
    {
        int cnt = UnitCount;
        float sumX = 0;
        float sumY = 0;
        Vector2 pos;
        int countSumUnit = 0;
        
        float distSqr = distanceToCenterSquad * distanceToCenterSquad;

        float sumX2 = 0;
        float sumY2 = 0;
        int countSumUnit2 = 0;

        for (int i = 0; i < UnitCount; i++)
        {
            pos = unitPositions[i].Unit.ThisTransform.position;
            if (Vector2.SqrMagnitude(pos - centerSquad) > distSqr)
            {
                sumX2 += pos.x;
                sumY2 += pos.y;
                countSumUnit2++;
            }
            else
            {
                sumX += pos.x;
                sumY += pos.y;
                countSumUnit++;
            }
        }
        if(countSumUnit > 0)
            centerSquad = new Vector2(sumX / countSumUnit, sumY / countSumUnit);
        else
            centerSquad = new Vector2(sumX2 / countSumUnit2, sumY2 / countSumUnit2);
    }

    void CalcSpeed()
    {
        //currentSpeed = maxSpeed * (1 + (inventory.Body).EquipmentStats.AddSpeed) * (1 + currentFormationModifyers.SQUAD_ADDITIONAL_SPEED);
        //currentRotationSpeed = maxRotationSpeed * (1 + currentFormationModifyers.SQUAD_ADDITIONAL_ROTATION_SPEED);
        var stats = UnitStats;

        currentSpeed = stats.Speed / 10;
        //if (currentSpeed > maxSpeed)
        //    currentSpeed = maxSpeed;

        currentRotationSpeed = stats.RotationSpeed / 6;
        if (currentRotationSpeed < minRotationSpeed)
            currentRotationSpeed = minRotationSpeed;
        if (currentRotationSpeed > maxRotationSpeed)
            currentRotationSpeed = maxRotationSpeed;
    }

    void SetProp(EquipmentStack newEquipment = null)
    {
        CalcSpeed();

        if (newEquipment != null && !(inventory.Weapon).EquipmentStats.CanReformToPhalanx)
                CurrentFormation = FormationStats.Formations.RANKS;

        if (newEquipment != null && newEquipment.EquipmentStats.Type == EquipmentStats.TypeOfEquipment.SHIELD && newEquipment.EquipmentStats.Empty)
            if (CurrentFormation == FormationStats.Formations.RISEDSHIELDS)
                CurrentFormation = FormationStats.Formations.RANKS;
    }

    void  SetFlagFight()
    {
        if (attakingUnitsCount > 0 || targettedUnitsCount > 0)
            InFight = true;
        else
            StartCoroutine(OlreadyNotInFight(delayToGetOutOfTheFight));
    }

    IEnumerator OlreadyNotInFight(float duration)
    {
        float time = 0;

        while(time < duration)
        {
            time += Time.deltaTime;
            yield return null;
            if (attakingUnitsCount > 0 || targettedUnitsCount > 0)
                break;
        }

        if (attakingUnitsCount == 0 && targettedUnitsCount == 0)
            InFight = false;
    }

    public void Charge(UnitStatsModifier modifyer, float duration)
    {
        StartCoroutine(WaitForEndGharge(modifyer, duration));
    }

    IEnumerator WaitForEndGharge(UnitStatsModifier modifyer, float duration)
    {
        if (OnBeginCharge != null)
            OnBeginCharge(modifyer);
        charging = true;
        
        yield return new WaitForSeconds(duration);
        if (OnEndCharge != null)
            OnEndCharge(modifyer);
        charging = false;
    }
    
    void Moving()
    {    
        switch (CurrentFormation)
        {
            case FormationStats.Formations.RANKS:
                MovingInRanks();
                break;
            case FormationStats.Formations.PHALANX:
                MovingInPhalanx();
                break;
            case FormationStats.Formations.RISEDSHIELDS:
                MovingInRiseshields();
                break;
        }
    }

    void MovingInRiseshields()
    {
        MovingInRanks();
    }

    /// <summary>
    /// При передвижении отряд будет всегда развеернут в конечное направление взгляда
    /// </summary>
    void MovingInPhalanx()
    {
        //если указан путь
        if (Path != null)
        {
            // если еще не дошли до последней точки пути
            if (pathStep < Path.Count)
            {
                //идем по указанному пути к следующей точке
                LookWithoutFullRotation(EndLookRotation);

                MoveTo(Path[pathStep]);

                if (CheckPositionInRange(Path[pathStep], positionAcuracy))
                    pathStep++;
            }
            else if (pathStep == Path.Count) // остановились
            {                
                LookWithoutFullRotation(EndLookRotation);

                if (CheckRotationInRange(EndLookRotation, rotationAcuracy))
                {
                    Path = null;
                    pathStep = 1;
                }
            }
        }
    }

    /// <summary>
    /// При передвижении отряд будет разворачиваться в сторону движения
    /// </summary>
    void MovingInRanks()
    {
        if (charging)
        {
            var pos = PositionsTransform.position;
            var step = flipRotation ? -PositionsTransform.up : PositionsTransform.up;
            MoveTo(pos + step * currentSpeed);
        }
        else
        {
            if (Path != null)
            {
                if (pathStep < Path.Count)
                {
                    Quaternion lookRot = CalcTargetRotations(Path[pathStep]);
                    LookWithoutFullRotation(lookRot);

                    MoveTo(Path[pathStep]);

                    if (CheckPositionInRange(Path[pathStep], positionAcuracy))
                        pathStep++;
                }
                else if (pathStep == Path.Count)
                {
                    LookWithoutFullRotation(EndLookRotation);

                    if (CheckRotationInRange(EndLookRotation, rotationAcuracy))
                    {
                        Path = null;
                        pathStep = 1;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Если добавлен компонент LineRenderer, будет отрисовываться текущий путь отряда
    /// </summary>
    void DrawPath()
    {
        if (lineRenderer != null)
        {
            if (Path != null)
            {
                lineRenderer.positionCount = Path.Count + 2 - pathStep;
                Vector3[] p = new Vector3[lineRenderer.positionCount];
                Array.Copy(Path.ToArray(), pathStep, p, 2, Path.Count - pathStep);
                p[0] = centerSquad;
                p[1] = PositionsTransform.position;
                for (int i = 0; i < p.Length; i++)
                    p[i].z = -1;
                lineRenderer.SetPositions(p);
            }
            else
            {
                lineRenderer.positionCount = 2;
                Vector3[] p = new Vector3[lineRenderer.positionCount];
                p[0] = centerSquad;
                p[1] = PositionsTransform.position;
                for (int i = 0; i < p.Length; i++)
                    p[i].z = -1;
                lineRenderer.SetPositions(p);
            }
        }
    }

    /// <summary>
    /// <para> Проверяем нужно ли отражать формацию отряда, высчитываем новы угол поворота отряда и устанавливаем соответствующее значение флага.</para>
    /// <para> При изменении флага отражения отряда происходит перестроение отряда.</para>
    /// </summary>
    /// <param name="lookRot"></param>
    void LookWithoutFullRotation(Quaternion lookRot)
    {
        bool flipingFlag = false;

        float rAngle = lookRot.eulerAngles.z - PositionsTransform.rotation.eulerAngles.z;
        float rotAngle = rAngle >= 0 ? rAngle : -rAngle;

        if (rotAngle > 90 && rotAngle < 270)
        {
            lookRot *= Quaternion.Euler(0, 0, 180);
            flipingFlag = true;
        }
        LookTo(lookRot);

        if (flipRotation != flipingFlag)
        {
            FlipRotation = flipingFlag;
            ReformSquad(FlipRotation);
        }               
    }

    /// <summary>
    /// Добавляет юнит в коллекцию юнитов и создает для него объект расположения. При этом отряд будут перестроен.
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(Unit unit)
    {
        AddUnitWithoutReformSquad(unit);
        ReformSquad(flipRotation);
    }

    /// <summary>
    /// Удаляет объект расположения юнита из отряда. Отряд будет перестроен.
    /// </summary>
    /// <param name="unitPositionObject"></param>
    public void UnitDeath(Unit unit)
    {
        unit.OnModifierAdd -= UnitModifierAdded;
        unit.OnModifierRemove -= UnitModifierRemoved;

        unit.OnTerrainModifierAdd -= UnitTerrainModifierAdded;
        unit.OnTerrainModifierRemove-= UnitTerrainModifierRemoved;
        //-------

        ReformSquad(flipRotation, unit.TargetMovePositionObject);
        
        if (unitPositions.Count <= 0)
        {
            if (OnSquadDestroy != null)
                OnSquadDestroy();
            Destroy(GetComponent<LineRenderer>());

            if(this == playerSquadInstance)
            {
                gameObject.AddComponent<DestroyOnAwake>();
         //       Destroy(this);
            }

            Destroy(gameObject);
        }

        if (OnUitCountChanged != null) OnUitCountChanged(unitPositions.Count);
    }

    /// <summary>
    /// Добавляем юнит в коллекцию юнитов и создаем для него объект расположения. При этом отряд не будут перестроен.
    /// </summary>
    /// <param name="unit"></param>
    void AddUnitWithoutReformSquad(Unit unit)
    {
        unit.OnModifierAdd += UnitModifierAdded;
        unit.OnModifierRemove += UnitModifierRemoved;

        unit.OnTerrainModifierAdd += UnitTerrainModifierAdded;
        unit.OnTerrainModifierRemove += UnitTerrainModifierRemoved;
        //-------

        UnitPosition targetMovePositionObject = unit.TargetMovePositionObject.GetComponent<UnitPosition>();

        unitPositions.Add(targetMovePositionObject);
        targetMovePositionObject.transform.parent = PositionsTransform;
        targetMovePositionObject.Unit = unit;
        targetMovePositionObject.Squad = this;

        unit.TargetMovePositionObject = targetMovePositionObject;
        unit.transform.parent = UnitsContainer.transform;
        unit.squad = this;
        unit.transform.rotation = PositionsTransform.rotation;

        int l = unit.gameObject.layer;

        unit.SetFraction(fraction);

        //if (l == LayerMask.NameToLayer(Squad.UnitFraction.NEUTRAL.ToString()))
            unit.Init();

        SquadHealth += unit.Health;
        if (OnUitCountChanged != null) OnUitCountChanged(unitPositions.Count);
    }

    /// <summary>
    /// Меняем местами первые и последние ряды, отзеркаливая отряд относительно рогизонтальной прямой
    /// На ссамом деле просто меняем местами объекты в списке.
    /// </summary>
    void FlipPhalanx()
    {
        int lastRowIndex = unitPositions.Count / squadLength;
        lastRowIndex = unitPositions.Count % squadLength == 0 ? lastRowIndex : lastRowIndex + 1;
        lastRowIndex--;

        if (lastRowIndex > 0)
        {
            int unitCountInLastRow = unitPositions.Count - lastRowIndex * squadLength;
            int shift = (squadLength - unitCountInLastRow) / 2;

            UnitPosition buffer = null;

            int firstIndex, secondIndex;

            for (int c = -shift; c < squadLength - shift; c++)
            {
                //проходим по всем рядам с первого по последний включая последний
                if (c >= -shift && c < unitCountInLastRow - shift)
                {
                    for (int row = 0; row < (lastRowIndex + 1) / 2; row++)
                    {
                        if (row == 0)
                        {
                            //меняем местами юниты с последнего ряда и первого, ставя их в центр отряда
                            firstIndex = (row * squadLength) + c + shift * 2;
                            secondIndex = (lastRowIndex - row) * squadLength + c + shift;
                        }
                        else
                        {
                            //после чего, меняем местами центры остльных рядов отряда
                            firstIndex = (row * squadLength) + c + shift * 2;
                            secondIndex = (lastRowIndex - row) * squadLength + c + shift * 2;
                        }

                        buffer = unitPositions[firstIndex];
                        unitPositions[firstIndex] = unitPositions[secondIndex];
                        unitPositions[secondIndex] = buffer;
                    }
                }
                else
                {
                    //проходим по всем рядам с первого до последнего не включая последний
                    for (int row = 0; row < (lastRowIndex + 1) / 2; row++)
                    {
                        if (c < (squadLength - 2 * shift))
                        {
                            firstIndex = (row * squadLength) + c + shift * 2;
                            secondIndex = (lastRowIndex - row - 1) * squadLength + c + shift * 2;
                        }
                        else
                        {
                            firstIndex = (row * squadLength) + c + 2 * shift - squadLength;
                            secondIndex = (lastRowIndex - row - 1) * squadLength + c + 2 * shift - squadLength;
                        }

                        buffer = unitPositions[firstIndex];
                        unitPositions[firstIndex] = unitPositions[secondIndex];
                        unitPositions[secondIndex] = buffer;
                    }
                }
            }
        }

        ReformSquad(flipRotation);
    }

    public void ResetUnitPositions()
    {
        if(OnInitiateUnitPositions != null)
            OnInitiateUnitPositions();
    }

    /// <summary>
    /// Перестраиваем отряд, что бы он держал заднную формацию
    /// </summary>
    /// <param name="flipRotation">отражать ли формацию отряда по горизонтали</param>
    /// <param name="objToRemove">позиция юнита, которую нужно удалить пере перестроением</param>
    void ReformSquad(bool flipRotation, UnitPosition objToRemove = null)
    {
        FormationStats.Formations newFormation = FormationStats.Formations.RANKS;
        switch (CurrentFormation)
        {
            case FormationStats.Formations.RANKS:
                ReformSquadInRanks(flipRotation, objToRemove);
                newFormation = FormationStats.Formations.RANKS;
                break;

            case FormationStats.Formations.PHALANX:
                ReformSquadInPhalanx(flipRotation, objToRemove);
                newFormation = FormationStats.Formations.PHALANX;
                break;

            case FormationStats.Formations.RISEDSHIELDS:

                //unitPositions.Remove(objToRemove);
                //if (objToRemove != null)
                //    Destroy(objToRemove.gameObject);
                ReformSquadInRanks(flipRotation, objToRemove);

                newFormation = FormationStats.Formations.RISEDSHIELDS;
                break;
        }

        if (OnReformSquad != null)
            OnReformSquad(newFormation);
    }

    //private void ReformSquadInRanks(bool flipRotation, UnitPosition objToRemove)
    //{
    //    unitPositions.Remove(objToRemove);
    //    if (objToRemove != null)
    //        Destroy(objToRemove.gameObject);

    //    if (unitPositions.Count > 0)
    //    {
    //        int rowsAsPhalanx = unitPositions.Count / squadLength;
    //        rowsAsPhalanx = unitPositions.Count % squadLength == 0 ? rowsAsPhalanx : rowsAsPhalanx + 1;

    //        int rows = (int)Mathf.Round(Mathf.Sqrt(unitPositions.Count));
    //        int cols = unitPositions.Count / rows;
    //        cols = cols * rows >= unitPositions.Count ? cols : cols + 1;

    //        int colStart = (squadLength - cols) / 2;

    //        int verticalShift = rowsAsPhalanx / 2;
    //        int rowToUp = rowsAsPhalanx % 2 == 0 ? verticalShift : verticalShift + 1;

    //        int unitCountInLastRow = unitPositions.Count - (rowsAsPhalanx - 1) * squadLength;
    //        int shift = (squadLength - unitCountInLastRow) / 2;

    //        int flip = flipRotation ? -1 : 1;

    //        for (int r = 0; r < rowsAsPhalanx; r++)
    //        {
    //            for (int c = colStart; c < colStart + cols; c++)
    //            {
    //                if (r * squadLength + c < unitPositions.Count + shift)
    //                {
    //                    int yShift;

    //                    if (r < rowToUp)
    //                        yShift = verticalShift;
    //                    else
    //                        yShift = -verticalShift;

    //                    if (r == rowsAsPhalanx - 1)
    //                    {
    //                        unitPositions[r * squadLength + c - shift].transform.localPosition = new Vector2(
    //                           //c - cols / 2 - 1f,
    //                           c - squadLength / 2 + 0.5f,
    //                           ((1 - r) + yShift) * flip
    //                        );
    //                    }
    //                    else
    //                    {
    //                        unitPositions[r * squadLength + c].transform.localPosition = new Vector2(
    //                            //c - cols / 2 - 1f,
    //                            c - squadLength / 2 + 0.5f,
    //                            ((1 - r) + yShift) * flip
    //                        );
    //                    }
    //                }
    //            }
    //        }

    //        int horizontalShift = cols / 2;

    //        for (int r = 0; r < rowsAsPhalanx; r++)
    //        {
    //            for (int c = 0; c < squadLength; c++)
    //            {
    //                if (r * squadLength + c < unitPositions.Count)
    //                {
    //                    int xShift;

    //                    if (c < colStart || c >= colStart + cols)
    //                    {
    //                        if (c < squadLength / 2)
    //                            xShift = verticalShift;
    //                        else
    //                            xShift = -verticalShift;

    //                        unitPositions[r * squadLength + c].transform.localPosition = new Vector2(
    //                            c + xShift - squadLength / 2 + 0.5f,
    //                            (1 - r) * flip
    //                        );
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    void ReformSquadInRanks(bool flipRotation, UnitPosition objToRemove)
    {
        //experemental row
        //squadLength = 5;


        unitPositions.Remove(objToRemove);
        if (objToRemove != null)
            Destroy(objToRemove.gameObject);

        int unitCount = unitPositions.Count;
        int rowLength = 0;
        int row = 0;

        int flip = flipRotation ? -1 : 1;

        while (unitCount > 0)
        {
            if (unitCount >= SQUAD_LENGTH)
                rowLength = SQUAD_LENGTH;
            else
                rowLength = unitCount;

            for (int i = row * SQUAD_LENGTH; i < row * SQUAD_LENGTH + rowLength; i++)
            {
                unitPositions[i].transform.localPosition = new Vector3(
                    ((-squadLength / 2f + i - row * SQUAD_LENGTH + 0.5f + (squadLength - rowLength) / 2) + UnityEngine.Random.value * 0.5f),//*unitPositions[i].Scale,
                    ((1 - row) * flip + UnityEngine.Random.value * 0.5f), //*unitPositions[i].Scale,
                    0
                );

                if (!flipRotation)
                {
                    unitPositions[i].PositionInArray = new Vector2(
                        i - row * SQUAD_LENGTH,
                        -((1 - row) * flip) + 1
                    );
                }
                else
                {
                    unitPositions[i].PositionInArray = new Vector2(
                        i - row * SQUAD_LENGTH,
                        (1 - row) * flip + 1
                    );
                }

                unitPositions[i].RowInPhalanx = (int)unitPositions[i].PositionInArray.y + 1;
            }

            row++;
            unitCount -= SQUAD_LENGTH;
        }
    }

    void ReformSquadInPhalanx(bool flipRotation, UnitPosition objToRemove)
    {
        //experemental row
        //squadLength = 10;

        //удаление объекта
        if (objToRemove != null)
        {
            int currentRowsCount = unitPositions.Count / squadLength;
            currentRowsCount = unitPositions.Count % squadLength == 0 ? currentRowsCount : currentRowsCount + 1;

            int unitCountInLastRow = unitPositions.Count - (currentRowsCount - 1) * squadLength;

            int r = (int)objToRemove.PositionInArray.y;

            int c = (int)objToRemove.PositionInArray.x;
            
            int c2;

            if (objToRemove != null)
                Destroy(objToRemove.gameObject);

            Vector2 posToRemove = new Vector2(c, r);

            for (int _row = r; _row < currentRowsCount - 1; _row++)
            {
                if (_row + 1 < currentRowsCount - 1)
                {
                    if ((_row + 1) * squadLength + c < FULL_SQUAD_UNIT_COUNT)
                    {
                        unitPositions[_row * squadLength + c] = unitPositions[(_row + 1) * squadLength + c];
                        posToRemove = new Vector2(c, _row + 1);
                    }
                }
                else
                {
                    c2 = c - (squadLength - unitCountInLastRow) / 2;
                    c2 = c2 < 0 ? 0 : c2;
                    c2 = c2 >= unitCountInLastRow ? unitCountInLastRow - 1 : c2;

                    if ((_row + 1) * squadLength + c2 < FULL_SQUAD_UNIT_COUNT)
                    {
                        unitPositions[_row * squadLength + c] = unitPositions[(_row + 1) * squadLength + c2];
                        posToRemove = new Vector2(c2, _row + 1);
                    }
                }
            }

            unitPositions.RemoveAt((int)posToRemove.y * squadLength + (int)posToRemove.x);
        }

        //установка положений (перегрупировкаотряда)
        int unitCount = unitPositions.Count;
        int rowLength = 0;
        int row = 0;

        int flip = flipRotation ? -1 : 1;

        while (unitCount > 0)
        {
            if (unitCount >= SQUAD_LENGTH)
                rowLength = SQUAD_LENGTH;
            else
                rowLength = unitCount;

            for (int i = row * SQUAD_LENGTH; i < row * SQUAD_LENGTH + rowLength; i++)
            {
                unitPositions[i].transform.localPosition = new Vector3(
                    (-squadLength / 2f + i - row * SQUAD_LENGTH + 0.5f + (squadLength - rowLength) / 2),// *unitPositions[i].Scale,
                    (1 - row) * flip,// * unitPositions[i].Scale,
                    0
                );

                if (!flipRotation)
                {
                    unitPositions[i].PositionInArray = new Vector2(
                        i - row * SQUAD_LENGTH,
                        -((1 - row) * flip) + 1
                    );
                }
                else
                {
                    unitPositions[i].PositionInArray = new Vector2(
                        i - row * SQUAD_LENGTH,
                        (1 - row) * flip + 1
                    );
                }

                unitPositions[i].RowInPhalanx = (int)unitPositions[i].PositionInArray.y + 1;                
            }

            row++;
            unitCount -= SQUAD_LENGTH;
        }
    }
    
    Quaternion CalcTargetRotations(Vector3 targetPosition)
    {
        return Quaternion.LookRotation(Vector3.forward, targetPosition - PositionsTransform.position);
    }

    bool CheckRotationInRange(Quaternion targetRotation, float rotationRange)
    {
        return Mathf.Abs(PositionsTransform.rotation.eulerAngles.z - targetRotation.eulerAngles.z) * Mathf.Rad2Deg <= rotationRange;
    }

    bool CheckPositionInRange(Vector2 position, float positionRange)
    {
        return Mathf.Sqrt(
                (position.x - PositionsTransform.position.x)* (position.x - PositionsTransform.position.x) +
                (position.y - PositionsTransform.position.y) * (position.y - PositionsTransform.position.y)
        ) <= positionRange;
    }

    void LookTo(Quaternion lookRotation)
    {
        PositionsTransform.rotation = Quaternion.RotateTowards(PositionsTransform.rotation, lookRotation, currentRotationSpeed * Time.deltaTime);
    }

    void MoveTo(Vector2 position)
    {
        PositionsTransform.position = Vector2.MoveTowards(PositionsTransform.position, position, CurrentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Указать отряду что нужно выкинуть стак.
    /// Это укажет всем юнитам что нужно дропнуть итем с этого стака
    /// </summary>
    /// <param name="stack"></param>
    public void DropEquipment(EquipmentStack stack)
    {
        if(OnDropStackFromInventory != null)
            OnDropStackFromInventory(stack);
    }

    void OnDrawGizmos()
    {
        if(inFight)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(centerSquad, 1);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(centerSquad, distanceToUnionWithSquad);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(centerSquad, distanceToCenterSquad);
    }
}
