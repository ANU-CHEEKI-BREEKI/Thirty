using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class ConvertTileResourceManager : MonoBehaviour
{
    public List<Tile> resourseToConvert;

    static public ConvertTileResourceManager Instance { get; private set; }

    [ContextMenu("Refresh")]
    private void Awake()
    {
        Instance = this;
    }
}
