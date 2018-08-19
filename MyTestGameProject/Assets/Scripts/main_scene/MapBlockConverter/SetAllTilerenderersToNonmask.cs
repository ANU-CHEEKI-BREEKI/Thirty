using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class SetAllTilerenderersToNonmask : MonoBehaviour
{
    private void Awake()
    {
        Execute();
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        SetAll(Tools.Others.GetAllComponentsWithAllChildrens<TilemapRenderer>(Tools.Others.FindChildWithNameContains(transform, "GroundTiles")));

        DestroyImmediate(this, true);
    }

    void SetAll(TilemapRenderer[] maps)
    {
        foreach (var map in maps)
        {
            map.maskInteraction = SpriteMaskInteraction.None;
            Debug.Log("SpriteMaskInteraction.None in " + map.gameObject.name + " completed");
        }
    }
}
