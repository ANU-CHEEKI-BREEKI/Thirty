using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class McCameraController : 
    MonoBehaviour, 
    IScrollHandler, 
    IDragHandler, 
    IPointerDownHandler,
    IPointerUpHandler
{
    McCursor cursor;

    public bool CanChangeCamera { get; set; }

    [SerializeField] float cameraScaleStep = 10f;

    new Camera camera;
    Transform cameraTransform;
    Vector3 startCameraPosition;
    Vector3 startMousePosition;
    Vector3 mouseDelta;

    public void OnDrag(PointerEventData eventData)
    {
        if (CanChangeCamera)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                mouseDelta = Camera.main.ScreenToWorldPoint(eventData.position) - startMousePosition;

                Vector3 cameraDrag = new Vector3(mouseDelta.x, mouseDelta.y, 0);
                cameraTransform.position -= cameraDrag;
            }
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanChangeCamera)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                startMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);

                cursor.SetHandCursor();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cursor.SetBrushCursor();
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (CanChangeCamera)
        {
            float size = camera.orthographicSize - cameraScaleStep * Math.Sign(eventData.scrollDelta.y);
            if (size > 0)
                camera.orthographicSize = size;
        }
    }

    // Use this for initialization
    void Start ()
    {
        camera = Camera.main;
        cameraTransform = camera.transform;

        CanChangeCamera = true;

        cursor = gameObject.GetComponent<McCursor>();
    }

    public void CameraScaleStepSlider_ValueChanged(float newValue)
    {
        cameraScaleStep = newValue;
    }
}
