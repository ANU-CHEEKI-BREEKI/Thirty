using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UILineConnector : MonoBehaviour
{
    [SerializeField] UILineRenderer line;
    [SerializeField] Color enabledColor;
    [SerializeField] Color disabledColor;
    [Space]
    [SerializeField] Transform prevObj;
    public Transform PrevObj { set { prevObj = value; } }
    [SerializeField] Vector2 shiftThis;
    [SerializeField] Vector2 shiftPrev;
    [SerializeField] Vector2 shiftCenter;

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
        //ВНИМАНИЕ!!!!!!
        //точки в лайнрендерере надо указывать в локальных координатах!

        if (line != null && prevObj != null)
        {
            Camera mainCam = Camera.main;

            Vector2 prevPos = prevObj.position;
            Vector2 thisPos = transform.position;

            int pointsCount = 2;
            if (prevPos.x != thisPos.x && prevPos.y != thisPos.y)
                pointsCount = 4;

            Vector2[] points = new Vector2[pointsCount];

            Vector2 prevPosScreen = MainCanvas.Instance.WorldToScreenPoint(prevPos);
            Vector2 thisPosScreen = MainCanvas.Instance.WorldToScreenPoint(thisPos);

            //точки в лайнрендерере надо указывать в локальных координатах!
            Vector2 prevScreenLocalPos = prevPosScreen + shiftPrev - thisPosScreen - shiftThis;

            points[0] = prevScreenLocalPos;
            points[pointsCount - 1] = Vector2.zero;

            if (pointsCount == 4)
            {             
                //при расположении линии горизонтально
                points[1] = new Vector2(prevScreenLocalPos.x / 2, prevScreenLocalPos.y) + shiftCenter;
                points[2] = new Vector2(prevScreenLocalPos.x / 2, 0) + shiftCenter;

                //для вертикального расположения можно будет дорботать. пока что не надо.
                // ...
            }

            line.Points = points;
        }
    }
}
