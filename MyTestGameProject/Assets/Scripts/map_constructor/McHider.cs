using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class McHider : MonoBehaviour
{
    public const int DELTA_Y = 400;

    RectTransform rectTransform;

    void Start ()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        Hide();
    }

    public void Hide()
    {
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, DELTA_Y, rectTransform.localPosition.z);
    }

    public void Show()
    {
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, 0, rectTransform.localPosition.z);
    }
}
