using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class SquadInfoPanel : MonoBehaviour
{
    [SerializeField] Squad squad;
    public Squad Squad { get { return squad; } set { squad = value; } }
    [Space]
    [SerializeField] SquadWeight squadWeight;
    [SerializeField] MainStatesPanel mainStatesPanel;
    [SerializeField] HealthBar healthBar;
    [SerializeField] MainStatsPanel mainStatsPanel;

    Transform thisTransform;

    static Action OnShowChanged;
    static bool show = true;
    static public bool Show
    {
        get { return show; }
        set
        {
            if (show != value)
            {
                show = value;
                if (OnShowChanged != null)
                    OnShowChanged();
            }
        }
    }

    static Camera camera;
    bool inCamera = true;
    bool InCamera
    {
        set
        {
            if (inCamera != value)
            {
                inCamera = value;
                Present();
            }
        }
    }
    float orthographicSize;

    bool inMask = true;
    bool InMask
    {
        set
        {
            if (inMask != value)
            {
                inMask = value;
                Present();
            }
        }
    }

    CanvasGroup cg;


    private void Awake()
    {
        OnShowChanged += Present;
        if(camera == null)
            camera = Camera.main;

        orthographicSize = camera.orthographicSize;

        thisTransform = transform;

        cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        var active = show && inCamera && inMask;
        cg.alpha = active ? 1 : 0;
    }

    private void Start()
    {
        if (squad == null)
            squad = Squad.playerSquadInstance;

        squadWeight.Squad = squad;
        mainStatesPanel.Squad = squad;
        healthBar.Squad = squad;
        mainStatsPanel.Squad = squad;

        Present();
    }

    private void OnDestroy()
    {
        OnShowChanged -= Present;
    }

    private void Update()
    {
        SetScaleAndVisible();
    }

    void SetScaleAndVisible()
    {
        if (squad != null)
        {
            if (orthographicSize != camera.orthographicSize)
            {
                orthographicSize = camera.orthographicSize;
                thisTransform.localScale = orthographicSize * 0.0015f * Vector3.one; //orthographicSize * 0.05f * (Vector3.one * 0.03f); 
            }

            var v = camera.WorldToViewportPoint(thisTransform.position);
            if (v.x > 0 && v.x < 1 && v.y > 0 && v.y < 1)
                InCamera = true;
            else
                InCamera = false;

            var mask = SquadMask.Instance;
            if (inCamera)
            {
                if (squad.Hiding && mask != null)
                {
                    var size = mask.Size / 2;
                    if (Vector2.SqrMagnitude((Vector2)thisTransform.position - mask.Position) <= size * size)
                        InMask = true;
                    else
                        InMask = false;
                }
                else
                    InMask = true;
            }
        }
    }
    
    void Present()
    {
        var active = show && inCamera && inMask;
        cg.alpha = active ? 1 : 0;

        squadWeight.Active = active;
        mainStatesPanel.Active = active;
        healthBar.Active = active;
        mainStatsPanel.Active = active;
    }

}
