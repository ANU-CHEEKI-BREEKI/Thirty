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

    //слои, через которые не будет искаться путь напрямую
    [SerializeField] LayerMask directFindPathLayers;

    [SerializeField] [Range(0, 10)] float lookVectorDistanse = 2;
    [SerializeField] [Range(0, 10)] float cicleCastRadius = 3;
    [SerializeField] [Range(0, 1)] float circleConstrincInPercent = 0.2f;
    [SerializeField] [Range(0, 1)] float circleConstrincGuaranteeUnits = 0.2f;
    [SerializeField] [Range(0, 1)] float circleExpandDurationInSec = 2f;
    [SerializeField] [Range(0, 1)] float circleExpandDeltaTime = 0.2f;
    [SerializeField] [Range(0, 1)] float circleMinRadius = 0.5f;
    
    bool coroutineAlive = false;

    float rHitCirclreAngle = 20;
    int rHitCirclreSegmentCount;

    Vector3[] rHitCirclreSegments;
    float[] sinAngles;
    float[] cosAngles;

    Thread thread;

    List<Vector3> path;
    Vector2 startFindPath;
    Vector2 endFindPath;

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
        ground = GameObject.FindWithTag("Ground").GetComponent<Ground>();
        squad = Squad.playerSquadInstance;

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        rHits = new RaycastHit2D[maxHitsCount];

        rHitCirclreSegmentCount = (int)(360 / rHitCirclreAngle) + 1;

        rHitCirclreSegments = new Vector3[rHitCirclreSegmentCount];
        sinAngles = new float[rHitCirclreSegmentCount];
        cosAngles = new float[rHitCirclreSegmentCount];

        switch (squad.fraction)
        {
            case Squad.UnitFraction.ALLY:
                rHitLayer = 1 << LayerMask.NameToLayer(Squad.UnitFraction.ENEMY.ToString());
                rHitLayer = rHitLayer | 1 << LayerMask.NameToLayer(Squad.UnitFraction.NEUTRAL.ToString());
                break;
            case Squad.UnitFraction.ENEMY:
                rHitLayer = 1 << LayerMask.NameToLayer(Squad.UnitFraction.ALLY.ToString());
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

                        lineRenderer.positionCount = rHitCirclreSegmentCount;
                        lineRenderer.SetPositions(rHitCirclreSegments);

                        break;
                }
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
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

    void CreateSinCosAngles()
    {
        float angle = rHitCirclreAngle;
        for (int i = 0; i < (rHitCirclreSegmentCount); i++)
        {
            sinAngles[i] = Mathf.Sin(Mathf.Deg2Rad * angle);
            cosAngles[i] = Mathf.Cos(Mathf.Deg2Rad * angle);
            angle += rHitCirclreAngle;
        }
    }

    void CreatePoints()
    {
        float x;
        float y;

        for (int i = 0; i < (rHitCirclreSegmentCount); i++)
        {
            x = sinAngles[i] * cicleCastRadius + lookPosition.x;
            y = cosAngles[i] * cicleCastRadius + lookPosition.y;
            
            rHitCirclreSegments[i] = new Vector3(x, y, -9);
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

                switch (squad.CurrentFormation)
                {
                    case FormationStats.Formations.PHALANX:
                        lookPosition = Camera.main.ScreenToWorldPoint(touch.position);
                        lookPosition = new Vector3(lookPosition.x, lookPosition.y, transform.position.z);

                        if (Vector3.Distance(lookPosition, movePosition) >= lookVectorDistanse)
                            lookRotation = Quaternion.LookRotation(Vector3.forward, lookPosition - movePosition);
                        else
                            lookRotation = Quaternion.LookRotation(Vector3.forward, movePosition - squad.PositionsTransform.position);
                        break;

                    default :// Formation.Formations.RANKS:
                        movePosition = Camera.main.ScreenToWorldPoint(touch.position);
                        movePosition = new Vector3(movePosition.x, movePosition.y, transform.position.z);
                        lookRotation = Quaternion.LookRotation(Vector3.forward, movePosition - squad.PositionsTransform.position);
                        SelectEnemyes();
                        if (rHitsCount > 0)
                            StartCoroutine(CircleCastConstrict());
                        break;
                }

                if (thread == null || !thread.IsAlive)
                {
                    SetStartPosition();

                    startFindPath = new Vector2(
                        Mathf.Round(squad.PositionsTransform.position.x / MapBlock.BLOCK_SCALE),
                        Mathf.Round(squad.PositionsTransform.position.y / MapBlock.BLOCK_SCALE)
                    );
                    endFindPath = new Vector2(
                        Mathf.Round(movePosition.x / MapBlock.BLOCK_SCALE),
                        Mathf.Round(movePosition.y / MapBlock.BLOCK_SCALE)
                    );

                    if (endFindPath.y >= 0 && endFindPath.y < ground.RowCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE / MapBlock.BLOCK_SCALE &&
                        endFindPath.x >= 0 && endFindPath.x < ground.ColCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE / MapBlock.BLOCK_SCALE)
                    {
                        if (!ground.Grid[(int)endFindPath.y][(int)endFindPath.x])
                        {
                            RaycastHit2D rhit = Physics2D.Linecast(squad.PositionsTransform.position, movePosition, directFindPathLayers.value);

                            if (rhit.collider == null)
                            {
                                path = new List<Vector3>();
                                path.Add(squad.transform.position);
                                path.Add(movePosition);
                                squad.SetEndMovePositions(movePosition, lookRotation);
                                squad.GoTo(path);
                            }
                            else
                            {
                                thread = new Thread(FindPath);
                                thread.Start();
                            }
                        }
                    }
                }
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
            if (unit != null)
                unit.Selected = true;
        }
    }

    void FindPath()
    {
        path = Labirinth.FindPathLee(ground.Grid, startFindPath, endFindPath, MapBlock.BLOCK_SCALE);

        if (path != null)
            squad.SetEndMovePositions(movePosition, lookRotation);

        squad.GoTo(path);
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
