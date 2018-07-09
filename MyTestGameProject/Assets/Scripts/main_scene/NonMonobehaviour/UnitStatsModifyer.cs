using System;
using System.Collections.Generic;
using UnityEngine;
using static Description;

[Serializable]
public struct UnitStatsModifier: IDescriptionable
{
    public enum UseType { APPLY, REJECT}

    [SerializeField] UIIfo uiInfo;
    public UIIfo UiInfo { get { return uiInfo; } }

    [Space]

	[SerializeField] Modifyer health;
    public Modifyer Health { get { return health; } set { health = value; } }
    
    [SerializeField] Modifyer armour;
    public Modifyer Armour { get { return armour; } set { armour = value; } }

    [SerializeField] Modifyer baseDamage;
    public Modifyer BaseDamage { get { return baseDamage; } set { baseDamage = value; } }

    [SerializeField] Modifyer armourDamage;
    public Modifyer ArmourDamage { get { return armourDamage; } set { armourDamage = value; } }

    [Space]
    [SerializeField] Modifyer attack;
    public Modifyer Attack { get { return attack; } set { attack = value; } }

    [SerializeField] Modifyer defence;
    public Modifyer Defence { get { return defence; } set { defence = value; } }

    [Space]
    [SerializeField] Modifyer defenceHalfSector;
    public Modifyer DefenceHalfSector { get { return defenceHalfSector; } set { defenceHalfSector = value; } }

    [SerializeField] Modifyer defenceGoingThrough;
    public Modifyer DefenceGoingThrought { get { return defenceGoingThrough; } set { defenceGoingThrough = value; } }

    [SerializeField] Modifyer missileBlock;
    public Modifyer MissileBlock { get { return missileBlock; } set { missileBlock = value; } }
    
    [Space]
    [SerializeField] Modifyer speed;
    public Modifyer Speed { get { return speed; } set { speed = value; } }

    [SerializeField] Modifyer acceleration;
    public Modifyer Acceleration { get { return acceleration; } set { acceleration = value; } }

    [SerializeField] Modifyer rotationSpeed;
    public Modifyer RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }

    [Space]
    [SerializeField] Modifyer chargeImpact;
    public Modifyer ChargeImpact { get { return chargeImpact; } set { chargeImpact = value; } }

    [SerializeField] Modifyer chargeDeflect;
    public Modifyer ChargeDeflect { get { return chargeDeflect; } set { chargeDeflect = value; } }

    [SerializeField] Modifyer chargeDamage;
    public Modifyer ChargeAddDamage { get { return chargeDamage; } set { chargeDamage = value; } }

    [Serializable]
    public struct Modifyer
    {
        /// <summary>
        /// Указывает что хранится в значении
        /// </summary>
        public enum ValueType { PERCENT, UNIT }

        /// <summary>
        /// Указывает что хранится в значении
        /// </summary>
        [Tooltip("Указывает что хранится в значении")]
        [SerializeField] ValueType type;
        [SerializeField] float value;

        public ValueType VType { get { return type; } }
        public float Value { get { return value; } }

        public Modifyer(ValueType type, float value)
        {
            this.type = type;
            this.value = value;
        }
    }

    [Serializable] 
    public struct UIIfo
    {
        [SerializeField] Sprite icon;
        public Sprite Icon { get { return icon; } }
        [SerializeField] string resourceName;
        public string ResourceName  { get { return resourceName; } }
        [SerializeField] string resourceDesc;
        public string ResourceDesc { get { return resourceDesc; } }
    }

    public Description GetDescription()
    {
        Description d = new Description();

        List<DescriptionItem> stats = new List<DescriptionItem>();

        var fieldsInfo = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var fi in fieldsInfo)
        {
            var f = fi.GetValue(this);
            if (f == null)
                continue;

            var ft = f.GetType();
            if (ft.Name != typeof(Modifyer).Name)
                continue;

            var vi = ft.GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            float v = Convert.ToSingle(vi.GetValue(f));

            if (v != 0)
            {
                var ti = ft.GetField("type", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Modifyer.ValueType t = (Modifyer.ValueType) ti.GetValue(f);

                if(t == Modifyer.ValueType.PERCENT)
                {
                    stats.Add(new DescriptionItem() { Name = Localization.GetString(fi.Name), Description = v.ToString(StringFormats.floatSignNumberPercent), ItPositiveDesc = v > 0 });
                }
                else
                {
                    stats.Add(new DescriptionItem() { Name = Localization.GetString(fi.Name), Description = v.ToString(StringFormats.floatSignNumber), ItPositiveDesc = v > 0 });
                }
            }
        }

        d.Stats = stats.ToArray();

        return d;
    }
}
