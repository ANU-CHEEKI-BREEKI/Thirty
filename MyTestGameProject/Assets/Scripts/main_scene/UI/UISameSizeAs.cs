using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UISameSizeAs : MonoBehaviour
{
    [SerializeField] RectTransform sameSizeAs;
    [SerializeField] bool forceSetPivot = false;
    [SerializeField] Vector2 pivot = Vector2.one;
    void Start()
    {
        Canvas.ForceUpdateCanvases();
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

        rt.pivot = myPrevPivot;
        var z = rt.position.z;
        var newPos = (Vector3) mypos;
        newPos.z = z;
        rt.position = newPos;

        if(forceSetPivot)
            rt.pivot = pivot;
    }
}
