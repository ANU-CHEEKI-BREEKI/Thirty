using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    static public event Action OnAnyUnitDeath;

    [HideInInspector] public UnitPosition TargetMovePositionObject;

    [HideInInspector] public Squad squad;

    [SerializeField] UnitPosition unitPosOriginal;

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
    [Tooltip("Когда юнит получает удар натиском и упадет, он будет застанет на данный промежуток времени.")]
    [SerializeField] [Range(0, 5)] float delayAfterFalling = 3f;
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

    List<UnitStatsModifier> statsModifyers = new List<UnitStatsModifier>();
    public event Action<UnitStatsModifier> OnModifierAdd;
    public event Action<UnitStatsModifier> OnModifierRemove;

    List<SOTerrainStatsModifier> terrainStatsModifyers = new List<SOTerrainStatsModifier>();
    public event Action<SOTerrainStatsModifier> OnTerrainModifierAdd;
    public event Action<SOTerrainStatsModifier> OnTerrainModifierRemove;

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

    bool fallen = false;
    public bool Fallen { get { return fallen; } }

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
    float takeHitChanse = 0;

    //текущая эффективность натиска
    float currentChargeImpact = 0;

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
    bool Attaking
    {
        get { return attaking; }
        set
        {
            if (value != attaking)
            {
                attaking = value;
                if (attaking)
                    squad.AttakingUnitsCount++;
                else
                    squad.AttakingUnitsCount--;
            }
        }
    }
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
    public event Action<Unit> OnUnitDeath;

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

    public event Action OnUnitFallDown;
    public event Action OnUnitStandUp;

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
            OnFormationChanged(squad.CurrentFormationModifyers);

            squad.OnDropStackFromInventory += OnDropStack;

            squad.OnReformSquad += OnReformSquad;
            OnReformSquad(squad.CurrentFormation);

            squad.OnInitiateUnitPositions += Squad_OnInitiateUnitPositions;

            squad.OnBeginCharge += Squad_OnBeginCharge;
            squad.OnEndCharge += Squad_OnEndCharge;

            squad.OnDrawUnitHpChanged += Squad_OnDrawUnitHpChanged;

            squad.OnUnitStatsChanged += Squad_OnUnitStatsChanged;

            if (gameObject.layer != LayerMask.NameToLayer(Squad.UnitFraction.ENEMY.ToString()))
                Selected = true;

            normLayer = gameObject.layer;

            squad.OnCallApplyModifierToAllUnit += OnCallApplyModifierToAll;
            squad.OnCallRejectModifierToAllUnit += OnCallRejectModifierToAll;
            squad.OnCallApplyTerrainModifierToAllUnit += OnCallApplyTerrainModifierToAll;
            squad.OnCallRejectTerrainModifierToAllUnit += OnCallRejectTerrainModifierToAll;
        }

        if (AfterInitiate != null) AfterInitiate();
    }

    public void Death()
    {
        if (squad != null)
        {
            DropEquipment();

            squad.OnFormationChanged -= OnFormationChanged;
            squad.OnDropStackFromInventory -= OnDropStack;
            squad.OnReformSquad -= OnReformSquad;
            squad.OnInitiateUnitPositions -= Squad_OnInitiateUnitPositions;

            squad.OnBeginCharge -= Squad_OnBeginCharge;
            squad.OnEndCharge -= Squad_OnEndCharge;

            squad.OnDrawUnitHpChanged -= Squad_OnDrawUnitHpChanged;

            squad.OnUnitStatsChanged -= Squad_OnUnitStatsChanged;


            squad.OnCallApplyModifierToAllUnit -= OnCallApplyModifierToAll;
            squad.OnCallRejectModifierToAllUnit -= OnCallRejectModifierToAll;
            squad.OnCallApplyTerrainModifierToAllUnit -= OnCallApplyTerrainModifierToAll;
            squad.OnCallRejectTerrainModifierToAllUnit -= OnCallRejectTerrainModifierToAll;

            //если этот юнит атаковал, то уменьшаем кол во атаковавших
            Attaking = false;

            // убираем метку targetted с цели
            if (target != null)
                target.RemoveTagrettedBy(this);

            //ставим всем, кто атаковал этот юнит, цель null 
            for (int i = 0; i < targettedBy.Count; i++)
                targettedBy[i].SetTarget(null);

            {//это всё должно идти именно в таком порядке!!!
             //сначала обрабатываем модификаторы,а потом уже указываем отряду что этот юнит умер
                foreach (var mod in statsModifyers)
                    RemoveStatsModifyer(mod);

                foreach (var tmod in terrainStatsModifyers)
                    RemoveTerrainStatsModifyer(tmod);

                squad.UnitDeath(this);
            }
        }

        DropingItemsManager.Instance.DropUnitCorp(this);

        Selected = false;

        IsAlive = false;
    }

    private void DropEquipment()
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
    }

    void LateDeath()
    {
        Destroy(GetComponent<Collider2D>());
        Destroy(rigidbody2D);

        if (OnUnitDeath != null)
            OnUnitDeath(this);

        if (OnAnyUnitDeath != null)
            OnAnyUnitDeath();
        
        

        Destroy(this);
    }

    void Squad_OnUnitStatsChanged(UnitStats oldS, UnitStats newS)
    {
        if (Health > newS.Health)
            Health = newS.Health;
        else if (oldS.Health < newS.Health)
        {
            float k = Health / oldS.Health;
            Health = newS.Health * k;
        }

        SetProperties();
    }

    void Squad_OnBeginCharge(UnitStatsModifier modifyer)
    {
        Charging = true;

        chargeRotation = squad.PositionsTransform.rotation;
        chargeRotation = squad.FlipRotation ? chargeRotation * Quaternion.Euler(0, 0, 180) : chargeRotation;

        AddStatsModifyer(modifyer);

        currentChargeImpact = stats.ChargeImpact;
    }

    void Squad_OnEndCharge(UnitStatsModifier modifyer)
    {
        Charging = false;

        RemoveStatsModifyer(modifyer);
    }

    public void AddStatsModifyer(UnitStatsModifier modifier)
    {
        //if (!statsModifyers.Contains(modifier))
        //{
        //    statsModifyers.Add(modifier);
        //    stats = UnitStats.ModifyStats(stats, modifier);
        //    Debug.Log("Модификатор добавлен");
           
        //}
        //else
        //{
        //    Debug.Log("Такой модификатор уже есть");
        //}

        if (OnModifierAdd != null)
            OnModifierAdd(modifier);
    }

    /// <summary>
    /// Удаляет модификаток, эквивалентный переданому, и отменяэт его действия с юнита
    /// </summary>
    /// <param name="modifier"></param>
    public void RemoveStatsModifyer(UnitStatsModifier modifier)
    {
        //if (statsModifyers.Remove(modifier))
        //{
        //    stats = UnitStats.ModifyStats(stats, modifier, UnitStatsModifier.UseType.REJECT);
        //    Debug.Log("Модификатор удален");
        //}
        //else
        //{
        //    Debug.Log("Такого модификатора нет");
        //}

        if (OnModifierRemove != null)
            OnModifierRemove(modifier);
    }

    public void AddTerrainStatsModifyer(SOTerrainStatsModifier modifier)
    {
        //if (!terrainStatsModifyers.Contains(modifier))
        //{
        //    terrainStatsModifyers.Add(modifier);
        //    stats = UnitStats.ModifyStats(stats, modifier.GetModifierByEquipmentMass(stats.EquipmentMass));
        //    Debug.Log("Модификатор ландшафта добавлен");
        //}
        if (OnTerrainModifierAdd != null)
            OnTerrainModifierAdd(modifier);
    }

    /// <summary>
    /// Удаляет модификаток, эквивалентный переданому, и отменяэт его действия с юнита
    /// </summary>
    /// <param name="modifier"></param>
    public void RemoveTerrainStatsModifyer(SOTerrainStatsModifier modifier)
    {
        //if (terrainStatsModifyers.Remove(modifier))
        //{
        //    stats = UnitStats.ModifyStats(stats, modifier.GetModifierByEquipmentMass(stats.EquipmentMass), UnitStatsModifier.UseType.REJECT);
        //    Debug.Log("Модификатор ландшафта удален");
        //}
        //else
        //{
        //    Debug.Log("Такого модификатора ландшафта нет");
        //}
        if (OnTerrainModifierRemove != null)
            OnTerrainModifierRemove(modifier);
    }

    void OnCallApplyModifierToAll(UnitStatsModifier modifier)
    {
        if (!statsModifyers.Contains(modifier))
        {
            statsModifyers.Add(modifier);
            stats = UnitStats.ModifyStats(stats, modifier);
        }
    }

    void OnCallRejectModifierToAll(UnitStatsModifier modifier)
    {
        if (statsModifyers.Remove(modifier))
        {
            stats = UnitStats.ModifyStats(stats, modifier, UnitStatsModifier.UseType.REJECT);
        }
    }

    void OnCallApplyTerrainModifierToAll(SOTerrainStatsModifier modifier)
    {
        if (!terrainStatsModifyers.Contains(modifier))
        {
            terrainStatsModifyers.Add(modifier);
            stats = UnitStats.ModifyStats(stats, modifier.GetModifierByEquipmentMass(stats.EquipmentMass));
        }
    }

    void OnCallRejectTerrainModifierToAll(SOTerrainStatsModifier modifier)
    {
        if (terrainStatsModifyers.Remove(modifier))
        {
            stats = UnitStats.ModifyStats(stats, modifier.GetModifierByEquipmentMass(stats.EquipmentMass), UnitStatsModifier.UseType.REJECT);
        }
    }

    public void SetFraction(Squad.UnitFraction fraction)
    {
        gameObject.layer = LayerMask.NameToLayer(fraction.ToString());

        switch (fraction)
        {
            case Squad.UnitFraction.ALLY:
                rHitLayerEnemy = 1 << LayerMask.NameToLayer("ENEMY");
                rHitLayerEnemy = rHitLayerEnemy | 1 << LayerMask.NameToLayer("FALLEN_ENEMY");
                Selected = true;
                break;
            case Squad.UnitFraction.ENEMY:
                rHitLayerEnemy = 1 << LayerMask.NameToLayer("ALLY");
                rHitLayerEnemy = rHitLayerEnemy | 1 << LayerMask.NameToLayer("FALLEN_ALLY");
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
            float t = Health / squad.DefaultUnitStats.Health;
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

    public void SetWeapon(EquipmentStack weapon)
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

    public void SetShield(EquipmentStack shield)
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
        stats = UnitStats.CalcStats(squad.DefaultUnitStats, equipStats, squad.CurrentFormationModifyers);

        foreach (var mod in statsModifyers)
            stats = UnitStats.ModifyStats(stats, mod);

        foreach (var mod in terrainStatsModifyers)
            stats = UnitStats.ModifyStats(stats, mod.GetModifierByEquipmentMass(stats.EquipmentMass));

        if (health > 0)//без проверки юниты умирают при инициализации
            stats.Health = health;
        rigidbody2D.mass = stats.EquipmentMass;
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

                //squad.SumPositionUnit(this);
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
                    {
                        if (unit.IsAlive)
                        {
                            SetTarget(unit);
                        }
                    }
                    else
                    {
                        SetTarget(null);
                    }

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
            Attaking = true;
            Hit = true;
        }
        else
        {
            Attaking = false;
        }
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
            DropingItemsManager.Instance.DropEquipment(new EquipmentStack(stack, 1), ThisTransform);
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

    void OnFormationChanged(FormationStats newFormation)
    {
        SetProperties();

        if (squad.CurrentFormation == FormationStats.Formations.PHALANX)
        {
            currentAttackDeltaTime = attacInPhalanxDeltaTime;

            pushingAlly = false;
            pushedUnit.Clear();
        }
        else //if (squad.CurrentFormation == FormationStats.Formations.RANKS)
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

            // если враг атакует в спину
            bool hitFromBack = !(rot <= stats.DefenceHalfSector || rot >= 360 - stats.DefenceHalfSector);

            if (hitFromBack || fallen)
            {
                takeHitChanse = enemy.stats.Attack;
            }
            else
            {
                if (squad.CurrentFormation == FormationStats.Formations.PHALANX || target != null)
                    takeHitChanse = enemy.stats.Attack * (1 - stats.Defence);
                else
                    //если нет цели и не фаланга (т.е. отряд пытается пробежать насквозь другой отряд)
                    //то не защищаться
                    takeHitChanse = enemy.stats.Attack * (1 - stats.Defence * stats.DefenceGoingThrought);
            }

            //если нет цели и это свободное построение, то пускай целью будет тот кто пытался ударить, если он выделенн
            if (squad.CurrentFormation == FormationStats.Formations.RANKS && target == null)
            {
                if (enemy.Selected && Vector2.Distance(ThisTransform.position, enemy.ThisTransform.position) <= stats.AttackDistance)
                    SetTarget(enemy);
            }

            float dmg = 0;
            //если враг попал по нам
            if (UnityEngine.Random.value <= takeHitChanse)
            {
                var damage = enemy.stats.Damage;

                dmg = damage.BaseDamage + damage.ArmourDamage - stats.Armour;

                //если попало в спину, то броню щита не учитываем
                if (hitFromBack && shield != null)
                    dmg += shield.Stats.Armour;

                if (dmg < damage.ArmourDamage)
                    dmg = damage.ArmourDamage;

                TakeDamage(dmg, enemy.squad, enemy);
            }
        }
    }

    /// <summary>
    /// Попытаться нанести урон метательным снарядом.
    /// <para>Расчет ведется с учётом вероятности блокировки.</para>
    /// <para>Если снаряд попадает в спину, то не учитывается щит (ни вероятность блока ни броня)</para>
    /// </summary>
    /// <param name="damage">Урон, который нужно нанести</param>
    /// <param name="arrowStartPosition">Откуда летит снаряд</param>
    /// <param name="owner">Кто инициировал атаку</param>
    public void TakeHitFromArrow(Damage damage, Vector2 arrowStartPosition, Squad owner)
    {
        if (IsAlive)
        {
            float dmg = 0;

            Quaternion enemyRot = Quaternion.LookRotation(Vector3.forward, (Vector3)arrowStartPosition - ThisTransform.position);
            float rot = enemyRot.eulerAngles.z - ThisTransform.rotation.eulerAngles.z;
            rot = rot < 0 ? -rot : rot;

            float chanceTohit = 1 - stats.MissileBlock;

            bool hitFromBack = !(rot <= stats.DefenceHalfSector || rot >= 360 - stats.DefenceHalfSector);

            //если стрелы летят в спину
            if (hitFromBack || fallen)
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

            //если стрела не была заблокирована
            if (UnityEngine.Random.value <= chanceTohit)
            {
                dmg = damage.BaseDamage + damage.ArmourDamage - stats.Armour;

                //если попало в спину, то броню щита не учитываем
                if (hitFromBack && shield != null)
                    dmg += shield.Stats.Armour;

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
    /// <para>!!!!Также, не учитывается броня щита!!!!</para>
    /// </summary>
    /// <param name="damage"></param>
    public void TakeHit(Damage damage)
    {
        if (IsAlive)
        {
            float dmg = damage.BaseDamage + damage.ArmourDamage - stats.Armour;
            if (shield != null)
                dmg += shield.Stats.Armour;

            if (dmg < damage.ArmourDamage)
                dmg = damage.ArmourDamage;

            TakeDamage(dmg, null);
        }
    }

    /// <summary>
    /// Нанести юниту урон без учета брони или каких либо вероятностей.
    /// </summary>
    /// <param name="damage">урон</param>
    /// <param name="owner">отряд, инициировавший удар</param>
    /// <param name="enemy">кто наносит урон</param>
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
                    0.2f,
                    ThisTransform.position
                ),
                SoundManager.SoundType.FX
            );

            Health -= damage;

            AfterTakingDamage(damage, owner);
        }
    }

    /// <summary>
    /// События после получения урона
    /// </summary>
    /// <param name="dmg">Урон, который нанес враг</param>
    /// <param name="owner">Вгар, который нанес урон</param>
    void AfterTakingDamage(float dmg, Squad owner)
    {
        if (squad != Squad.playerSquadInstance && owner == Squad.playerSquadInstance)
            GameManager.Instance.PlayerProgress.Score.expirience.Value += (int)dmg;

        bool friendlyfire = false;
        if (gameObject.layer != LayerMask.NameToLayer("ENEMY") && 
            gameObject.layer != LayerMask.NameToLayer("FALLEN_ENEMY") && 
                owner == Squad.playerSquadInstance)
            friendlyfire = true;

        ShowPopUpTakenDamageText(dmg.ToString(StringFormats.floatNumber), friendlyfire);
                
        TakeStan();
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
        if (squad != null)
        {
            if (targettedBy.Count == 0)
                squad.TargettedUnitsCount++;
            targettedBy.Add(unit);
        }
    }

    void RemoveTagrettedBy(Unit unit)
    {
        if (squad != null)
        {
            targettedBy.Remove(unit);
            if (targettedBy.Count == 0)
                squad.TargettedUnitsCount--;
        }
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


    void ShowPopUpTakenDamageText(string text, bool friendly = false)
    {
        if (GameManager.Instance.Settings.graphixSettings.ShowDamage)
        {
            Color color;

            if (friendly)
                color = GameManager.Instance.Settings.graphixSettings.FrieldlyDamageColor;
            else
                color = GameManager.Instance.Settings.graphixSettings.DamageToAllyColor;

            if (gameObject.layer == LayerMask.NameToLayer("ENEMY") || gameObject.layer == LayerMask.NameToLayer("FALLEN_ENEMY"))
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

    /// <summary>
    /// тут а = 1 по умолчанию!!!!!!!!!!!!!!111
    /// </summary>
    /// <param name="koef">коефициент от 0 до 1</param>
    public void TakeStan(float koef = 1)
    {
        if (!charging)
        {
            koef = Mathf.Clamp01(koef);

            Running = false;

            float duration = 1;
            if (fallen)
                duration = delayAfterFalling;
            else
                duration = delayAfterTakingDamage;

            //если воин не в фаланге то застанить
            if (squad == null || squad.CurrentFormation != FormationStats.Formations.PHALANX)
                StartCoroutine(Stan(duration * koef));
            else if (fallen) //если в фаланге, то станить только если ебили с ног
                StartCoroutine(Stan(duration * koef));
        }
    }

    /// <summary>
    /// После получения урона, юнит будет застанен.
    /// Перед тем как позволить ему снова двигаться пройдет указанная задержка.
    /// </summary>
    /// <returns></returns>
    IEnumerator Stan(float delay)
    {
        stanned = true;

        //тут сбрасываем всё что только можно
        SetTarget(null);
        
        yield return new WaitForSeconds(delay);
        stanned = false;

        if (fallen)
        {
            StandUp();
        }
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

            //если цель применяет натиск на фалангу, то наносим ей урон
            if(target.charging)
            {
                //проверяем чтобы target бежал в сторону фаланги и только тогда наносим урон
                //проверяем дется ли удар в спину врагу
                //тут не совсем точно просчитано, но работает и хер с ним
                //Quaternion enemyRot = Quaternion.LookRotation(Vector3.forward, -target.ThisTransform.position + ThisTransform.position);
                //float enemyRotation = enemyRot.eulerAngles.z - ThisTransform.rotation.eulerAngles.z;
                //enemyRotation = enemyRotation < 0 ? -enemyRotation : enemyRotation;
                //bool hitFromBack = !(enemyRotation <= stats.DefenceHalfSector || enemyRotation >= 360 - stats.DefenceHalfSector);

                //если враг спереди и смотрит на нас
                if (true)//!hitFromBack)
                {
                    //расчитываем урон
                    float dmg = (stats.Damage.BaseDamage + stats.Damage.ArmourDamage);
                    dmg = dmg < 0 ? -dmg : dmg;

                    dmg -= target.stats.Armour;
                    if (dmg < stats.Damage.ArmourDamage)
                        dmg = stats.Damage.ArmourDamage;

                    //сбрасываем цели натиск и наносим урон
                    target.Charging = false;
                    target.TakeDamage(dmg, squad, this);
                }
            }
        }        

        //юнит смотрит туда куда должен смотреть отряд
        LookTo(squad.EndLookRotation);

        //если не дожли ещё, то идём
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
                    LookTo(squad.EndLookRotation);

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

    void FallDown()
    {
        if(gameObject.layer == LayerMask.NameToLayer("ALLY"))
            gameObject.layer = LayerMask.NameToLayer("FALLEN_ALLY");
        else if(gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            gameObject.layer = LayerMask.NameToLayer("FALLEN_ENEMY");
        else if(gameObject.layer == LayerMask.NameToLayer("NEUTRAL"))
            gameObject.layer = LayerMask.NameToLayer("FALLEN_NEUTRAL");
        fallen = true;

        if (OnUnitFallDown != null)
            OnUnitFallDown();
    }

    void StandUp()
    {
        gameObject.layer = normLayer;
        fallen = false;
        if (OnUnitStandUp != null)
            OnUnitStandUp();
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
        float speedKoef = currentSpeed / stats.Speed;
        speedKoef = speedKoef > 1 ? 1 : speedKoef;

        //если врезались в стену
        if (collision.gameObject.layer == LayerMask.NameToLayer("MAP") || collision.gameObject.layer == LayerMask.NameToLayer("WALL"))
        {
            float dmg = speedKoef * weapon.Stats.Damag.BaseDamage * (1 + stats.ChargeAddDamage);
            FallDown();
            fallen = true;
            Charging = false;
            TakeDamage(dmg, squad);
        }
        else//если врезались в юнита
        {
            Unit enemy = collision.transform.GetComponent<Unit>();
            if(enemy != null)
            {
                //чтоб коллизия была спереди
                //тоесть чтобы враг, с которым мы столкнулись был спереди нас
                float rot = Quaternion.LookRotation(Vector3.forward, enemy.ThisTransform.position - ThisTransform.position).eulerAngles.z;
                float sub = Mathf.Abs(ThisTransform.rotation.eulerAngles.z - rot);
                if (sub >= chargePpushingAngle / 2 && sub <= 360 - chargePpushingAngle / 2)
                   return;
                                
                //если врезались в союзника
                if (enemy.gameObject.layer == gameObject.layer)
                {
                    if (enemy.squad != squad || !enemy.Charging)
                        TakeStan(speedKoef);
                }
                //если врезались во врага или нейтрала
                else
                {                
                    float dmg = (stats.Damage.BaseDamage + stats.Damage.ArmourDamage) * (1 + stats.ChargeAddDamage) * speedKoef;
                    dmg = dmg < 0 ? -dmg : dmg;

                    dmg -= enemy.stats.Armour;
                    if (dmg < stats.Damage.ArmourDamage)
                        dmg = stats.Damage.ArmourDamage;

                    //проверяем пришелся ли удар в спину врагу
                    //тут не совсем точно просчитано, но работает и хер с ним
                    Quaternion enemyRot = Quaternion.LookRotation(Vector3.forward, -enemy.ThisTransform.position + ThisTransform.position);
                    float enemyRotation = enemyRot.eulerAngles.z - ThisTransform.rotation.eulerAngles.z;
                    enemyRotation = enemyRotation < 0 ? -enemyRotation : enemyRotation;
                    bool hitFromBack = !(enemyRotation <= stats.DefenceHalfSector || enemyRotation >= 360 - stats.DefenceHalfSector);

                    float enemyDeflect = 0.5f;
                    bool weWonCharge = true;

                    Debug.Log(hitFromBack);

                    //если натиск против натиска, то считается у кого натиск сильнее
                    if (enemy.charging)
                    {
                        //если против натиска, то вместо суммарного сопротивления врага используем базовое значение.
                        //короче, надо лучше думать, перед тем как контрнатиск применять!!!
                        if (enemy.stats.ChargeImpact < stats.ChargeImpact)
                        {
                            if (enemy.squad != null)
                                enemyDeflect = enemy.squad.UnitStats.ChargeDeflect;
                        }
                        //если мы проигрываем врагу по силе натиска, то ничего не делаем
                        else
                            weWonCharge = false;
                    }
                    //если применять натиск на стоячего юнита, то считаем его сопротивление натиску
                    else
                    {
                        enemyDeflect = enemy.stats.ChargeDeflect;

                        //если ударили в спину, то щит и иоружие не сщитаются 
                        if (hitFromBack)
                        {   //Если его щит поднят, то он уже и так не учтён и отнимать не надо!!!
                            if (enemy.shield != null && enemy.squad != null && enemy.squad.CurrentFormation != FormationStats.Formations.RISEDSHIELDS)
                                enemyDeflect -= enemy.shield.Stats.ChargeDeflect;
                            //оружие в любом случае отнимаем
                            if (enemy.weapon != null)
                                enemyDeflect -= enemy.weapon.Stats.ChargeDeflect;
                        }
                    }
                       

                    //если значения эффективности натиска больше или равно чем защита врага, то наносим урон
                    if (weWonCharge && stats.ChargeImpact - enemyDeflect >= 0)
                    {
                        //но наносим до тех пор, пока хватает текущей эффективности
                        currentChargeImpact -= enemyDeflect;
                        if (currentChargeImpact >= 0)
                        {
                            //тут надо правильно рассчитать силу импульса
                            Vector2 forseToEnemy = ThisTransform.up * (stats.ChargeImpact * currentSpeed) / Time.deltaTime / 2;

                            enemy.FallDown();
                            if(enemy.charging)
                                Charging = false;
                            enemy.GoTo(forseToEnemy);
                            enemy.TakeDamage(dmg, squad, this);
                        }
                        else
                        {
                            Charging = false;
                        }
                    }
                    //если не смогли пробить натиском врага, то натиск спадает
                    else
                    {
                        Charging = false;
                    }
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