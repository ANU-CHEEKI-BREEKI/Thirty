using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UILineConnector : MonoBehaviour
{
    [SerializeField] UILineRenderer line;
    [SerializeField] Color enabledColor;
    [SerializeField] Color disabledColor;
    [Space]
    [SerializeField] Transform prevObj;
    public Transform PrevObj { set { prevObj = value; } }

    void Start()
    {
        DrawLine();
    }

#if UNITY_EDITOR

    void Update()
    {
        DrawLine();
    }

#endif

    bool enabled = true;
    public bool Enabled
    {
        set
        {
            if (line != null)
            {
                enabled = value;
                if (value)
                    line.color = enabledColor;
                else
                    line.color = disabledColor;
            }
        }
        get { return enabled; }
    }

    void DrawLine()
    {
        if (line != null && prevObj != null)
        {

            Camera mainCam = Camera.main;

            int pointsCount = 1;
            Vector2 nextPos = prevObj.position;
            Vector2 thisPos = transform.position;

            if (nextPos.x != thisPos.x) pointsCount++;
            if (nextPos.y != thisPos.y) pointsCount++;
            if (pointsCount == 3) pointsCount++;

            Vector2[] points = new Vector2[pointsCount];

            Vector2 prevPosScreen = mainCam.WorldToScreenPoint(nextPos);
            Vector2 thisPosScreen = mainCam.WorldToScreenPoint(thisPos);

            Vector2 prevPosShift = prevPosScreen - thisPosScreen;

            points[0] = prevPosShift;
            points[pointsCount - 1] = Vector2.zero;

            float dist = prevPosShift.x / 2 - ((RectTransform)transform).rect.width / 2;

            if (pointsCount == 4)
            {
                points[1] = new Vector2(dist, prevPosShift.y);
                points[2] = new Vector2(dist, 0);
            }
            line.Points = points;
        }
    }
}
