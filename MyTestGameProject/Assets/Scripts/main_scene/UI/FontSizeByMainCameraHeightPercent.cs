using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))][ExecuteInEditMode]
public class FontSizeByMainCameraHeightPercent : MonoBehaviour
{
    [SerializeField] float persent;
    TextMeshProUGUI text;
    Camera mainCamera;

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
        text.fontSize = camera.pixelHeight * persent;
    }
}
