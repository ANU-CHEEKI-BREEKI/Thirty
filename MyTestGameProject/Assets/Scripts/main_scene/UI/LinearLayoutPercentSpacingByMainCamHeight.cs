using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
[ExecuteInEditMode]
public class LinearLayoutPercentSpacingByMainCamHeight : MonoBehaviour
{
    [SerializeField] [Range(0, 0.5f)] float spacingPercentByMainCamHeight = 0.01f;
    [SerializeField] RectOffset paddindPercentByMainCamHeight;
    Camera mainCam;
    HorizontalOrVerticalLayoutGroup layout;

    private void OnEnable()
    {
        mainCam = Camera.main;
        layout = GetComponent<HorizontalOrVerticalLayoutGroup>();
    }

    void Start ()
    {
        SetSpacing();
    }

#if UNITY_EDITOR
    void Update ()
    {
        SetSpacing();
    }
#endif

    void SetSpacing()
    {
        layout.spacing = spacingPercentByMainCamHeight * mainCam.pixelHeight;
        layout.padding.left =  paddindPercentByMainCamHeight.left * mainCam.pixelHeight / 100;
        layout.padding.right = paddindPercentByMainCamHeight.right * mainCam.pixelHeight / 100;
        layout.padding.top = paddindPercentByMainCamHeight.top * mainCam.pixelHeight / 100;
        layout.padding.bottom = paddindPercentByMainCamHeight.bottom * mainCam.pixelHeight / 100;
    }
}
