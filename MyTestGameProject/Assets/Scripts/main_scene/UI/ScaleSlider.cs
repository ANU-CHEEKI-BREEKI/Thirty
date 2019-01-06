using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ScaleSlider : MonoBehaviour
{
    void Start()
    {
        var sl = GetComponent<Slider>();
        var cam = Camera.main;
        var camController = cam.GetComponent<CameraController>();
        sl.onValueChanged.AddListener(camController.OnScrolbarValueChanged);
    }
}
