using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class ConvertRuleTileToSimpleTile : MonoBehaviour
{
    Dictionary<RuleTile, MockRuleTile> old_new_Tiles = new Dictionary<RuleTile, MockRuleTile>();

    const string exmes = "Нужно создать менеджер ресурсов и закинуть туда ВСЕ тайлы, которые будут использоваться для конвертации.\r\n" +
                         "А иначе, префабы не сохранятся у будет жопаблять!!!!!";

    private void Awake()
    {
        Execute();
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        if (ConvertTileResourceManager.Instance == null ||
            ConvertTileResourceManager.Instance.resourseToConvert == null ||
            ConvertTileResourceManager.Instance.resourseToConvert.Count == 0)
            throw new Exception(exmes);
        ConvertAll(Tools.Others.GetAllComponentsWithAllChildrens<Tilemap>(Tools.Others.FindChildWithNameContains(transform, "GroundTiles")));

        DestroyImmediate(this, true);
    }
    
    void ConvertAll(Tilemap[] maps)
    {
        old_new_Tiles.Clear();

        foreach (var map in maps)
        {
            //заменяем руле тайл на подставной рулетайл с калбеком
            ConverteToSimpleTile(map);
            Dictionary<Vector3Int, TileData> tiles = new Dictionary<Vector3Int, TileData>();
            List<MockRuleTile> subscribedTiles = new List<MockRuleTile>();
            //подписываемся на калбек по одному разу на один тайл (тайлы это скрипьабле обджект!) 
            SubscribeAllTiles(map, tiles, subscribedTiles);
            //обновляем мапу, соответственно вызываются калбеки и мы получаем нужную дату
            map.RefreshAllTiles();
            //отписываемся от калбеков, т.к. они будут вызываться при замене тайлов
            foreach (var item in subscribedTiles)
                item.ClearEvent();
            //таменяем подставные тайлы на новые самые обычные
            SetAllTiles(map, tiles);
            Debug.Log("Convrting tilemap to mock tiles in " + map.gameObject.name + " completed");
        }
    }

    void SetAllTiles(Tilemap map, Dictionary<Vector3Int, TileData> tiles)
    {
        foreach (var key in tiles.Keys)
        {
            var data = tiles[key];
            //находим нужный тайл
            Tile t = ConvertTileResourceManager.Instance.resourseToConvert.Find((tile) =>
            {
                return tile.sprite == data.sprite;
            });

            if (t != null)
            {
                //устанавливаем тайл
                map.SetTile(key, t);

                //устанавливаем повотор, приворот, подъём-перевотор. гы кек ололо
                map.SetTransformMatrix(key, data.transform);
                map.SetTileFlags(key, data.flags);
            }
            else
                throw new Exception(exmes);
        }
    }

    void SubscribeAllTiles(Tilemap map, Dictionary<Vector3Int, TileData> tiles, List<MockRuleTile> subscribedTiles)
    {
        var size = map.size;
        for (int row = -size.y / 2; row < size.y; row++)
        {
            for (int col = -size.x / 2; col < size.x; col++)
            {
                var pos = new Vector3Int(col, row, 0);
                MockRuleTile tile = map.GetTile(pos) as MockRuleTile;
                if (tile != null)
                {
                    if (!subscribedTiles.Contains(tile))
                    {
                        Action<Vector3Int, TileData> act = null;
                        act = (position, tiledata) =>
                        {
                            if (!tiles.ContainsKey(position))
                                tiles.Add(position, tiledata);
                            else
                                tiles[position] = tiledata;
                        };
                        tile.OnGetTileData += act;

                        subscribedTiles.Add(tile);
                    }
                    
                }
            }
        }
    }

    void ConverteToSimpleTile(Tilemap map)
    {
        var size = map.size;
        for (int row = -size.y / 2; row < size.y; row++)
        {
            for (int col = -size.x / 2; col < size.x; col++)
            {
                var pos = new Vector3Int(col, row, 0);
                RuleTile oldTile = map.GetTile(pos) as RuleTile;
                if(oldTile != null)
                {
                    MockRuleTile newTile;
                    if (!old_new_Tiles.ContainsKey(oldTile))
                    {
                        newTile = new MockRuleTile(oldTile);
                        old_new_Tiles.Add(oldTile, newTile);                        
                    }
                    else
                    {
                        newTile = old_new_Tiles[oldTile];                        
                    }
                    map.SetTile(pos, newTile);
                }
            }
        }
    }
}
