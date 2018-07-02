using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/SquadProperties",  fileName = "SO_SSR_SquadProperties")]
public class SOSquadSpawnerSquadPropertiesResourse : ScriptableObject
{
    [SerializeField] AnimationCurve squadLevelDependency;
    [SerializeField] Squad[] origin;
    public Squad SquadOrigin { get { throw new System.NotImplementedException(); } }
    [Space]
    [SerializeField] bool useDefauldUnitCount;
    public bool UseDefaultUnitCount { get { return useDefauldUnitCount; } }
    [SerializeField] AnimationCurve countLevelDependency;
    [SerializeField] int unitMinCount = 10;
    [SerializeField] int unitMaxCount = 30;
    [SerializeField] int unitCountDispersion = 10;
    public int UnitCount { get { throw new System.NotImplementedException(); } }
}
