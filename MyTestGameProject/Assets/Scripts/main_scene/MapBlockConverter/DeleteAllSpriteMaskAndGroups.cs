using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class DeleteAllSpriteMaskAndGroups : MonoBehaviour
{
    void Awake()
    {
        Execute();
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        foreach (var item in Tools.Others.GetAllComponentsWithAllChildrens<SortingGroup>(Tools.Others.FindChildWithNameContains(transform, "GroundTiles")))
            DestroyImmediate(item, true);
        foreach (var item in Tools.Others.GetAllComponentsWithAllChildrens<SpriteMask>(Tools.Others.FindChildWithNameContains(transform, "GroundTiles")))
            DestroyImmediate(item, true);

        DestroyImmediate(this, true);
    }
}
