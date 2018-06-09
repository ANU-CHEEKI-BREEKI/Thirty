using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterUnitSelection : MonoBehaviour
{
    public GameObject TargetMovePositionObject { private get; set; }

    private void FixedUpdate()
    {
        transform.position = TargetMovePositionObject.transform.position 
            + transform.parent.position - TargetMovePositionObject.transform.parent.position;
    }
}
