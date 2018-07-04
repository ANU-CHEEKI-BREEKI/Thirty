using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAwakeWithChanse : MonoBehaviour
{
    [SerializeField] bool dependentOfLevelNumber = false;
    [SerializeField] AnimationCurve chanseToStayAliveLevelDependency;
    [Space]
    [SerializeField] [Range(0, 1)] float chanseToStayAlive = 1;
    [Space]
    [Tooltip("Если этот объект не удалится, то он удалит все объекты из нданного массива")]
    [SerializeField] GameObject[] otherGOToDestroy;

    void Awake()
    {
        float chanse = 0;
        if (dependentOfLevelNumber)
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            chanse = chanseToStayAliveLevelDependency.Evaluate(t);
        }
        else
        {
            chanse = chanseToStayAlive;
        }

        if (Random.value > chanse)
            DestroyImmediate(gameObject);
    }

    void Start()
    {
        if(otherGOToDestroy != null)
            foreach (var g in otherGOToDestroy)
                DestroyImmediate(g);
        Destroy(this);
    }
}
