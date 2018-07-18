using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShowColliderGizmos : MonoBehaviour
{
    enum ColType { POLYGON, CIRCLE, BOX }

    Transform thisTransform;
    Collider2D col;

    Vector2[] points;
    float radius;
    Vector2 size;
    ColType colType;
    bool round;

    private void Awake()
    {
        thisTransform = transform;
        col = GetComponent<Collider2D>();
        GetPoints(col);
    }

    private void GetPoints(Collider2D col)
    {
        if (col is PolygonCollider2D)
        {
            points = (col as PolygonCollider2D).points;
            colType = ColType.POLYGON;
            round = true;
        }
        else if (col is EdgeCollider2D)
        {
            points = (col as EdgeCollider2D).points;
            colType = ColType.POLYGON;
            round = false;
        }
        else if (col is CircleCollider2D)
        {
            radius = (col as CircleCollider2D).radius;
            colType = ColType.CIRCLE;
        }
        else if (col is BoxCollider2D)
        {
            size = (col as BoxCollider2D).size;
            colType = ColType.BOX;
        }
    }

    private void OnDrawGizmos()
    {
        if(thisTransform == null)
            thisTransform = transform;

        if(col == null)
            col = GetComponent<Collider2D>();

        GetPoints(col);

        switch (colType)
        {
            case ColType.POLYGON:
                DrawPoly();
                break;
            case ColType.CIRCLE:
                DrawCircle();
                break;
            case ColType.BOX:
                DrawBox();
                break;
        }
    }

    void DrawBox()
    {
        Gizmos.DrawWireCube(thisTransform.position, size);
    }

    void DrawCircle()
    {
        Gizmos.DrawWireSphere(thisTransform.position, radius);
    }

    void DrawPoly()
    {
        Vector2 pos = thisTransform.position;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Debug.DrawLine(pos + points[i], pos + points[i + 1]);
        }
        if (round)
            Debug.DrawLine(pos + points[0], pos + points[points.Length - 1]);
    }
}
