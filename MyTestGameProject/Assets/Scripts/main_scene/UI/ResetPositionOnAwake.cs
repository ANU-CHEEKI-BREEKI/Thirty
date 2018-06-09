using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionOnAwake : MonoBehaviour {

    private void Awake()
    {
        ((RectTransform)transform).anchoredPosition = Vector2.zero;
        Destroy(this);
    }
}
