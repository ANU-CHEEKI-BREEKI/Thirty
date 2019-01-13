using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MainCanvases : MonoBehaviour
{
    static MainCanvases mainInstance;
    public static MainCanvases MainInstance
    {
        get
        {
            if (mainInstance == null)
            {
                mainInstance = GameObject.FindWithTag("MainCanvas").AddComponent<MainCanvases>();
                mainInstance.Clear();
            }
            return mainInstance;
        }

        private set
        {
            mainInstance = value;
        }
    }

    static MainCanvases dialogsInstance;
    public static MainCanvases DialogsInstance
    {
        get
        {
            if (dialogsInstance == null)
            {
                dialogsInstance = GameObject.FindWithTag("DialogsCanvas").AddComponent<MainCanvases>();
                dialogsInstance.Clear();
            }
            return dialogsInstance;
        }

        private set
        {
            dialogsInstance = value;
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

    new Camera camera;
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
        Canvas = GetComponent<Canvas>();
        Camera = Camera.main;
    }

    private void Start()
    {
        Clear();
    }

    [ContextMenu("Clear")]
    void Clear()
    {
        var arr = gameObject.GetComponents<MainCanvases>();
        foreach (var item in arr)
            if (item != mainInstance && item != dialogsInstance)
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
