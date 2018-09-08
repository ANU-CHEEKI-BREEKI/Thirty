using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR 
public class EditorHelperTools: Editor
{
    [MenuItem("EditorHelperTools/InsnantiateAllGrasslandBlocks")]
    static void InsnantiateAllGrasslandBlocks()
    {
        var blocks = GetBlocksAssets(Ground.GroundType.GRASSLAND);
        Transform parent = null;
        if (blocks.Length > 0)
        {
            parent = new GameObject("All Grassland Blocks").transform;
            InstantiateAll(parent, blocks);
        }
    }

    [MenuItem("EditorHelperTools/InsnantiateAllSwampBlocks")]
    static void InsnantiateAllSwampBlocks()
    {
        var blocks = GetBlocksAssets(Ground.GroundType.SWAMP);
        Transform parent = null;
        if (blocks.Length > 0)
        {
            parent = new GameObject("All Swamp Blocks").transform;
            InstantiateAll(parent, blocks);
        }        
    }

    static GameObject[] GetBlocksAssets(Ground.GroundType type)
    {
        return Resources.LoadAll<GameObject>(Ground.PATH_TO_PREFABS + Ground.PATH_TO_BLOCK_OF_TYPE(type));
    }

    static void InstantiateAll(Transform parent, GameObject[] blocks)
    {
        int rowLength = Mathf.RoundToInt(Mathf.Sqrt(blocks.Length));
        int x = 0, y = 0;
        for (int i = 0; i < blocks.Length; i++)
        {
            var go = PrefabUtility.InstantiatePrefab(blocks[i]) as GameObject;
            go.name = blocks[i].name;
            go.transform.parent = parent;
            go.transform.position = new Vector3
            (
                x * MapBlock.WORLD_BLOCK_SIZE + x * 10,
                y * MapBlock.WORLD_BLOCK_SIZE + y * 10,
                0
            );

            if (x < rowLength)
            {
                x++;
            }
            else
            {
                x = 0;
                y--;
            }
        }
    }
}
#endif