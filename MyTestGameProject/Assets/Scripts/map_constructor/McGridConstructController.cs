using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class McGridConstructController :
    MonoBehaviour,
    IDragHandler,
    IPointerDownHandler
{
    McGrid grid;

    McFileManager fileManager;

    [SerializeField] float brushSize = 0;
    public float BrushSize { get { return brushSize; } }

    public enum GroundType { GRASSLAND, DESERT, ROCK, NEATHER, FOREST}

    [SerializeField] GroundType groundTypeName;
    public GroundType GroundTypeName
    {
        get
        {
            return groundTypeName;
        }
    }

    void Start()
    {
        grid = gameObject.GetComponent<McGrid>();
        fileManager = gameObject.GetComponent<McFileManager>();
    }

    public void BrushSizeSlider(float newValue)
    {
        brushSize = newValue;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!McGrid.hasInputedBlock && !McToggleTypeOfBlock.BlocksSelected)
        {
            Vector2 coordinates = Camera.main.ScreenToWorldPoint(eventData.position);
            DrawGrid(coordinates, eventData.button);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Middle)
        {
            Vector2 coordinates = Camera.main.ScreenToWorldPoint(eventData.position);

            if (!McGrid.hasInputedBlock && !McToggleTypeOfBlock.BlocksSelected)
            {
                DrawGrid(coordinates, eventData.button);
            }

            if (McGrid.hasInputedBlock)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                    grid.InputMapBlock();
                else
                    grid.ReseteInputMapBlock();
            }

            if (McToggleTypeOfBlock.BlocksSelected)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (coordinates.x >= 0 && coordinates.y >= 0 && 
                        coordinates.x <= McGrid.HORISONTAL_BLOCK_COUNT * MapBlock.WORLD_BLOCK_SIZE &&
                        coordinates.y <= McGrid.VERTICAL_BLOCK_COUNT * MapBlock.WORLD_BLOCK_SIZE)
                    {
                        McFileManager.mapBlocks = grid.GetSelectedBlocks();
                        fileManager.GeneradeFileName();
                        fileManager.ShowDialogPanel();
                    }
                }
                else
                    McToggleTypeOfBlock.DeselectAll();
            }
        }
    }
    
    void DrawGrid(Vector2 coordinates, PointerEventData.InputButton button)
    {
        if (!McToggleTypeOfBlock.BlocksSelected)
        {
            switch (button)
            {
                case PointerEventData.InputButton.Left:
                    grid.FillCell(coordinates, BrushSize, true);
                    break;
                case PointerEventData.InputButton.Right:
                    grid.FillCell(coordinates, BrushSize, false);
                    break;
            }
        }
    }
}