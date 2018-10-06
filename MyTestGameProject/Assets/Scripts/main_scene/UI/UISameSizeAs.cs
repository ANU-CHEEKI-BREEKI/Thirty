using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UISameSizeAs : MonoBehaviour
{
    [SerializeField] RectTransform sameSizeAs;

    void Start()
    {
        SetSize();
    }

#if UNITY_EDITOR
    void Update()
    {
        SetSize();
    }
#endif

    void SetSize()
    {
        var rt = transform as RectTransform;

        Vector2 myPrevPivot = rt.pivot;
        Vector2 mypos = rt.position;

        rt.pivot = sameSizeAs.pivot;
        rt.position = sameSizeAs.position;
        rt.localScale = sameSizeAs.localScale;

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sameSizeAs.rect.width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sameSizeAs.rect.height);
        //rectTransf.ForceUpdateRectTransforms(); - needed before we adjust pivot a second time?

        rt.pivot = myPrevPivot;
        rt.position = mypos;
    }
}
