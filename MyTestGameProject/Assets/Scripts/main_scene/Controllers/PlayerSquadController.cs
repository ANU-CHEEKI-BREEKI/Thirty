using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSquadController : MonoBehaviour,  IPointerDownHandler, IPointerUpHandler
{
    public static PlayerSquadController Instance { get; private set; }

    Squad squad;
    Ground ground;
    
    [SerializeField] [Range(0, 10)] float lookVectorDistanse = 2;
    [SerializeField] [Range(0, 10)] float cicleCastRadius = 3;
    [SerializeField] [Range(0, 1)] float circleConstrincInPercent = 0.2f;
    [SerializeField] [Range(0, 1)] float circleConstrincGuaranteeUnits = 0.2f;
    [SerializeField] [Range(0, 1)] float circleExpandDurationInSec = 2f;
    [SerializeField] [Range(0, 1)] float circleExpandDeltaTime = 0.2f;
    [SerializeField] [Range(0, 1)] float circleMinRadius = 0.5f;
    
    bool coroutineAlive = false;

    float circlreAngle = 20;
    int circlreSegmentCount;

    Vector3[] circlreSegments;
    float[] sinAngles;
    float[] cosAngles;

    Vector3 lookPosition;
    Vector3 movePosition;
    Quaternion lookRotation;

    LineRenderer lineRenderer;

    [SerializeField] [Range(1, 1000)] int maxHitsCount = 100;

    RaycastHit2D[] rHits;
    int rHitsCount = 0;
    ContactFilter2D rHitFilter;
    int rHitLayer;

    bool mouseDown = false;

    int touchId;

    void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
        ground = Ground.Instance;
        squad = Squad.playerSquadInstance;

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        rHits = new RaycastHit2D[maxHitsCount];

        circlreSegmentCount = (int)(360 / circlreAngle) + 1;

        circlreSegments = new Vector3[circlreSegmentCount];
        sinAngles = new float[circlreSegmentCount];
        cosAngles = new float[circlreSegmentCount];

        switch (squad.fraction)
        {
            case Squad.UnitFraction.ALLY:
                rHitLayer = 1 << LayerMask.NameToLayer(Squad.UnitFraction.ENEMY.ToString());
                rHitLayer = rHitLayer | 1 << LayerMask.NameToLayer(Squad.UnitFraction.NEUTRAL.ToString());
                rHitLayer = rHitLayer | 1 << LayerMask.NameToLayer("FALLEN_ENEMY");
                rHitLayer = rHitLayer | 1 << LayerMask.NameToLayer("FALLEN_NEUTRAL");
                break;
            case Squad.UnitFraction.ENEMY:
                rHitLayer = 1 << LayerMask.NameToLayer(Squad.UnitFraction.ALLY.ToString());
                rHitLayer = rHitLayer | 1 << LayerMask.NameToLayer("FALLEN_ALLY");
                break;
        }
        


        rHitFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = new LayerMask()
            {
                value = rHitLayer
            }
        };

        CreateSinCosAngles();
    }

    void Update()
    {
        if (squad != null)
        {
            if (mouseDown)
            {

                int count = Input.touchCount;
                Touch touch = new Touch();
                if (count > 0)
                {
                    foreach (var item in Input.touches)
                    {
                        if (item.fingerId == touchId)
                        {
                            touch = item;
                            break;
                        }
                    }
                }
                else
                {
                    touch.position = Input.mousePosition;
                }                

                lookPosition = Camera.main.ScreenToWorldPoint(touch.position);
                lookPosition = new Vector3(lookPosition.x, lookPosition.y, transform.position.z);

                switch (squad.CurrentFormation)
                {
                    case FormationStats.Formations.PHALANX:
                        lineRenderer.positionCount = 2;
                        lineRenderer.SetPositions(new Vector3[] { movePosition, lookPosition });

                        break;

                    default: // Formation.Formations.RANKS:
                        CreatePoints();

                        lineRenderer.positionCount = circlreSegmentCount;
                        lineRenderer.SetPositions(circlreSegments);

                        break;
                }
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }
    }

    

    void CreateSinCosAngles()
    {
        float angle = circlreAngle;
        for (int i = 0; i < (circlreSegmentCount); i++)
        {
            sinAngles[i] = Mathf.Sin(Mathf.Deg2Rad * angle);
            cosAngles[i] = Mathf.Cos(Mathf.Deg2Rad * angle);
            angle += circlreAngle;
        }
    }

    void CreatePoints()
    {
        float x;
        float y;

        for (int i = 0; i < (circlreSegmentCount); i++)
        {
            x = sinAngles[i] * cicleCastRadius + lookPosition.x;
            y = cosAngles[i] * cicleCastRadius + lookPosition.y;
            
            circlreSegments[i] = new Vector3(x, y, -1);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (squad != null && !GameManager.Instance.GamePaused && !mouseDown && !squad.Charging)
        {
            mouseDown = true;

            int count = Input.touchCount;
            Touch touch;
            if (count > 0)
            {
                touch = Input.GetTouch(Input.touchCount - 1);
                touchId = touch.fingerId;
            }
            else
            {
                touch = new Touch();
                touch.position = Input.mousePosition;
            }

            movePosition = Camera.main.ScreenToWorldPoint(touch.position);
            movePosition = new Vector3(movePosition.x, movePosition.y, transform.position.z);
            lookPosition = movePosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        int count = Input.touchCount;
        Touch touch;
        if (count > 0)
        {
            touch = Input.GetTouch(Input.touchCount - 1);
        }
        else
        {
            touch = new Touch();
            touch.position = Input.mousePosition;
        }

        if (touch.fingerId == touchId || count == 0)
        {
            if (squad != null && !GameManager.Instance.GamePaused && !squad.Charging)
            {
                mouseDown = false;

                lookRotation = Quaternion.LookRotation(Vector3.forward, movePosition - squad.PositionsTransform.position);

                switch (squad.CurrentFormation)
                {
                    case FormationStats.Formations.PHALANX:
                        lookPosition = Camera.main.ScreenToWorldPoint(touch.position);
                        lookPosition = new Vector3(lookPosition.x, lookPosition.y, transform.position.z);

                        if (Vector3.Distance(lookPosition, movePosition) >= lookVectorDistanse)
                            lookRotation = Quaternion.LookRotation(Vector3.forward, lookPosition - movePosition);

                        break;

                    default:// Formation.Formations.RANKS:
                        movePosition = Camera.main.ScreenToWorldPoint(touch.position);
                        movePosition = new Vector3(movePosition.x, movePosition.y, transform.position.z);
                        SelectEnemyes();
                        if (rHitsCount > 0)
                            StartCoroutine(CircleCastConstrict());
                        break;
                }

                squad.Controller.MoveToPoint(movePosition);
                squad.Controller.RotateAfterMoving(lookRotation);
            }
        }
    }

    void SetStartPosition()
    {
        //
        var angle = Quaternion.LookRotation(Vector3.forward, movePosition - squad.PositionsTransform.position).eulerAngles.z;

        float angleLook = squad.PositionsTransform.rotation.eulerAngles.z;
        if (squad.FlipRotation) angleLook += 180;

        var sqrt = Mathf.Sqrt(0.5f);

        var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        var sin2 = Mathf.Sin(angleLook * Mathf.Deg2Rad);

        var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        var cos2 = Mathf.Cos(angleLook * Mathf.Deg2Rad);
        
        if ( (Mathf.Abs(cos2) >= sqrt && cos * cos2 < 0) || (Mathf.Abs(sin2) >= sqrt && sin * sin2 < 0))
        { 
            RaycastHit2D rhit = Physics2D.CircleCast(
                squad.CenterSquad,
                0.1f,
                Vector2.zero,
                0,
                1 << LayerMask.NameToLayer("MAP")
            );

            if (!rhit)
                squad.PositionsTransform.position = squad.CenterSquad;
        }
    }

    void SelectEnemyes()
    {
        DeselectEnemyes();

        rHitsCount = Physics2D.CircleCast(movePosition, cicleCastRadius, Vector2.zero, rHitFilter, rHits);

        for (int i = 0; i < rHitsCount; i++)
        {
            Unit unit = rHits[i].transform.gameObject.GetComponent<Unit>();
            if (unit != null && unit.IsAlive)
                unit.Selected = true;
        }
    }

    public void DeselectEnemyes()
    {
        for (int i = 0; i < rHitsCount; i++)
        {
            if (rHits[i])
            {
                Unit unit = rHits[i].transform.gameObject.GetComponent<Unit>();
                if (unit != null)
                    unit.Selected = false;
            }
        }
    }
    
    IEnumerator CircleCastConstrict()
    {
        if (cicleCastRadius > circleMinRadius)
        {
            float dec = cicleCastRadius * circleConstrincInPercent;
            if (dec < circleConstrincGuaranteeUnits)
                dec = circleConstrincGuaranteeUnits;

            cicleCastRadius -= dec;

            float inc = 0;

            int ticks = (int)(circleExpandDurationInSec / circleExpandDeltaTime);
            float incPerStep = dec / ticks;

            while (coroutineAlive)
            {
                yield return new WaitForSeconds(circleExpandDeltaTime);
            }

            coroutineAlive = true;

            for (int i = 0; i < ticks; i++)
            {
                cicleCastRadius += incPerStep;
                inc += incPerStep;

                yield return new WaitForSeconds(circleExpandDeltaTime);
            }

            cicleCastRadius += dec - inc;

            coroutineAlive = false;
        }
    }
}
