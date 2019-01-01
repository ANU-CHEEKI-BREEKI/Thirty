using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))][ExecuteInEditMode]
public class FontSizeByMainCameraHeightPercent : MonoBehaviour
{
    [SerializeField] float persent;
    [Space]
    [SerializeField] float maxPhysicSizeMM = 10;
    TextMeshProUGUI text;
    Camera mainCamera;

    const float inchToMM = 25.4f;

    private void Awake()
    {
        Refresh();
    }

    public void Refresh()
    {
        text = GetComponent<TextMeshProUGUI>();
        mainCamera = Camera.main;
        SetSize(mainCamera);
    }

#if UNITY_EDITOR

    void Update()
    {
        SetSize(Camera.main);
    }

#endif
        
    void SetSize(Camera camera)
    {
        float size = camera.pixelHeight * persent;

#if UNITY_EDITOR        
#else
        var maxSize = maxPhysicSizeMM / inchToMM * Screen.dpi;
        if (maxPhysicSizeMM > 0 && size > maxSize)
            size = maxSize;
#endif

        text.fontSize = size;
    }
}
