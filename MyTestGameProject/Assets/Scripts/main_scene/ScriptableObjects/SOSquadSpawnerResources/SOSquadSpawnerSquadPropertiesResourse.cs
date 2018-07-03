using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/SquadProperties",  fileName = "SO_SSR_SquadProperties")]
public class SOSquadSpawnerSquadPropertiesResourse : ScriptableObject
{
    [SerializeField] AnimationCurve squadLevelDependency;
    [SerializeField] Squad[] origin;
    public Squad SquadOrigin
    {
        get
        {
            Squad res = null;

            if (origin.Length > 0)
            {
                float t = GameManager.Instance.CurrentLevel.WholeLevelT;
                int index = Mathf.RoundToInt(squadLevelDependency.Evaluate(t) * (origin.Length - 1));
                res = origin[index];
            }

            return res;
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
