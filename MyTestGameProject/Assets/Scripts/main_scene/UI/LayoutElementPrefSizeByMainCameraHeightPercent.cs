using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
[ExecuteInEditMode]
public class LayoutElementPrefSizeByMainCameraHeightPercent : MonoBehaviour
{
    [SerializeField] float prefWidthPercent;
    [SerializeField] float prefHeightPercent;
    Camera mainCamera;
    LayoutElement el;

    private void Awake()
    {
        Refresh();
    }

    public void Refresh()
    {
        el = GetComponent<LayoutElement>();
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
        el.preferredHeight = camera.pixelHeight * prefHeightPercent;
        el.preferredWidth = camera.pixelWidth * prefWidthPercent;
    }
}
