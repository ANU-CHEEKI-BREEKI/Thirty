using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MainCanvas : MonoBehaviour
{
    static MainCanvas instance;
    public static MainCanvas Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("Canvas").AddComponent<MainCanvas>();
                instance.Clear();
            }
            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    Canvas canvas;
    public Canvas Canvas
    {
        get
        {
            if (canvas == null)
                canvas = GetComponent<Canvas>();
            return canvas;
        }

        private set
        {
            canvas = value;
        }
    }

    Camera camera;
    Camera Camera
    {
        get
        {
            if (camera == null) camera = Camera.main;
            return camera;
        }
        set
        {
            camera = value;
        }
    }

    private void Awake()
    {
        Instance = this;
        Canvas = GetComponent<Canvas>();

        Camera = Camera.main;
    }

    private void Start()
    {
        var arr = gameObject.GetComponents<MainCanvas>();
        foreach (var item in arr)
            if (item != instance)
                Destroy(item);
    }

    [ContextMenu("Clear")]
    void Clear()
    {
        var arr = gameObject.GetComponents<MainCanvas>();
        foreach (var item in arr)
            if (item != instance)
                DestroyImmediate(item);
    }

    /// <summary>
    /// Переводит кооддинаты экрана в координаты мировые сразу ичитывая scaleFactor
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 ScreenToWorldPoint(Vector3 point)
    {
        return Camera.ScreenToWorldPoint(point * Canvas.scaleFactor);
    }

    /// <summary>
    /// Переводит кооддинаты мировые в координаты экрана сразу ичитывая scaleFactor
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 WorldToScreenPoint(Vector3 point)
    {
        return Camera.WorldToScreenPoint(point) / Canvas.scaleFactor;
    }

    public Vector2 ClampToScreenRect(Vector2 screenPos, Vector2 rectSize)
    {
        Vector2 res = new Vector2(
            Mathf.Clamp(screenPos.x, rectSize.x / 2, Camera.pixelWidth / Canvas.scaleFactor - rectSize.x / 2),
            Mathf.Clamp(screenPos.y, rectSize.y / 2, Camera.pixelHeight / Canvas.scaleFactor - rectSize.y / 2)
        );
        return res;
    }
}
