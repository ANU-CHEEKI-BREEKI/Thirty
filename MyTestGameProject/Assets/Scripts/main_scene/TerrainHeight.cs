using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class TerrainHeight : MonoBehaviour
{
    [SerializeField] [Range(-1, 1)] float outterAddWalkerSizeInPercent = 0;
    [SerializeField] [Range(-1, 1)] float innerAddWalkerSizeInPercent = 0;
    [Space]
    [SerializeField] Transform center;
    [SerializeField] float distance;
    Vector2 centerPos;

    int rHitLayer;
    ContactFilter2D rHitFilter;

    RaycastHit2D hit, hit2;

    private void Start()
    {
        centerPos = center.position;

        rHitLayer = 1 << LayerMask.NameToLayer("TERRAIN_HEIGHT");
        rHitFilter = new ContactFilter2D()
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = new LayerMask()
            {
                value = rHitLayer
            }
        };
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var walker = collision.gameObject.GetComponent<ITerrainWalker>();
        var walkerPos = walker.Position;

        hit = Physics2D.Linecast(centerPos, walkerPos, rHitLayer);

        if (hit)
        {
            var a = -centerPos.y + walkerPos.y;
            var b = -centerPos.x + walkerPos.x;
            var c = Vector2.Distance(centerPos, walkerPos);
            var sin = a / c;
            var cos = b / c;
            Vector2 p = new Vector2();
            p.x = distance * cos + centerPos.x;
            p.y = distance * sin + centerPos.y;

            hit2 = Physics2D.Linecast(p, centerPos, rHitLayer);

            float distBetwenBorders = Vector2.SqrMagnitude(hit.point - hit2.point);
            float distToWalkerFromInnerBorder = Vector2.SqrMagnitude(hit.point - walkerPos);
            float k = distToWalkerFromInnerBorder / distBetwenBorders;

            CalculateSize(walker, k);
        }
       
        Debug.DrawLine(centerPos, walker.Position, Color.gray);
    }

    void CalculateSize(ITerrainWalker walker, float distanceKoeficient)
    {
        var addScale = Mathf.Lerp(innerAddWalkerSizeInPercent, outterAddWalkerSizeInPercent, distanceKoeficient);
        walker.Scale = 1 + addScale;
    }

    private void OnDrawGizmos()
    {
        var outterField = GetComponent<PolygonCollider2D>();

        Color color = Color.blue;
        int cnt = outterField.points.Length - 1;
        for (int i = 0; i < cnt; i++)
            Debug.DrawLine(transform.TransformPoint(outterField.points[i]), transform.TransformPoint(outterField.points[i + 1]), color);
        Debug.DrawLine(transform.TransformPoint(outterField.points[0]), transform.TransformPoint(outterField.points[outterField.points.Length - 1]), color);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(center.position, 0.5f);

        Gizmos.DrawSphere(hit.point, 0.3f);
        Gizmos.DrawSphere(hit2.point, 0.3f);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(center.position, distance);
    }
}
