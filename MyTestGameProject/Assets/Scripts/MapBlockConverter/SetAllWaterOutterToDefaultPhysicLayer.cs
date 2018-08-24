using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetAllWaterOutterToDefaultPhysicLayer : MonoBehaviour
{
    [ContextMenu("Execute")]
    private void Awake()
    {
        Execute();
        DestroyImmediate(this, true);
    }

    void Execute()
    {
        Tools.Others.FindChildWithNameContains(
            Tools.Others.FindChildWithNameContains(
                Tools.Others.FindChildWithNameContains(
                    transform, 
                    "GroundTiles"
                ),
                "lower layer"
            ),
            "WaterOutter"
        )
        .gameObject.layer = 0;
    }
}
