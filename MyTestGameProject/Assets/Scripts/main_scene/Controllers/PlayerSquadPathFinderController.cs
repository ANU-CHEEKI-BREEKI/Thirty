using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSquadPathFinderController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Camera cameraToControl = null;
    [SerializeField] Squad squadToControl = null;

    float lookVectorDistanse = 1;

    List<Vector3> path;
    Vector2 startFindPath;
    Vector2 endFindPath;

    Vector3 lookPosition;
    Vector3 movePosition;

    bool mouseDown = false;
    int touchId;

    Ground ground;

    Thread thread;

    RawImage minimap;

    void Start()
    {
        if (cameraToControl == null)
            cameraToControl = Camera.main;

        if (squadToControl == null)
            squadToControl = Squad.playerSquadInstance;

        ground = Ground.Instance;

        minimap = GetComponent<RawImage>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (squadToControl != null && !GameManager.Instance.GamePaused && !mouseDown && !squadToControl.Charging)
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

            movePosition = TextureToWorldPosition(touch.position);
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

            if (squadToControl != null && !GameManager.Instance.GamePaused && !squadToControl.Charging)
            {
                mouseDown = false;

                switch (squadToControl.CurrentFormation)
                {
                    case FormationStats.Formations.PHALANX:
                        lookPosition = cameraToControl.ScreenToWorldPoint(touch.position);
                        lookPosition = new Vector3(lookPosition.x, lookPosition.y, transform.position.z);
                        break;

                    default:// Formation.Formations.RANKS:
                        movePosition = TextureToWorldPosition(touch.position);
                        movePosition = new Vector3(movePosition.x, movePosition.y, transform.position.z);
                        lookPosition = movePosition;
                        break;
                }

                squadToControl.Controller.MoveToPoint(movePosition);
                squadToControl.Controller.RotateAfterMoving(Quaternion.LookRotation(Vector3.forward, movePosition - squadToControl.PositionsTransform.position));
            }
        }


    }

    Vector2 TextureToWorldPosition(Vector2 touchPos)
    {
        Rect rect = minimap.rectTransform.rect;

        Vector2 minimapPos = Camera.main.WorldToScreenPoint(minimap.rectTransform.position);

        touchPos.x -= minimapPos.x - rect.width * 0.5f;
        touchPos.y -= minimapPos.y - rect.height * 0.5f;

        float scale;
        if (Ground.Instance.ColCountOfBlocks > Ground.Instance.RowCountOfBlocks)
        {
            scale = rect.width / (Ground.Instance.ColCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE);
            touchPos.y -= (rect.height - Ground.Instance.RowCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE * scale) * 0.5f;
        }
        else
        {
            scale = rect.height / (Ground.Instance.RowCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE);
            touchPos.x -= (rect.width - Ground.Instance.ColCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE * scale) * 0.5f;
        }

        var res = new Vector2(touchPos.x / scale, touchPos.y / scale);

        return res;
    }

}
