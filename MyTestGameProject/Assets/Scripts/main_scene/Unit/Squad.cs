using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Squad : MonoBehaviour
{
    public enum UnitFraction { ALLY, ENEMY, NEUTRAL }

    static public Squad playerSquadInstance;

    [SerializeField] Transform minimapMark;

    [Header("Default units properties in this squad")]
    [SerializeField] Unit unitOriginal;
    [Space]
    [SerializeField] UnitStats unitStats;
    public UnitStats UnitStats { get { return unitStats; } }

    [Header("Squad default properties")]
    [Range(1, 100)] public int fullSquadUnitCount = 30;
    public int FULL_SQUAD_UNIT_COUNT { get { return fullSquadUnitCount; } }
    [SerializeField] [Range(1, 100)] int squadLength = 10;
    public int SQUAD_LENGTH { get { return squadLength; } }
    int squadRowsMax;
    public int SQUAD_ROWS_MAX {  get { return squadRowsMax; } }
    [Space]
    [SerializeField] [Range(1, 10)] float maxSpeed = 4;
    public float MaxSpeed { get { return maxSpeed; } }
    [SerializeField] [Range(1, 180)] float maxRotationSpeed = 50;
    public float MaxRotationSpeed { get { return maxRotationSpeed; } }
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

                    if (!(inventory.Weapon).Stats.CanReformToPhalanx)
                        formation = lastFormation;
                    else if (!inventory.Weapon.Stats.CanReformToPhalanxInFight && InFight)
                        formation = lastFormation;

                    break;

                case FormationStats.Formations.RISEDSHIELDS:

                    if(inventory.Shield.Stats.Empty)
                        formation = lastFormation;

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
                    OnFormationChanged(formation);

            if (tag == "Player" && formation != FormationStats.Formations.RANKS && PlayerSquadController.Instance != null)
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
    [SerializeField] bool flipRotation = false;
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
      

    [SerializeField] int attakingUnitsCount = 0;
    public int AttakingUnitsCount
    {
        get { return attakingUnitsCount; }
        set { attakingUnitsCount = value; if (attakingUnitsCount < 0) attakingUnitsCount = 0; SetFlagFight(); }
    }
    [SerializeField] int targettedUnitsCount = 0;
    public int TargettedUnitsCount
    {
        get { return targettedUnitsCount; }
        set { targettedUnitsCount = value; SetFlagFight(); }
    }
    [Space]
    [SerializeField] Vector2 centerSquad;
    public Vector2 CenterSquad { get { return centerSquad; } }
    [Space]
    [SerializeField] List<UnitPosition> unitPositions;
    /// <summary>
    /// Ни в коем случае не надо ничкго менять в позициях! Понял, йопта?!
    /// </summary>
    public List<UnitPosition> UnitPositions { get { return unitPositions; } }

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

    public GameObject UnitsContainer { get; private set; }
    public int UnitCount { get { return unitPositions.Count; } }

    float squadHelth;
    public float SquadHealth
    {
        get { return squadHelth; }
        set { squadHelth = value; if (OnSumHealthChanged != null) OnSumHealthChanged(value); }
    }

    List<Vector3> path = null;
    public Quaternion endLookRotation;
    Vector3 endMovePosition;

    LineRenderer lineRenderer;

    int pathStep = 1;

    public event Action<int> OnUitCountChanged;
    public event Action<float> OnSumHealthChanged;
    public event Action<FormationStats.Formations> OnFormationChanged;
    public event Action<EquipmentStack> OnDropStackFromInventory;
    public event Action<FormationStats.Formations> OnReformSquad;
    public event Action OnInitiateUnitPositions;
    public event Action<UnitStatsModifyer> OnBeginCharge;
    public event Action<UnitStatsModifyer> OnEndCharge;

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
        endLookRotation = PositionsTransform.rotation;
        LookWithoutFullRotation(endLookRotation);

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
            OnFormationChanged(CurrentFormation);

        if (minimapMark != null)
            minimapMark.gameObject.SetActive(true);

        inventory.OnEquipmentChanged += SetProp;

        SetProp(inventory.Weapon);

        if (this == playerSquadInstance && (GameManager.SceneIndex)SceneManager.GetActiveScene().buildIndex != GameManager.SceneIndex.LEVEL_TUTORIAL)
        {
            var progress = GameManager.Instance.PlayerProgress;

            SetUnitsStats(progress.Stats);
            var skill = progress.Skills.firstSkill;
            inventory.FirstSkill.Skill = skill;
            if(skill != null)
                inventory.FirstSkill.SkillStats = skill.CalcUpgradedStats(progress.Skills.skills.Find((t)=> { return t.Id == skill.Id; }).Upgrades);

            skill = progress.Skills.secondSkill;
            inventory.SecondSkill.Skill = progress.Skills.secondSkill;
            if (skill != null)
                inventory.SecondSkill.SkillStats = skill.CalcUpgradedStats(progress.Skills.skills.Find((t) => { return t.Id == skill.Id; }).Upgrades);
        }
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

        if(minimapMark != null)
            minimapMark.position = centerSquad;

        Moving();
    }



    /// <summary>
    /// тут старая реализация !!!
    /// ЕСЛИ ЕСТЬ БАГ С ПЕРЕМЕЩЕНИЕМ, НАДО ПОСМОТРЕТЬ СЮДА
    /// </summary>
    void CalcSpeed()
    {
        currentSpeed = maxSpeed * (1 + (inventory.Body).Stats.AddSpeed) * (1 + currentFormationModifyers.SQUAD_ADDITIONAL_SPEED);
        currentRotationSpeed = maxRotationSpeed * (1 + currentFormationModifyers.SQUAD_ADDITIONAL_ROTATION_SPEED);
    }
    


    void SetProp(Equipment newEquipment = null)
    {
        CalcSpeed();

        if (newEquipment != null && newEquipment.Stats.Type == EquipmentStats.TypeOfEquipment.WEAPON)
        {
            if(!inventory.Weapon.Stats.CanUseWithShield)
            {
                if (!inventory.Shield.Stats.Empty)
                {
                    DropEquipment(new EquipmentStack(inventory.Shield, UnitCount));
                    inventory.Shield = null;

                    if (CurrentFormation == FormationStats.Formations.RISEDSHIELDS)
                        CurrentFormation = FormationStats.Formations.RANKS;
                }
            }
            if (!(inventory.Weapon).Stats.CanReformToPhalanx)
            {
                CurrentFormation = FormationStats.Formations.RANKS;
            }
        }
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

    public void Charge(UnitStatsModifyer modifyer, float duration)
    {
        StartCoroutine(WaitForEndGharge(modifyer, duration));
    }

    IEnumerator WaitForEndGharge(UnitStatsModifyer modifyer, float duration)
    {
        if (OnBeginCharge != null)
            OnBeginCharge(modifyer);
        charging = true;
        
        currentSpeed *= 1 + modifyer.Speed.Value;

        yield return new WaitForSeconds(duration);
        if (OnEndCharge != null)
            OnEndCharge(modifyer);
        charging = false;

        currentSpeed /= 1 + modifyer.Speed.Value;
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
        if (path != null)
        {
            // если еще не дошли до последней точки пути
            if (pathStep < path.Count)
            {
                //идем по указанному пути к следующей точке
                LookWithoutFullRotation(endLookRotation);

                MoveTo(path[pathStep]);

                if (CheckPositionInRange(path[pathStep], positionAcuracy))
                    pathStep++;
            }
            else if (pathStep == path.Count) // остановились
            {                
                LookWithoutFullRotation(endLookRotation);

                if (CheckRotationInRange(endLookRotation, rotationAcuracy))
                {
                    path = null;
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
        if (path != null)
        {
            if (pathStep < path.Count)
            { 
                Quaternion lookRot = CalcTargetRotations(path[pathStep]);
                LookWithoutFullRotation(lookRot);

                MoveTo(path[pathStep]);

                if (CheckPositionInRange(path[pathStep], positionAcuracy))
                    pathStep++;
            }
            else if (pathStep == path.Count)
            {
                LookWithoutFullRotation(endLookRotation);

                if (CheckRotationInRange(endLookRotation, rotationAcuracy))
                {
                    path = null;
                    pathStep = 1;
                }
            }
        }
    }

    /// <summary>
    /// <para>Прибавляет позицию юнита к общей сумме и вычисляя среднее значение, определяет координаты центра отряда.</para>
    /// <para>Этот метод необходимо вызывать в отдном из методов Update</para>
    /// </summary>
    /// <param name="unit"></param>
    public void SumPositionUnit(Unit unit)
    {
        Vector2 pos = unit.ThisTransform.position;

        sumX += pos.x;
        sumY += pos.y;
        countSumUnit++;

        if(countSumUnit >= unitPositions.Count)
        {
            centerSquad = new Vector2(sumX / countSumUnit, sumY / countSumUnit);
            sumX = 0;
            sumY = 0;
            countSumUnit = 0;
        }       

    }

    /// <summary>
    /// Если добавлен компонент LineRenderer, будет отрисовываться текущий путь отряда
    /// </summary>
    void DrawPath()
    {
        if (lineRenderer != null)
        {
            if (path != null)
            {
                lineRenderer.positionCount = path.Count + 2 - pathStep;
                Vector3[] p = new Vector3[lineRenderer.positionCount];
                Array.Copy(path.ToArray(), pathStep, p, 2, path.Count - pathStep);
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

                unitPositions.Remove(objToRemove);
                if (objToRemove != null)
                    Destroy(objToRemove.gameObject);

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
            }

            row++;
            unitCount -= SQUAD_LENGTH;
        }
    }

    void ReformSquadInPhalanx(bool flipRotation, UnitPosition objToRemove)
    {
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

    /// <summary>
    /// Усанавливает конечную точку пути и поворот отряда
    /// </summary>
    /// <param name="positionMove">конечная точка пути отряда</param>
    /// <param name="rotationLook">конечный поворот отряда</param>
    public void SetEndMovePositions(Vector3 positionMove, Quaternion rotationLook)
    {
        endLookRotation = rotationLook;
        endMovePosition = positionMove;
    }

    /// <summary>
    /// Задает отряду путь
    /// </summary>
    /// <param name="path">Путь для отряда</param>
    public void GoTo(List<Vector3> path)
    {
        this.path = path;
        if (path != null)
        {
            path[path.Count - 1] = endMovePosition;
        }

        pathStep = 1;
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

    }
}
