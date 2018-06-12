using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundBlock : MonoBehaviour
{
    public MapBlock block;
    [HideInInspector] public Vector2 posInMinigrid;

    [ContextMenu("RecalculateMabBlockMatrix")]
    public void RecalculateMabBlockMatrix()
    {
        string path = @"Assets\Resources\" + Ground.PATH_TO_GRIDS + name + ".xml";

        block.Grid = new bool[MapBlock.BLOCK_SIZE][];
        for (int row = 0; row < MapBlock.BLOCK_SIZE; row++)
            block.Grid[row] = new bool[MapBlock.BLOCK_SIZE];

        var t = transform;
        int ccnt = t.childCount;

        //сначала находим объект с тайлмапом
        Transform chld = null;
        for (int i = 0; i < ccnt; i++)
        {
            chld = t.GetChild(i);
            var grid = chld.GetComponent<Grid>();
            if (grid != null)
                break;
            else
                chld = null;
        }
        
        if (chld != null)
        {
            //если объект с тайлмапом был найден, то ищем объект lower layer (в котором отрисованы все объекты на ЗЕМЛЕ)
            FindTilesInLayer(chld, "lower layer");
            //если объект с тайлмапом был найден, то ищем объект lower layer (в котором отрисованы все объекты НАД землей)
            FindTilesInLayer(chld, "uper layer");
        }

        Extensions.Serialize(path, block);

        Debug.Log("GroundBlock -- RecalculateMabBlockMatrix -- Done! -- " + name);
    }

    void FindTilesInLayer(Transform chld, string name)
    {
        var ll = chld.Find(name);
        if (ll != null)
        {
            //если нашли, то проходим по всем оъектам и ищем у какого из них есть коллайдер
            int ccnt = ll.childCount;
            for (int i = 0; i < ccnt; i++)
            {
                var layer = ll.GetChild(i);
                var collider = layer.GetComponent<TilemapCollider2D>();
                //если нашли нужный, то проходим по всем ясейкам и записываем в массив. а потом и в соответствующий файл
                if (collider != null)
                {
                    var tilemap = layer.GetComponent<Tilemap>();
                    CalcMatrix(ref block, tilemap);
                }
            }
        }
    }

    void CalcMatrix(ref MapBlock block, Tilemap tilemap)
    {
        int r = MapBlock.BLOCK_SIZE;
        int c = r;

        for (int row = 0; row < r; row++)
        {
            for (int col = 0; col < c; col++)
            {
                var tile = tilemap.GetTile(new Vector3Int(col - MapBlock.BLOCK_SIZE / 2, row - MapBlock.BLOCK_SIZE / 2, 0));
                if (tile != null)
                    block.Grid[row][col] = true;
            }
        }
    }

    MapBlock blc;
    private void OnDrawGizmos()
    {
        blc = block;
        if (blc.Grid == null)
        {
            string path = @"Assets\Resources\" + Ground.PATH_TO_GRIDS + name + ".xml";
            blc = Extensions.Deserialize(path);
        }

        if (blc.Grid != null)
        {
            int rc = blc.Grid.Length;
            int cc = blc.Grid[0].Length;
            for (int row = 0; row < rc; row++)
                for (int col = 0; col < cc; col++)
                    if (blc.Grid[row][col])
                        Gizmos.DrawWireCube(
                            new Vector3(
                                transform.position.x + col * MapBlock.BLOCK_SCALE - MapBlock.WORLD_BLOCK_SIZE / 2f + MapBlock.BLOCK_SCALE / 2f,
                                transform.position.y + row * MapBlock.BLOCK_SCALE - MapBlock.WORLD_BLOCK_SIZE / 2f + MapBlock.BLOCK_SCALE / 2f
                            ),
                            new Vector3(MapBlock.BLOCK_SCALE, MapBlock.BLOCK_SCALE) / 1.1f
                        );
        }
    }


}
