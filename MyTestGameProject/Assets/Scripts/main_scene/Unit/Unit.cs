using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    static Transform DroppedItemsContainer;
    static Transform DeatUnitsContainer;

    static public event Action OnAnyUnitDeath;

    [HideInInspector] public UnitPosition TargetMovePositionObject;

    [HideInInspector] public Squad squad;

    [SerializeField] UnitPosition unitPosOriginal;
    [SerializeField] GameObject DroppedItemOriginal;
    

    [Space] [SerializeField] Transform shadow;
    public GameObject Shadow { get { return shadow.gameObject; } }

    public event Action<GameObject> OnArmsChanged;
    [Space] [SerializeField] GameObject arms;
    public GameObject Arms
    {
        get { return arms; }
        private set { arms = value; if (OnArmsChanged != null) OnArmsChanged(arms); }
    }

    public event Action<GameObject> OnLegsChanged;
    [SerializeField] GameObject legs;
    public GameObject Legs
    {
        get { return legs; }
        private set { legs = value; if (OnLegsChanged != null) OnLegsChanged(legs); }
    }

    [Space]
    [SerializeField] GameObject arms_with_spear;
    [SerializeField] GameObject arms_with_sword;
    [SerializeField] GameObject arms_with_pike;

    public float Health
    {
        get { return stats.Health; }
        private set
        {
            float oldVal = stats.Health;
            stats.Health = value;
            if (squad != null)
                OnHealthChanged(squad.DrawUnitHp);
            if (squad != null)
                squad.SquadHealth -= oldVal - stats.Health;

            if (stats.Health <= 0)
                Death();
        }
    }

    [Header("Unit default properties (will set by squad)")]
    [SerializeField] UnitStats stats;
    public UnitStats Stats { get { return stats; } }

    [Header("Unit current properties")]
    [SerializeField]
    float currentSpeed = 100;
    public float CurrentSpeed { get { return currentSpeed; } }

    [Header("Unit independent properties")]
    [Tooltip("Радиус вогруг точки, в которую движется юнит, при попадении в который, юнит перестанет пытаться идти дальше.")]
    [SerializeField] float movingRange = 1;
    public float MovingRange { get { return movingRange; } }
    [Tooltip("Сектор, при попадении поворота юнита в который, он перестанет пытаться поворачиваться.")]
    [SerializeField] float rotationRange = 5;
    public float RotationRange { get { return rotationRange; } }
    [Space]
    [Tooltip("Длина области пространства, в которой будут определятся цель.")]
    [SerializeField] [Range(0, 1)] float attackLineLength = 0.5f;
    [Tooltip("Ширина области пространства, в которой будут определятся цель.")]
    [SerializeField] [Range(0, 1)] float attackLineWidth = 0.2f;
    [Space]
    [Tooltip("Когда юнит получает урон, он будет застанет на данный промежуток времени.")]
    [SerializeField] [Range(0, 5)] float delayAfterTakingDamage = 1f;
    [Tooltip("Когда юнит толкает союзника, он будет застанет на данный промежуток времени.")]
    [SerializeField] [Range(0, 5)] float delayAfterPushingAlly = 0.2f;
    [Space]
    [SerializeField] [Range(0, 5)] float findTargetDeltaTime = 0.2f;
    [SerializeField] [Range(0, 5)] float attacInRanksDeltaTime = 1f;
    [SerializeField] [Range(0, 5)] float attacInPhalanxDeltaTime = 0.5f;
    [SerializeField] [Range(0, 5)] float timeForDeath = 1f;
    [Space]
    [SerializeField] [Range(0, 180)] float frontPushingAngleException = 40;
    [SerializeField] [Range(0, 360)] float pushingAngle = 40;
    [SerializeField] [Range(0, 360)] float chargePpushingAngle = 120;
    [SerializeField] [Range(0, 90)] float weaponAngleDeviation = 10;

    List<UnitStatsModifyer> StatsModifyers = new List<UnitStatsModifyer>();

    new Rigidbody2D rigidbody2D;
    CircleCollider2D circleCollider2D;

    GameObject selection;
    SpriteRenderer selectionRenderer;

    bool selected = false;
    public bool Selected
    {
        get { return selected; }
        set
        {
            if (gameObject.layer == LayerMask.NameToLayer(Squad.UnitFraction.ALLY.ToString()))
            {
                selected = true;
                selection.SetActive(false);
            }
            else if (gameObject.layer == LayerMask.NameToLayer(Squad.UnitFraction.ENEMY.ToString()))
            {
                selected = value;
                selection.SetActive(value);
            }
            else
            {
                selected = value;
                selection.SetActive(value);
            }
        }
    }

    bool stanned = false;
    public bool Stanned { get { return stanned; } }

    public Transform ThisTransform { get; private set; }

    Transform startRay;
    Transform endRay;

    float currentAttackDeltaTime = 0;
    float unionCheckDeltatime = 0.5f;

    int rHitLayerEnemy;
    RaycastHit2D rHit;
    Unit target;

    /// <summary>
    /// шанс получить удар от врага
    /// </summary>
    float takeHit = 0;

    Vector2 v1, v2;

    public event Action<GameObject> OnHelmetChanged;
    GameObject _helmet;
    public GameObject Helmet
    {
        get { return _helmet; }
        private set { _helmet = value; if (OnHelmetChanged != null) OnHelmetChanged(_helmet); }
    }
    Equipment helmet;

    public event Action<GameObject> OnBodyChanged;
    GameObject _body;
    public GameObject Body
    {
        get { return _body; }
        private set { _body = value; if (OnBodyChanged != null) OnBodyChanged(_body); }
    }
    Equipment body;

    public event Action<GameObject> OnShieldChanged;
    GameObject _shield;
    public GameObject Shield
    {
        get { return _shield; }
        private set { _shield = value; if (OnShieldChanged != null) OnShieldChanged(_shield); }
    }
    Equipment shield;

    public event Action<GameObject> OnWeaponChanged;
    GameObject _weapon;
    public GameObject Weapon
    {
        get { return _weapon; }
        private set { _weapon = value; if (OnWeaponChanged != null) OnWeaponChanged(_weapon); }
    }
    Equipment weapon;
    
    bool pushingAlly = false;
    List<Unit> pushedUnit = new List<Unit>(3);

    /// <summary>
    /// список всех юнитов который атакуют данный юнит (который взяли данный юнит как target)
    /// </summary>
    List<Unit> targettedBy = new List<Unit>(5);
    bool attaking = false;
    bool olreadyNotPushingAlly = false;

    float timerForAttack = 0;
    float timerForFindTarget = 0;
    float timerPushingAlly = 0;
    float timerForUnion = 0;
    float timerForLateDeath = 0;

    public event Action<bool> OnRunValueChanged;
    bool running = false;
    public bool Running
    {
        get { return running; }
        private set
        {
            if (running != value)
            {
                running = value;
                if (OnRunValueChanged != null) OnRunValueChanged(running);
            }
        }
    }

    public event Action<bool> OnChargingValueChanged;
    bool charging = false;
    public bool Charging
    {
        get { return charging; }
        private set
        {
            if (charging != value)
            {
                charging = value;
                if (OnChargingValueChanged != null) OnChargingValueChanged(charging);
            }
        }
    }
    Quaternion chargeRotation;

    int normLayer;

    /// <summary>
    /// Все юниты ищут цель в один кадр, и атакуют тоже в один кадр. Это садит фпс. Добавим задержку для таймера поиска цели. 
    /// Это размажет выполнение функции по большему отрезку времени.
    /// </summary>
    [NonSerialized] public float delayToFindTargetAndAttack = 0;

    public event Action AfterInitiate;
    public event Action OnUnitDeath;

    /// <summary>
    /// Флаг, который будет включатся при нанесении врагу удара, и сразу выключатся (в следующем кадре).
    /// </summary>
    public event Action<bool> OnHitChanged;
    bool hit = false;
    public bool Hit
    {
        get { return hit; }
        private set { hit = value; if (OnHitChanged != null) OnHitChanged(hit); }
    }

    public bool IsAlive { get; private set; } = true;

    void Awake()
    {
        ThisTransform = transform;

        TargetMovePositionObject = Instantiate(unitPosOriginal, ThisTransform);

        rigidbody2D = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();

        //начало и конец луча для атаки
        startRay = ThisTransform.Find("StartRay").gameObject.transform;
        endRay = ThisTransform.Find("EndRay").gameObject.transform;

        selection = ThisTransform.Find("SelectionUnit").gameObject;
        selectionRenderer = selection.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Squad_OnInitiateUnitPositions();

        SetDistanceLineForAttack();

        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        normLayer = gameObject.layer;
    }

    public void Init()
    {
        if (squad != null)
        {
            SetEquipment();

            squad.OnFormationChanged += OnFormationChanged;
            OnFormationChanged(squad.CurrentFormation);

            squad.OnDropStackFromInventory += OnDropStack;

            squad.OnReformSquad += OnReformSquad;
            OnReformSquad(squad.CurrentFormation);

            squad.OnInitiateUnitPositions += Squad_OnInitiateUnitPositions;

            squad.OnBeginCharge += Squad_OnBeginCharge;
            squad.OnEndCharge += Squad_OnEndCharge;

            squad.OnDrawUnitHpChanged += Squad_OnDrawUnitHpChanged;

            squad.OnUnitStatsChanged += Squad_OnUnitStatsChanged;

            if(gameObject.layer != LayerMask.NameToLayer(Squad.UnitFraction.ENEMY.ToString()))
            Selected = true;
            
            normLayer = gameObject.layer;
        }

        if (AfterInitiate != null) AfterInitiate();
    }

    public void Death()
    {
        if (squad != null)
        {
            EquipmentStack stack;

            if (!helmet.Stats.Empty)
            {
                stack = new EquipmentStack(helmet);
                OnDropStack(stack);
            }
            if (!body.Stats.Empty)
            {
                stack = new EquipmentStack(body);
                OnDropStack(stack);
            }
            if (!shield.Stats.Empty)
            {
                stack = new EquipmentStack(shield);
                OnDropStack(stack);
            }
            if (!weapon.Stats.Empty)
            {
                stack = new EquipmentStack(weapon);
                OnDropStack(stack);
            }

            int cnt = squad.Inventory.Length;
            for (int i = 0; i < cnt; i++)
            {
                if (squad.Inventory[i] != null && squad.Inventory[i].Count > 0 && squad.Inventory[i].Count >= squad.UnitCount)
                {
                    OnDropStack(new EquipmentStack(squad.Inventory[i].EquipmentMainProperties, squad.Inventory[i].EquipmentStats));
                    squad.Inventory[i].PopItems(1);
                }
            }

            squad.OnFormationChanged -= OnFormationChanged;
            squad.OnDropStackFromInventory -= OnDropStack;
            squad.OnReformSquad -= OnReformSquad;
            squad.OnInitiateUnitPositions -= Squad_OnInitiateUnitPositions;

            squad.OnBeginCharge -= Squad_OnBeginCharge;
            squad.OnEndCharge -= Squad_OnEndCharge;

            squad.OnDrawUnitHpChanged -= Squad_OnDrawUnitHpChanged;

            squad.OnUnitStatsChanged -= Squad_OnUnitStatsChanged;

            //если этот юнит атаковал, то уменьшаем кол во атаковавших
            if (attaking)
                squad.AttakingUnitsCount--;
            
            // убираем метку targetted с цели
            if (target != null)
                target.RemoveTagrettedBy(this);

            //ставим всем, кто атаковал этот юнит, цель null 
            for (int i = 0; i < targettedBy.Count; i++)
            {
                targettedBy[i].SetTarget(null);
                i--;
            }
            
            squad.UnitDeath(this);
        }

        if (DeatUnitsContainer == null)
        {
            var alldeads = GameObject.Find("AllDeadUnits");
            if (alldeads == null)
                alldeads = new GameObject("AllDeadUnits");
            DeatUnitsContainer = alldeads.transform;
        }

        ThisTransform.parent = DeatUnitsContainer;

        Selected = false;

        IsAlive = false;
    }

    void LateDeath()
    {
        if (OnUnitDeath != null)
            OnUnitDeath();

        if (OnAnyUnitDeath != null)
            OnAnyUnitDeath();

        Destroy(GetComponent<Collider2D>());
        Destroy(rigidbody2D);
        Destroy(this);
    }

    void Squad_OnUnitStatsChanged(UnitStats oldS, UnitStats newS)
    {
        if (Health > newS.Health)
            Health = newS.Health;
        else if(oldS.Health < newS.Health)
        {
            float k = Health / oldS.Health;
            Health = newS.Health * k;
        }

        SetProperties();
    }

    void Squad_OnBeginCharge(UnitStatsModifyer modifyer)
    {
        Charging = true;

        chargeRotation = squad.PositionsTransform.rotation;
        chargeRotation = squad.FlipRotation ? chargeRotation * Quaternion.Euler(0, 0, 180) : chargeRotation;

        //throw new NotImplementedException();

        AddStatsModifyer(modifyer);
    }

    void Squad_OnEndCharge(UnitStatsModifyer modifyer)
    {
        Charging = false;

        //throw new NotImplementedException();

        RemoveStatsModifyer(modifyer);
    }

    public void AddStatsModifyer(UnitStatsModifyer modifyer)
    {
        StatsModifyers.Add(modifyer);
        stats = UnitStats.ModifyStats(stats, modifyer);
    }

    /// <summary>
    /// Удаляет первый попавшийся модификаток, эквивалентный переданому, и отменяэт его действия с юнита
    /// </summary>
    /// <param name="modifyer"></param>
    public void RemoveStatsModifyer(UnitStatsModifyer modifyer)
    {
        bool removed = false;
        int cnt = StatsModifyers.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (StatsModifyers[i].Equals(modifyer))
            {
                StatsModifyers.RemoveAt(i);
                removed = true;
                break;
            }
        }

        if (removed)
        {
            stats = UnitStats.ModifyStats(stats, modifyer, UnitStatsModifyer.UseType.REJECT);
        }
        else
        {
            Debug.Log("cant ramove modifyer");
        }
    }

    public void SetFraction(Squad.UnitFraction fraction)
    {
        gameObject.layer = LayerMask.NameToLayer(fraction.ToString());

        switch (fraction)
        {
            case Squad.UnitFraction.ALLY:
                rHitLayerEnemy = 1 << LayerMask.NameToLayer("ENEMY");
                Selected = true;
                break;
            case Squad.UnitFraction.ENEMY:
                rHitLayerEnemy = 1 << LayerMask.NameToLayer("ALLY");
                break;
        }

        
    }

    void Squad_OnInitiateUnitPositions()
    {
        if (squad != null)
        {
            ThisTransform.position = TargetMovePositionObject.transform.position;
            ThisTransform.rotation = squad.PositionsTransform.rotation;
        }
    }

    void Squad_OnDrawUnitHpChanged(bool newValue)
    {
        selection.SetActive(newValue);
        OnHealthChanged(newValue);
    }

    void OnHealthChanged(bool drawHp)
    {
        Color color = Color.white;
        if (drawHp)
            color = GetColorByHealth();
        selectionRenderer.color = color;
    }

    public Color GetColorByHealth()
    {
        Color color = Color.white;
        if (squad != null)
        {
            float t = Health / squad.UnitStats.Health;
            if (t >= 0.5f)
                color = Color.Lerp(Color.yellow, Color.green, t * 2 - 1 - 0.3f);
            else
                color = Color.Lerp(Color.red, Color.yellow, t * 2 - 0.3f);
        }
        return color;
    }

    internal void SetSelectionColor(Color color)
    {
        selection.GetComponent<SpriteRenderer>().color = color;
    }

    void SetEquipment()
    {
        SetHelmet(squad.Inventory.Helmet);
        SetBody(squad.Inventory.Body);
        SetShield(squad.Inventory.Shield);
        SetWeapon(squad.Inventory.Weapon);
    }

    void SetWeapon(EquipmentStack weapon)
    {
        Destroy(Weapon);
        Weapon = Instantiate(Resources.Load<GameObject>(weapon.EquipmentMainProperties.PathToPrefab), ThisTransform.position, ThisTransform.rotation, ThisTransform);
        Weapon.name = "Weapon";
        this.weapon = (Weapon.transform.GetComponent<Item>() as Equipment);
        this.weapon.Stats = weapon.EquipmentStats;

        if(!this.weapon.Stats.CanReformToPhalanx)
        {
            SetArms(arms_with_sword);
        }
        else if(!this.weapon.Stats.CanReformToPhalanxInFight)
        {
            SetArms(arms_with_pike);
        }
        else
        {
            SetArms(arms_with_spear);
        }
    }

    void SetShield(EquipmentStack shield)
    {
        Destroy(Shield);
        Shield = Instantiate(Resources.Load<GameObject>(shield.EquipmentMainProperties.PathToPrefab), ThisTransform.position, ThisTransform.rotation, ThisTransform);
        Shield.name = "Shield";
        this.shield = (Shield.transform.GetComponent<Item>() as Equipment);
        this.shield.Stats = shield.EquipmentStats;
    }

    public void SetBody(EquipmentStack body)
    {
        Destroy(Body);
        Body = Instantiate(Resources.Load<GameObject>(body.EquipmentMainProperties.PathToPrefab), ThisTransform.position, ThisTransform.rotation, ThisTransform);
        Body.name = "Body";
        this.body = (Body.transform.GetComponent<Item>() as Equipment);
        this.body.Stats = body.EquipmentStats;
    }

    public void SetHelmet(EquipmentStack helmet)
    {
        Destroy(Helmet);
        Helmet = Instantiate(Resources.Load<GameObject>(helmet.EquipmentMainProperties.PathToPrefab), ThisTransform.position, ThisTransform.rotation, ThisTransform);
        Helmet.name = "Helmet";
        this.helmet = (Helmet.transform.GetComponent<Item>() as Equipment);
        this.helmet.Stats = helmet.EquipmentStats;
    }

    void SetArms(GameObject arms)
    {
        Destroy(Arms);
        Arms = Instantiate(arms, ThisTransform.position, ThisTransform.rotation, ThisTransform);
        Arms.name = "Arms";
    }





    /// <summary>
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
    ///    ТУТ СКОРЕЕ ВСЕГО БАГ!!!!
    ///    ЕСЛИ ПРИМЕНИТЬ МОДИФИКАТОРЫ К ОТРЯДУ, А ПОТОМ ВЫЗВАТЬ ДАНЫЙ МЕТОД
    ///    ТО МОДИФИКАТОРЫ НЕ БУДУТ УЧТЕНЫ!!!
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
    /// </summary>
    void SetProperties()
    {
        EquipmentStats[] equipStats = new EquipmentStats[]
        {
            squad.Inventory.Helmet.EquipmentStats,
            squad.Inventory.Body.EquipmentStats,
            squad.Inventory.Shield.EquipmentStats,
            squad.Inventory.Weapon.EquipmentStats
        };

        float health = stats.Health;
        stats = UnitStats.CalcStats(squad.UnitStats, equipStats, squad.CurrentFormationModifyers);
        if(health > 0)//без проверки юниты умирают при инициализации
            stats.Health = health;
        rigidbody2D.mass = stats.Mass;
    }






    RaycastHit2D Cast(int layers)
    {
        SetDistanceLineForAttack();

        //rHit = Physics2D.Linecast(startRay.position, endRay.position, rHitLayer);                

        return Physics2D.BoxCast(
            startRay.position + (endRay.position - startRay.position) / 2,
            new Vector2(attackLineWidth, attackLineLength),
            Quaternion.LookRotation(endRay.position - startRay.position).eulerAngles.z,
            endRay.position - startRay.position,
            0,
            layers
        );
    }

    void SetDistanceLineForAttack()
    {
        endRay.localPosition = new Vector3(endRay.localPosition.x, stats.AttackDistance, endRay.localPosition.z);
        startRay.localPosition = Vector3.MoveTowards(endRay.localPosition, Vector3.zero, attackLineLength);
        startRay.localPosition = new Vector3(startRay.localPosition.x, startRay.localPosition.y, endRay.localPosition.z);
    }

    float timerForCheckEquipment = 0;

    void Update()
    {
        float deltaTime = Time.deltaTime;
        shadow.rotation = Quaternion.identity;

        if (IsAlive)
        {
            if (squad != null)
            {
                if (timerForCheckEquipment < 0.5f)
                    timerForCheckEquipment += deltaTime;
                else
                    CheckEquipment();

                squad.SumPositionUnit(this);
            }

            if (olreadyNotPushingAlly && pushedUnit.Count == 0)
            {
                if (timerPushingAlly < delayAfterPushingAlly)
                {
                    timerPushingAlly += deltaTime;
                }
                else
                {
                    timerPushingAlly = 0;
                    pushingAlly = false;
                }
            }
            else
            {
                olreadyNotPushingAlly = false;
            }

            if (delayToFindTargetAndAttack <= 0)
            {
                if (!stanned && !pushingAlly)
                {
                    if (timerForAttack < currentAttackDeltaTime)
                    {
                        timerForAttack += deltaTime;
                        if (Hit) Hit = false;
                    }
                    else
                        AttackTarget();

                    if (timerForFindTarget < findTargetDeltaTime)
                        timerForFindTarget += deltaTime;
                    else
                        FindTarget();
                }

                if (timerForUnion < unionCheckDeltatime)
                    timerForUnion += deltaTime;
                else
                    CheckToUnion();
            }
            else
            {
                delayToFindTargetAndAttack -= deltaTime;
            }
        }
        else
        {
            if (timerForLateDeath < timeForDeath)
                timerForLateDeath += deltaTime;
            else
                LateDeath();
        }
    }

    void CheckToUnion()
    {
        timerForUnion = 0;
        if (selected && squad == null)
        {
            if (Squad.playerSquadInstance.UnitCount < Squad.playerSquadInstance.FULL_SQUAD_UNIT_COUNT && Squad.playerSquadInstance.CurrentFormation == FormationStats.Formations.RANKS)
            {
                float dist = Vector2.Distance(ThisTransform.position, Squad.playerSquadInstance.CenterSquad);
                if (dist <= Squad.playerSquadInstance.DistanceToUnionWithSquad && !Squad.playerSquadInstance.InFight)
                    Squad.playerSquadInstance.AddUnit(this);
            }
        }
    }

    void FindTarget()
    {
        timerForFindTarget = 0;

        if (squad != null)
        {
            Unit unit = null;
            rHit = Cast(rHitLayerEnemy);
            if(rHit)
                unit = rHit.transform.gameObject.GetComponent<Unit>();

            switch (squad.CurrentFormation)
            {
                case FormationStats.Formations.PHALANX:
                    if (rHit)
                        SetTarget(unit);
                    else
                        SetTarget(null);

                    break;

                default: // Formation.Formations.RANKS:
                    if (target == null)
                    {
                        if (rHit)
                        {
                            if (unit.selected && unit.IsAlive)
                                SetTarget(unit);
                        }
                    }
                    else
                    {
                        float dist = Vector2.Distance(target.ThisTransform.position, ThisTransform.position);
                        if (!target.IsAlive || dist > stats.AttackDistance || !target.Selected)
                            SetTarget(null);
                    }

                    break;
            }
        }
    }

    void AttackTarget()
    {
        timerForAttack = 0;
        if (target != null)
        {
            target.TakeHitFromUnit(this);
            SetAttakingFlag(true);
            Hit = true;
        }
        else
        {
            SetAttakingFlag(false);
        }
    }

    void SetAttakingFlag(bool flag)
    {
        if (flag && !attaking)
            squad.AttakingUnitsCount++;
        else if (!flag && attaking)
            squad.AttakingUnitsCount--;

        attaking = flag;
    }

    void OnReformSquad(FormationStats.Formations newFormation)
    {
        switch (newFormation)
        {
            case FormationStats.Formations.RANKS:
                OnReformInRanks();
                break;
            case FormationStats.Formations.PHALANX:
                OnReformInPhalanx();
                break;
            case FormationStats.Formations.RISEDSHIELDS:
                break;
        }
    }

    void OnReformInRanks()
    {
        RotateBody(0);
    }

    void OnReformInPhalanx()
    {
        int f = 0;
        if (TargetMovePositionObject.RowInPhalanx % 2 == 0)
            f = 1;
        else if (TargetMovePositionObject.RowInPhalanx % 3 == 0)
            f = -1;

        RotateBody(f * weaponAngleDeviation);
    }

    void RotateBody(float angle)
    {
        var weapon = Weapon.transform;
        var hands = arms.transform;
        var body = Body.transform;

        weapon.localRotation = Quaternion.identity * Quaternion.Euler(0, 0, angle);
        hands.localRotation = Quaternion.identity * Quaternion.Euler(0, 0, angle);
        body.localRotation = Quaternion.identity * Quaternion.Euler(0, 0, angle);

        float sin = Mathf.Sin(-angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(-angle * Mathf.Deg2Rad);

        Vector2 p = endRay.localPosition;
        endRay.localPosition = new Vector3(
            p.magnitude * sin,
            p.magnitude * cos,
            endRay.localPosition.z
        );
    }

    void OnDropStack(EquipmentStack stack)
    {
        if (stack.Count > 0)
        {            
            stack.PopItems(1);

            if(DroppedItemsContainer == null)
            {
                var dropitcont = GameObject.Find("DroppedItemsContainer");
                if (dropitcont == null)
                    dropitcont = new GameObject("DroppedItemsContainer");
                DroppedItemsContainer = dropitcont.transform;
            }

            DroppedItem di = Instantiate(DroppedItemOriginal, ThisTransform.position, ThisTransform.rotation, DroppedItemsContainer).GetComponent<DroppedItem>();
            di.Stack = new EquipmentStack(stack.EquipmentMainProperties, stack.EquipmentStats);
        }
    }

    void CheckEquipment()
    {
        timerForCheckEquipment = 0;
        bool equipChanged = false;
        if (!helmet.Stats.Equals(squad.Inventory.Helmet.EquipmentStats))
        {
            SetHelmet(squad.Inventory.Helmet);
            equipChanged = true;
        }
        if (!body.Stats.Equals(squad.Inventory.Body.EquipmentStats))
        {
            SetBody(squad.Inventory.Body);
            equipChanged = true;
        }
        if (!shield.Stats.Equals(squad.Inventory.Shield.EquipmentStats))
        {
            SetShield(squad.Inventory.Shield);
            equipChanged = true;
        }
        if (!weapon.Stats.Equals(squad.Inventory.Weapon.EquipmentStats))
        {
            SetWeapon(squad.Inventory.Weapon);
            equipChanged = true;
        }

        if (equipChanged)
            SetProperties();
    }

    void OnFormationChanged(FormationStats.Formations newFormation)
    {
        SetProperties();

        if (squad.CurrentFormation == FormationStats.Formations.PHALANX)
        {
            currentAttackDeltaTime = attacInPhalanxDeltaTime;

            pushingAlly = false;
            pushedUnit.Clear();
        }
        else if (squad.CurrentFormation == FormationStats.Formations.RANKS)
        {
            currentAttackDeltaTime = attacInRanksDeltaTime;
        }
    }

    void FixedUpdate()
    {
        if (IsAlive)
        {
            if (squad != null)
                Moving();

            if (Running && !pushingAlly && !stanned)
            {
                if (currentSpeed < stats.Speed)
                    currentSpeed += stats.Acceleration * Time.deltaTime;
                else
                    currentSpeed = stats.Speed;
            }
            else
            {
                currentSpeed = 0;
            }
        }
    }

    void TakeHitFromUnit(Unit enemy)
    {
        if (IsAlive)
        {
            Quaternion enemyRot = Quaternion.LookRotation(Vector3.forward, enemy.ThisTransform.position - ThisTransform.position);
            float rot = enemyRot.eulerAngles.z - ThisTransform.rotation.eulerAngles.z;
            rot = rot < 0 ? -rot : rot;

            // if it attack from back
            if (rot > stats.DefenceHalfSector && rot < 360 - stats.DefenceHalfSector)
            {
                takeHit = enemy.stats.Attack;
            }
            else
            {
                if (squad.CurrentFormation == FormationStats.Formations.PHALANX || target != null)
                    takeHit = enemy.stats.Attack * (1 - stats.Defence);
                else
                    //если нет цели и не фаланга (т.е. отряд пытается пробежать насквозь другой отряд)
                    //то не защищаться
                    takeHit = enemy.stats.Attack * (1 - stats.Defence * stats.DefenceGoingThrought);
            }

            if (squad.CurrentFormation == FormationStats.Formations.RANKS && target == null)
            {
                if (enemy.Selected && Vector2.Distance(ThisTransform.position, enemy.ThisTransform.position) <= stats.AttackDistance)
                    SetTarget(enemy);
            }

            float dmg = 0;
            //если враг попал по нам
            if (UnityEngine.Random.value <= takeHit)
            {
                dmg = enemy.stats.Damage.BaseDamage + enemy.stats.Damage.ArmourDamage - stats.Armour;
                if (dmg < enemy.stats.Damage.ArmourDamage)
                    dmg = enemy.stats.Damage.ArmourDamage;

                TakeDamage(dmg, enemy.squad, enemy);
            }
        }
    }

    public void TakeHitFromArrow(Damage damage, Vector2 arrowStartPosition, Squad owner)
    {
        if (IsAlive)
        {
            float dmg = 0;

            Quaternion enemyRot = Quaternion.LookRotation(Vector3.forward, (Vector3)arrowStartPosition - ThisTransform.position);
            float rot = enemyRot.eulerAngles.z - ThisTransform.rotation.eulerAngles.z;
            rot = rot < 0 ? -rot : rot;

            float chanceTohit = 1 - stats.MissileBlock;

            //если стрелы летят в спину
            if (!(rot <= stats.DefenceHalfSector || rot >= 360 - stats.DefenceHalfSector))
            {
                //если есть щит, то увеличиваем шанс попадания (так как мы уже учли щит в статах)
                if (squad != null && !squad.Inventory.Shield.EquipmentStats.Empty)
                {
                    var shieldMissileBlock = (squad.Inventory.Shield).EquipmentStats.MissileBlock;

                    if (squad.CurrentFormation != FormationStats.Formations.RISEDSHIELDS)
                        shieldMissileBlock *= 0.5f;

                    chanceTohit += shieldMissileBlock;
                }
            }

            if (UnityEngine.Random.value <= chanceTohit)
            {
                dmg = damage.BaseDamage + damage.ArmourDamage - stats.Armour;

                //если щиты подняты, то учитываем их броню
                if (squad != null && squad.CurrentFormation == FormationStats.Formations.RISEDSHIELDS)
                    dmg -= (squad.Inventory.Shield).EquipmentStats.Armour;

                if (dmg < damage.ArmourDamage)
                    dmg = damage.ArmourDamage;

                TakeDamage(dmg, owner);
            }
            else
            {
                bool friendlyfire = false;
                if (gameObject.layer != LayerMask.NameToLayer(Squad.UnitFraction.ENEMY.ToString()) && owner == Squad.playerSquadInstance)
                    friendlyfire = true;
                ShowPopUpTakenDamageText("block", friendlyfire);
            }
        }
    }

    /// <summary>
    /// Нанести удар по юниту.
    /// <para>Урон будет расчитан с учетом брони юнита, но без учета вероятности попадания по нему (его параметра защиты).</para>
    /// </summary>
    /// <param name="damage"></param>
    public void TakeHit(Damage damage)
    {
        if (IsAlive)
        {
            float dmg = damage.BaseDamage + damage.ArmourDamage - stats.Armour;

            if (dmg < damage.ArmourDamage)
                dmg = damage.ArmourDamage;

            TakeDamage(dmg, null);
        }
    }

    void TakeDamage(float damage, Squad owner, Unit enemy = null)
    {
        if (IsAlive)
        {
            if (Health - damage < 0)
                damage = Health;

            SoundManager.Instance.PlaySound(
                new SoundChannel.ClipSet(
                    SoundManager.Instance.SoundClipsContainer.FX.TakeDamage[UnityEngine.Random.Range(0, SoundManager.Instance.SoundClipsContainer.FX.TakeDamage.Length - 1)],
                    false,
                    0.2f
                ),
                SoundManager.SoundType.FX
            );

            Health -= damage;

            AfterTakingDamage(damage, owner);
        }
    }

    public void Heal(float health)
    {
        if (health > 0)
        {
            stats.Health += health;
            squad.SquadHealth += health;
        }
    }

    void AddTargettedBy(Unit unit)
    {
        if (targettedBy.Count == 0)
            squad.TargettedUnitsCount++;
        targettedBy.Add(unit);
    }

    void RemoveTagrettedBy(Unit unit)
    {
        targettedBy.Remove(unit);
        if (targettedBy.Count == 0)
            squad.TargettedUnitsCount--;
    }

    void SetTarget(Unit newTarget)
    {
        if (target == null && newTarget != null)
        {
            newTarget.AddTargettedBy(this);
        }
        else if (target != null && newTarget != null)
        {
            target.RemoveTagrettedBy(this);
            newTarget.AddTargettedBy(this);
        }
        else if (target != null && newTarget == null)
        {
            target.RemoveTagrettedBy(this);
        }

        target = newTarget;
    }

    /// <summary>
    /// События после получения урона
    /// </summary>
    /// <param name="enemy">Вгар, который нанес урон</param>
    /// /// <param name="dmg">Урон, который нанес враг</param>
    void AfterTakingDamage(float dmg, Squad owner)
    {
        if (squad != Squad.playerSquadInstance && owner == Squad.playerSquadInstance)
            GameManager.Instance.PlayerProgress.Score.expirience.Value += (int)dmg;

        bool friendlyfire = false;
        if (gameObject.layer != LayerMask.NameToLayer(Squad.UnitFraction.ENEMY.ToString()) && owner == Squad.playerSquadInstance)
            friendlyfire = true;

        ShowPopUpTakenDamageText(dmg.ToString("0.#"), friendlyfire);

        if (squad == null || squad.CurrentFormation != FormationStats.Formations.PHALANX)
            TakeStan();
    }

    void ShowPopUpTakenDamageText(string text, bool friendly = false)
    {
        if (GameManager.Instance.Settings.graphixSettings.ShowDamage)
        {
            Color color;

            if (friendly)
                color = GameManager.Instance.Settings.graphixSettings.FrieldlyDamageColor;
            else
                color = GameManager.Instance.Settings.graphixSettings.DamageToAllyColor;

            if (gameObject.layer == LayerMask.NameToLayer("ENEMY"))
                color = GameManager.Instance.Settings.graphixSettings.DamageToEnemyColor;

            Color endColor = color;
            endColor.a = 0.5f;

            PopUpTextController.Instance.AddTextLabel(
                text,
                ThisTransform.position,
                2,
                new Vector2(0, 2),
                color,
                endColor
            );
        }
    }

    public void TakeStan(float a = 1)
    {
        if (a > 1 || a < 0)
            throw new Exception("Коефициент a дожен быть в отрезке [0, 1]. Переданное значение: " + a);

        Running = false;
        Charging = false;

        StartCoroutine(Stan(delayAfterTakingDamage * a));
    }

    /// <summary>
    /// После получения урона, юнит будет застанен.
    /// Перед тем как позволить ему снова двигаться пройдет указанная задержка.
    /// </summary>
    /// <returns></returns>
    IEnumerator Stan(float delay)
    {
        stanned = true;
        yield return new WaitForSeconds(delay);
        stanned = false;

        gameObject.layer = normLayer;
    }

    void MovingToTarget()
    {
        LookTo(calcTargetRotations(target.ThisTransform.position));

        if (Vector2.Distance(ThisTransform.position, target.ThisTransform.position) > stats.AttackDistance)
        {
            GoTo(ThisTransform.up.normalized * CurrentSpeed * Time.fixedDeltaTime);
            Running = true;
        }
        else
        {
            Running = false;
        }
    }

    void Moving()
    {
        if (!stanned && !pushingAlly) // squad != null
        {
            switch (squad.CurrentFormation)
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
    }

    void MovingInRiseshields()
    {
        MovingInRanks();
    }

    void MovingInPhalanx()
    {
        if (target != null)
        {
            //отталкиваем врага
            target.rigidbody2D.velocity = Vector2.zero;
            target.GoTo(ThisTransform.up * (target.stats.Speed + currentSpeed));
            //target.rigidbody2D.AddForce(
            //    ThisTransform.up * (target.speed + currentSpeed), 
            //    ForceMode2D.Force
            //);
        }

        //юнит смотрит туда куда и отряд
        //if (squad.FlipRotation)
        //    LookTo(squad.PositionsTransform.rotation * Quaternion.Euler(0, 0, 180));
        //else
        //    LookTo(squad.PositionsTransform.rotation);

        //юнит смотрит туда куда должен смотреть отряд
        LookTo(squad.EndLookRotation);

        if (!CheckPositionInRange(TargetMovePositionObject.transform.position))
        {
            Vector3 tPos = TargetMovePositionObject.transform.position - ThisTransform.position;
            GoTo(tPos.normalized * CurrentSpeed);
            Running = true;
        }
        else
        {
            Running = false;
        }
    }

    void MovingInRanks()
    {
        if (Charging)
        {
            ThisTransform.rotation = chargeRotation;
            GoTo(ThisTransform.up * CurrentSpeed);

            Running = true;
        }
        else
        {
            if (target != null)
            {
                MovingToTarget();
            }
            else
            {
                if (!CheckPositionInRange(TargetMovePositionObject.transform.position))
                {
                    LookTo(calcTargetRotations(TargetMovePositionObject.transform.position));
                    GoTo(ThisTransform.up * CurrentSpeed);

                    Running = true;
                }
                else
                {
                    Running = false;
                }
            }
        }
    }

    Quaternion calcTargetRotations(Vector3 targetPosition)
    {
        return Quaternion.LookRotation(Vector3.forward, targetPosition - ThisTransform.position);
    }

    bool CheckRotationInRange(Quaternion targetRotation)
    {
        return Math.Abs(ThisTransform.rotation.eulerAngles.z - targetRotation.eulerAngles.z) <= rotationRange;
    }

    bool CheckPositionInRange(Vector3 targetPosition)
    {
        float movingRangeSqr = movingRange * movingRange;
        return Vector2.SqrMagnitude(ThisTransform.position - targetPosition) <= movingRangeSqr;
    }

    void GoTo(Vector2 force)
    {
        rigidbody2D.AddForce(force * rigidbody2D.mass, ForceMode2D.Force);
    }

    void LookTo(Quaternion lookRotation)
    {
        ThisTransform.rotation = Quaternion.RotateTowards(ThisTransform.rotation, lookRotation, stats.RotationSpeed * Time.deltaTime);
    }
    
    void OnPushStart(Collision2D collision)
    {
        if (squad != null && squad.CurrentFormation == FormationStats.Formations.RANKS && squad.InFight)
        {
            if (target != null)
                return;

            if (collision.gameObject.layer != gameObject.layer)
                return;

            Unit collisionUnit = collision.transform.GetComponent<Unit>();

            if (ThisTransform.position == collisionUnit.ThisTransform.position)
                return;

            //чтоб смотрели не друг на друга
            float subRot = Mathf.Abs(ThisTransform.rotation.eulerAngles.z - collisionUnit.ThisTransform.rotation.eulerAngles.z);
            if (subRot >= 180 - frontPushingAngleException / 2 && subRot <= 180 + frontPushingAngleException / 2)
                return;

            //чтоб коллизия была спереди
            float rot = Quaternion.LookRotation(Vector3.forward, collisionUnit.ThisTransform.position - ThisTransform.position).eulerAngles.z;
            float sub = Mathf.Abs(ThisTransform.rotation.eulerAngles.z - rot);
            if (sub >= pushingAngle / 2 && sub <= 360 - pushingAngle / 2)
                return;

            //чтоб проверить что this находится сзади коллизии
            float rot2 = Quaternion.LookRotation(Vector3.forward, ThisTransform.position - collisionUnit.ThisTransform.position).eulerAngles.z;


            //ошибка в том что rot2 > rot в 50% случаев а не в 100%

            if (ThisTransform.rotation.eulerAngles.z >= 180 && rot < rot2)
                return;

            if (ThisTransform.rotation.eulerAngles.z < 180 && rot > rot2)
                return;

            pushingAlly = true;
            rigidbody2D.velocity = Vector2.zero;
            pushedUnit.Add(collisionUnit);
        }
    }

    void OnPushEnd(Collision2D collision)
    {
        if (squad != null && squad.CurrentFormation == FormationStats.Formations.RANKS)
        {
            if (collision.gameObject.layer != gameObject.layer)
                return;

            Unit collisionUnit = collision.transform.GetComponent<Unit>();

            if (!pushedUnit.Contains(collisionUnit))
                return;

            pushedUnit.Remove(collisionUnit);
            if (pushedUnit.Count > 0)
                return;

            olreadyNotPushingAlly = true;
        }
    }

    void OnChargeHit(Collision2D collision)
    {
        float k = currentSpeed / stats.Speed;
        k = k > 1 ? 1 : k;

        if (collision.gameObject.layer == LayerMask.NameToLayer("MAP") || collision.gameObject.layer == LayerMask.NameToLayer("WALL"))
        {
            float dmg = k * weapon.Stats.Damag.BaseDamage * (1 + stats.ChargeAddDamage);
            TakeDamage(dmg, squad);
        }
        else
        {
            Unit unit = collision.transform.GetComponent<Unit>();
            if(unit != null)
            {
                //чтоб коллизия была спереди
                float rot = Quaternion.LookRotation(Vector3.forward, unit.ThisTransform.position - ThisTransform.position).eulerAngles.z;
                float sub = Mathf.Abs(ThisTransform.rotation.eulerAngles.z - rot);
                if (sub >= chargePpushingAngle / 2 && sub <= 360 - chargePpushingAngle / 2)
                    return;
                
                //союзники
                if (unit.gameObject.layer == gameObject.layer)
                {
                    if (unit.squad != squad || !unit.Charging)
                    {
                        TakeStan(k);
                    }
                }
                //враги и нейтралы
                else
                {
                    float impact = stats.ChargeImpact - unit.stats.ChargeDeflect;
                    float dmg = (stats.Damage.BaseDamage + stats.Damage.ArmourDamage) * (1 + stats.ChargeAddDamage) * k;
                    dmg = dmg < 0 ? -dmg : dmg;

                    dmg -= unit.stats.Armour;
                    if (dmg < stats.Damage.ArmourDamage)
                        dmg = stats.Damage.ArmourDamage;
                    
                    if (impact >= 0)
                    {
                        //тут надо правильно рассчитать силу импульса
                        unit.GoTo(ThisTransform.up * (impact * currentSpeed) / Time.deltaTime / 5);
                        unit.TakeDamage(dmg, squad, this);
                        unit.gameObject.layer = LayerMask.NameToLayer("FALLEN_UNIT");
                    }
                    else if(impact < 0)
                    {
                        TakeDamage(dmg, unit.squad, unit);
                    }

                    //if (rigidbody2D.velocity.sqrMagnitude < 7)
                    Charging = false;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnPushStart(collision);

        if (Charging)
            OnChargeHit(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnPushEnd(collision);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(ThisTransform.position, startRay.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(startRay.position, endRay.position);

        Gizmos.color = Color.yellow;
        if (pushingAlly)
            Gizmos.DrawSphere(ThisTransform.position, 0.4f);

        Gizmos.color = Color.gray;
        if (targettedBy.Count > 0)
            Gizmos.DrawSphere(ThisTransform.position, 0.3f);

        Gizmos.color = Color.blue;
        if (attaking)
            Gizmos.DrawSphere(ThisTransform.position, 0.2f);

        Gizmos.color = Color.red;
        if (stanned)
            Gizmos.DrawSphere(ThisTransform.position, 0.1f);
    }

    //for debug
    //private void OnMouseDown()
    //{
    //    Health -= 10;
    //}
}