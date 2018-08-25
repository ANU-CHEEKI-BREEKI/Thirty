using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAwakeAllAndStayOne : MonoBehaviour
{
    [SerializeField] bool chanseDependentOfLevelNumber = false;
    [SerializeField] AnimationCurve chanseToDestroyAllDependentOfLevelNumber;
    [Space]
    [SerializeField] [Range(0, 1)] float chanseToDestroyAll = 0;
    [Space]
    [SerializeField] GameObject[] objs;

    private void Awake()
    {
        float chanse = 0;
        if (chanseDependentOfLevelNumber)
        {
            float t = GameManager.Instance.CurrentLevel.WholeLevelT;
            chanse = chanseToDestroyAllDependentOfLevelNumber.Evaluate(t);
        }
        else
        {
            chanse = chanseToDestroyAll;
        }

        if (Random.value <= chanse)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            int cnt = objs.Length;
            if (cnt > 0)
            {
                var objToAlive = objs[Random.Range(0, cnt)];
                foreach (var obj in objs)
                    if (obj != objToAlive)
                        DestroyImmediate(obj);
            }
        }
    }
}
