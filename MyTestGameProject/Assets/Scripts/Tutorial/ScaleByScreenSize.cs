using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleByScreenSize : MonoBehaviour
{
    readonly Resolution defaultRes = new Resolution() { width = 1280, height = 720 };

    private void Start()
    {
        SetScale();
    }

#if UNITY_EDITOR
    void Update()
    {
        SetScale();
    }
#endif

    [ContextMenu("SetScale")]
    private void SetScale()
    {
        float scaleH = (float)Screen.height / defaultRes.height;
        float scaleW = (float)Screen.width / defaultRes.width;
        float scale = Mathf.Min(scaleH, scaleW);
        this.transform.localScale = new Vector3(scale, scale, scale);
    }
}
