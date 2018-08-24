using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class SetAllTileCollidersToUseByComposite : MonoBehaviour
{
    [ContextMenu("Execute")]
    private void Awake()
    {
        Execute();
        DestroyImmediate(this, true);
    }

    public void Execute()
    {
        var cols = Tools.Others.GetAllComponentsWithAllChildrens<TilemapCollider2D>(transform);
        foreach (var col in cols)
            col.usedByComposite = true;
    }

}
