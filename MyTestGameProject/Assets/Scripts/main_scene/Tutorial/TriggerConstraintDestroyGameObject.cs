using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintDestroyGameObject : ATriggerConstraint
{
    [SerializeField] GameObject monitoringObject;

    public override bool IsTrue
    {
        get
        {
            return monitoringObject == null;
        }
    }
}
