using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapCamera : MonoBehaviour
{
    [SerializeField] RenderTexture map;
    [SerializeField] LayerMask mapLayer;
    [Space]
    [SerializeField]
    RenderTexture minimap;
    [SerializeField] LayerMask minimapLayer;
        
    static public Camera Instance { get; private set; }

    private void Awake()
    {
        Instance = GetComponent<Camera>();
    }

    void Start()
    {
        Instance.enabled = false;

        Ground.Instance.OnGenerationDone += OnGroundGenerated;
        Ground.Instance.OnRecalcByCurrentBlocks += OnGroundGenerated;
    }

    void OnGroundGenerated()
    {
        StartCoroutine(GeneradeMapMiniature());
    }

    IEnumerator GeneradeMapMiniature()
    {
        var gr = Ground.Instance;

        Instance.orthographicSize =  gr.RowCountOfBlocks > gr.ColCountOfBlocks ? gr.RowCountOfBlocks : gr.ColCountOfBlocks;
        Instance.orthographicSize *= MapBlock.WORLD_BLOCK_SIZE / 2;
        Instance.transform.position = new Vector3(
            gr.ColCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE / 2,
            gr.RowCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE / 2,
            Instance.transform.position.z
        );

        Instance.cullingMask = mapLayer.value;
        Instance.targetTexture = map;
        Instance.enabled = true;

        yield return new WaitForEndOfFrame();

        Instance.cullingMask = minimapLayer.value;
        Instance.targetTexture = minimap;
    }
}
