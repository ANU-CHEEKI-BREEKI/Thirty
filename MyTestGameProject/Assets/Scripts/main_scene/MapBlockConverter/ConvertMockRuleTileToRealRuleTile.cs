using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class ConvertMockRuleTileToRealRuleTile : MonoBehaviour
{
    void Awake()
    {
        ConvertAll(Tools.Others.GetAllComponentsWithAllChildrens<Tilemap>(Tools.Others.FindChildWithNameContains(transform, "GroundTiles")));

        DestroyImmediate(this);
    }

    Dictionary<MockRuleTile, RuleTile> old_new_Tiles = new Dictionary<MockRuleTile, RuleTile>();
    
    void ConvertAll(Tilemap[] maps)
    {
        old_new_Tiles.Clear();

        foreach (var map in maps)
        {
            ConverteToBseTile(map);
        }
    }

    void ConverteToBseTile(Tilemap map)
    {
        var size = map.size;
        for (int row = -size.y / 2; row < size.y; row++)
        {
            for (int col = -size.x / 2; col < size.x; col++)
            {
                var pos = new Vector3Int(col, row, 0);
                MockRuleTile oldTile = map.GetTile(pos) as MockRuleTile;
                if (oldTile != null)
                {
                    if (!old_new_Tiles.ContainsKey(oldTile))
                    {
                        var newTile = oldTile.RealTile;
                        map.SetTile(pos, newTile);
                        old_new_Tiles.Add(oldTile, newTile);
                    }
                    else
                    {
                        map.SetTile(pos, old_new_Tiles[oldTile]);
                    }
                }
            }
        }
        map.RefreshAllTiles();

        Debug.Log("Convrting tilemap to real tiles in " + map.gameObject.name + " completed");
    }
}
