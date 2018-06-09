using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class McBlockEntryChooser :
    MonoBehaviour,
    IDragHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    MapBlock.Direction entryDirection;
    
    [SerializeField] Toggle left, right, top, bottom;
    McBlockChooserDirectionToggleFlag leftFlag, rightFlag, topFlag, bottomFlag;

    [SerializeField] McFileManager fileManager;

    float dragDistanseX, dragDistanseY;
    Vector2 mouseDownPosition;

    static List<MapBlock.Direction> blockEntrance;
    public static List<MapBlock.Direction> BlockEntrance
    {
        get
        {
            return blockEntrance;
        }
    }

    static McBlockEntryChooser[] brothersToggles = null;

    void Start ()
    {
        blockEntrance = new List<MapBlock.Direction>();
        if (brothersToggles == null)
            brothersToggles = transform.parent.GetComponentsInChildren<McBlockEntryChooser>();
    }

    public static void Show(McToggleTypeOfBlock.TypeOfBlock typeofBlock)
    {
        foreach (var item in brothersToggles)
            item.gameObject.SetActive(false);

        switch (typeofBlock)
        {
            case McToggleTypeOfBlock.TypeOfBlock.DOT:
                brothersToggles[1].gameObject.SetActive(true);
                break;
            case McToggleTypeOfBlock.TypeOfBlock.VPLANE:
                brothersToggles[1].gameObject.SetActive(true);
                brothersToggles[2].gameObject.SetActive(true);
                break;
            case McToggleTypeOfBlock.TypeOfBlock.HPLANE:
                brothersToggles[1].gameObject.SetActive(true);
                brothersToggles[0].gameObject.SetActive(true);
                break;
            case McToggleTypeOfBlock.TypeOfBlock.SQUARE:
                foreach (var item in brothersToggles)
                    item.gameObject.SetActive(true);
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        dragDistanseX = dragPosition.x - mouseDownPosition.x;
        dragDistanseY = dragPosition.y - mouseDownPosition.y;

        if (Mathf.Abs(dragDistanseX) > Mathf.Abs(dragDistanseY))
        {
            if (!topFlag.IsSelected)
                top.isOn = false;

            if (!bottomFlag.IsSelected)
                bottom.isOn = false;

            if (!rightFlag.IsSelected)
            {
                if (dragDistanseX > 0)
                    right.isOn = true;
                else
                    right.isOn = false;
            }

            if (!leftFlag.IsSelected)
            {
                if (dragDistanseX < 0)
                    left.isOn = true;
                else
                    left.isOn = false;
            }
        }
        else
        {
            if (!rightFlag.IsSelected)
                right.isOn = false;

            if (!leftFlag.IsSelected)
                left.isOn = false;

            if (!topFlag.IsSelected)
            {
                if (dragDistanseY > 0)
                    top.isOn = true;
                else
                    top.isOn = false;
            }

            if (!bottomFlag.IsSelected)
            {
                if (dragDistanseY < 0)
                    bottom.isOn = true;
                else
                    bottom.isOn = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDownPosition = Camera.main.ScreenToWorldPoint(eventData.position);
               
        foreach (var item in brothersToggles)
            item.GetFlags();
    }

    public void GetFlags()
    {
        if (leftFlag == null)
            leftFlag = left.GetComponent<McBlockChooserDirectionToggleFlag>();

        if (rightFlag == null)
            rightFlag = right.GetComponent<McBlockChooserDirectionToggleFlag>();

        if (topFlag == null)
            topFlag = top.GetComponent<McBlockChooserDirectionToggleFlag>();

        if (bottomFlag == null)
            bottomFlag = bottom.GetComponent<McBlockChooserDirectionToggleFlag>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool needToAdd = false;

        if (!leftFlag.IsSelected && left.isOn)
        {
            leftFlag.IsSelected = true;
            entryDirection = MapBlock.Direction.LEFT;
            needToAdd = true;
        }

        if (!rightFlag.IsSelected && right.isOn)
        { 
            rightFlag.IsSelected = true;
            entryDirection = MapBlock.Direction.RIGHT;
            needToAdd = true;
        }

        if (!topFlag.IsSelected && top.isOn)
        { 
            topFlag.IsSelected = true;
            entryDirection = MapBlock.Direction.TOP;
            needToAdd = true;
        }

        if (!bottomFlag.IsSelected && bottom.isOn)
        {
            bottomFlag.IsSelected = true;
            entryDirection = MapBlock.Direction.BOTTOM;
            needToAdd = true;
        }

        if (needToAdd)
        {
            ((Toggle)gameObject.GetComponent<Toggle>()).isOn = true;
            AddEntrance();            
        }
    }

    void AddEntrance()
    {
        MapBlock.Direction entrance = new MapBlock.Direction();
        entrance = entryDirection;

        blockEntrance.Add(entrance);

        blockEntrance.Sort();

        fileManager.GeneradeFileName();
    }

    void RemoveEntrance()
    {
        foreach (var item in brothersToggles)
        {
            item.GetComponent<Toggle>().isOn = false;
            item.DeselectAllDirections();
        }

        blockEntrance.Clear();

        fileManager.GeneradeFileName();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameObject.GetComponent<Toggle>().isOn)
            RemoveEntrance();
    }

    public void DeselectAllDirections()
    {
        left.isOn = false;
        leftFlag.IsSelected = false;

        right.isOn = false;
        rightFlag.IsSelected = false;

        top.isOn = false;
        topFlag.IsSelected = false;

        bottom.isOn = false;
        bottomFlag.IsSelected = false;
    }
}
