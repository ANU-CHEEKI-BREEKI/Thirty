using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class DeleteTilesOutOfBounds : MonoBehaviour
{
    void Awake()
    {
        Execute();
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        ConvertAll(Tools.Others.GetAllComponentsWithAllChildrens<Tilemap>(Tools.Others.FindChildWithNameContains(transform, "GroundTiles")));

        DestroyImmediate(this, true);
    }

    void ConvertAll(Tilemap[] maps)
    {
        foreach (var map in maps)
            ConverteToBseTile(map);
    }

    void ConverteToBseTile(Tilemap map)
    {
        var size = map.size;
        for (int row = -size.y / 2; row < size.y; row++)
        {
            for (int col = -size.x / 2; col < size.x; col++)
            {
                if (row < -10 || row > 9 || col < -10 ||  col > 9)
                {
                    var pos = new Vector3Int(col, row, 0);
                    map.SetTile(pos, null);
                }
            }
        }
        map.RefreshAllTiles();

        Debug.Log("Deleting out of bounds tiles in " + map.gameObject.name + " completed");
    }
}
