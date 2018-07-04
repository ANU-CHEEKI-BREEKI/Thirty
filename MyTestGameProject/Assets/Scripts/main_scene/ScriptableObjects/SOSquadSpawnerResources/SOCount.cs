using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOCount")]
public class SOCount : ScriptableObject
{
    [SerializeField] AnimationCurve countLevelDependency;
    [SerializeField] int count;
    [SerializeField] int minCount;
    [SerializeField] int maxCount;

    public int CountByLevel { get { return Mathf.RoundToInt(countLevelDependency.Evaluate(GameManager.Instance.CurrentLevel.WholeLevelT) * (maxCount - minCount) + minCount); } }
    public int RandomCount { get { return Random.Range(minCount, maxCount + 1); } }
    public int Count { get { return count; } }
    public int MinCount { get { return minCount; } }
    public int MaxCount { get { return maxCount; } }

}
