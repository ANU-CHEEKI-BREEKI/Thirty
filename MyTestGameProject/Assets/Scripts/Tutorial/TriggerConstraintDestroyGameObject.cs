using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintDestroyGameObject : ATriggerConstraint
{
    [SerializeField] GameObject monitoringObject;

    [SerializeField] bool all;
    [SerializeField] GameObject[] allOfThisMonitoring;

    public override bool IsTrue
    {
        get
        {
            bool res = true;
            if (!all)
            {
                res = monitoringObject == null;
            }
            else
            {
                int c = allOfThisMonitoring.Length;
                for (int i = 0; i < c; i++)
                {
                    if (allOfThisMonitoring[i] != null)
                    {
                        res = false;
                        break;
                    }
                }
            }
            return res;
        }
    }
}
