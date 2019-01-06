using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
public class SetCanvasRenderMainCamera : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

#if UNITY_EDITOR
    void Update()
    {
        var c = GetComponent<Canvas>();
        if (c.worldCamera == null)
            c.worldCamera = Camera.main;
    }
#endif
}
