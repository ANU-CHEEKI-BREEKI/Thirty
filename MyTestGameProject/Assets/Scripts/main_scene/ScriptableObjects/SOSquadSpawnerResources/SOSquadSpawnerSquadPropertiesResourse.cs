using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/SquadProperties", fileName = "SO_SSR_SquadProperties")]
public class SOSquadSpawnerSquadPropertiesResourse : ScriptableObject
{
    [SerializeField] AnimationCurve squadStatsLevelDependency;
    [SerializeField] Squad originSquad;
    public Squad SquadOrigin { get { return originSquad; } }
    [SerializeField] UnitStats minUnitStats;
    [SerializeField] UnitStats maxUnitStats;
    public UnitStats UnitStats
    {
        get
        {
            float level = GameManager.Instance.CurrentLevel.WholeLevelT;
            float t = squadStatsLevelDependency.Evaluate(level);
            return new UnitStats
            (
                Mathf.Lerp(minUnitStats.Health, maxUnitStats.Health, t),
                Mathf.Lerp(minUnitStats.Attack, maxUnitStats.Attack, t),
                Mathf.Lerp(minUnitStats.Defence, maxUnitStats.Defence, t),
                Mathf.Lerp(minUnitStats.DefenceHalfSector, maxUnitStats.DefenceHalfSector, t),
                Mathf.Lerp(minUnitStats.Speed, maxUnitStats.Speed, t),
                Mathf.Lerp(minUnitStats.Acceleration, maxUnitStats.Acceleration, t),
                Mathf.Lerp(minUnitStats.RotationSpeed, maxUnitStats.RotationSpeed, t),
                Mathf.Lerp(minUnitStats.ChargeImpact, maxUnitStats.ChargeImpact, t),
                Mathf.Lerp(minUnitStats.ChargeDeflect, maxUnitStats.ChargeDeflect, t),
                Mathf.Lerp(minUnitStats.ChargeAddDamage, maxUnitStats.ChargeAddDamage, t)
            );
        }
    }
    [Space]
    [SerializeField] Squad.UnitFraction fraction = Squad.UnitFraction.ENEMY;
    public Squad.UnitFraction Fraction { get { return fraction; } }
    [Space]
    [SerializeField] bool useDefauldUnitCount;
    public bool UseDefaultUnitCount { get { return useDefauldUnitCount; } }
    [SerializeField] AnimationCurve countLevelDependency;
    [SerializeField] int unitMinCount = 10;
    [SerializeField] int unitMaxCount = 30;
    [SerializeField] int unitCountDispersion = 10;
    public int UnitCount
    {
        get
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            float val = countLevelDependency.Evaluate(t) * (unitMaxCount - unitMinCount) + unitMinCount;
            int cnt = Mathf.RoundToInt(val) + Random.Range(-unitCountDispersion / 2, unitCountDispersion / 2 + 1);
            if (cnt < unitMinCount) cnt = unitMinCount;
            if (cnt > unitMaxCount) cnt = unitMaxCount;
            return cnt;
        }
    }
}
