using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainWalker : MonoBehaviour, ITerrainWalker
{
    Transform thisTransform;
    UnitPosition unitPos;
    float scale = 1;

    public float Scale
    {
        get
        {
            return scale;
        }

        set
        {
            scale = value;
            thisTransform.localScale = new Vector2(scale, scale);
            unitPos.Scale = scale;
        }
    }

    public Vector2 Position
    {
        get
        {
            return thisTransform.position;
        }
    }

    void Start()
    {
        thisTransform = transform;
        unitPos = thisTransform.GetComponent<Unit>().TargetMovePositionObject.GetComponent<UnitPosition>();
    }
}
