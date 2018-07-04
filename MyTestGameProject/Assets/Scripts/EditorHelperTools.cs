using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR 
public class EditorHelperTools: Editor
{
    [MenuItem("EditorHelperTools/InsnantiateAllGroundBlocks")]
    static void InsnantiateAllGroundBlocks()
    {
        var allBlocks = Resources.LoadAll<GameObject>(Ground.PATH_TO_PREFABS);

        Transform parent = null;
        if (allBlocks.Length > 0)
            parent = new GameObject("All Ground Blocks").transform;

        int rowLength = Mathf.RoundToInt(Mathf.Sqrt(allBlocks.Length));
        int x = 0, y = 0;
        for (int i = 0; i < allBlocks.Length; i++)
        {
            var go = PrefabUtility.InstantiatePrefab(allBlocks[i]) as GameObject;
            go.name = allBlocks[i].name;
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