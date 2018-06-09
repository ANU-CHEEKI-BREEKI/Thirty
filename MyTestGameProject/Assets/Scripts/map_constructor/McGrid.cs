using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class McGrid : MonoBehaviour
{  
    public const int HORISONTAL_BLOCK_COUNT = 2;
    public const int VERTICAL_BLOCK_COUNT = 2;

    Color DEFAULT_CELLS_FILL_GRID_COLOR = Color.white;
    Color DEFAULT_CELLS_EMPTY_GRID_COLOR = Color.gray;
    Color SELECTEDT_GRID_COLOR = Color.red;
    Color BLOCK_GRID_COLOR = Color.cyan;
    Color DEFAULT_CELLS_FILL_INPUTED_GRID_COLOR = Color.green;
    Color DEFAULT_CELLS_EMPTY_INPUTED_GRID_COLOR = Color.yellow;
        
    int gridWidth = MapBlock.BLOCK_SIZE * HORISONTAL_BLOCK_COUNT;
    int gridHeight = MapBlock.BLOCK_SIZE * VERTICAL_BLOCK_COUNT;
        
    static Vector2 selectedBlocksPosition;

    public static McGridConstructController controller;

    /// <summary>
    /// if true - wall in this plase, cant walc
    /// if false - empty cell, can walc
    /// </summary> 
    public static bool[][] Grid { get; private set; }

    public static bool hasInputedBlock = false;

    static MapBlock inputedMapBlock;
    public static MapBlock InputedMapBlock { get { return inputedMapBlock; } set { inputedMapBlock = value; } }
    static Vector2 inputedMapBlockPosition;

    void Start ()
    {
        Grid = new bool[gridHeight][];
        for (int row = 0; row < gridHeight; row++)
            Grid[row] = new bool[gridWidth];            
           
        controller = gameObject.GetComponent<McGridConstructController>();
    }
        
    private void DrawBlockAreas()
    {
        Color color = Gizmos.color;
        for (int row = 0; row < VERTICAL_BLOCK_COUNT; row++)
        {
            for (int col = 0; col < HORISONTAL_BLOCK_COUNT; col++)
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(
                    new Vector2(
                        col * MapBlock.WORLD_BLOCK_SIZE + MapBlock.WORLD_BLOCK_SIZE / 2 - 0.5f * MapBlock.BLOCK_SCALE,
                        row * MapBlock.WORLD_BLOCK_SIZE + MapBlock.WORLD_BLOCK_SIZE / 2 - 0.5f * MapBlock.BLOCK_SCALE
                    ),
                    new Vector2(
                        MapBlock.WORLD_BLOCK_SIZE, 
                        MapBlock.WORLD_BLOCK_SIZE
                    )
                );
                Gizmos.color = Color.yellow;
                //horisontal line on center of area
                Gizmos.DrawLine(
                    new Vector2(
                        col * MapBlock.WORLD_BLOCK_SIZE + MapBlock.WORLD_BLOCK_SIZE - MapBlock.BLOCK_SCALE,
                        row * MapBlock.WORLD_BLOCK_SIZE + MapBlock.WORLD_BLOCK_SIZE / 2 - 0.5f * MapBlock.BLOCK_SCALE
                    ),
                    new Vector2(
                        col * MapBlock.WORLD_BLOCK_SIZE,
                        row * MapBlock.WORLD_BLOCK_SIZE + MapBlock.WORLD_BLOCK_SIZE / 2 - 0.5f * MapBlock.BLOCK_SCALE
                    )
                );
                //vertical line on center of area
                Gizmos.DrawLine(
                    new Vector2(
                        col * MapBlock.WORLD_BLOCK_SIZE + MapBlock.WORLD_BLOCK_SIZE / 2 - 0.5f * MapBlock.BLOCK_SCALE,
                        row * MapBlock.WORLD_BLOCK_SIZE + MapBlock.WORLD_BLOCK_SIZE - MapBlock.BLOCK_SCALE
                    ),
                    new Vector2(
                        col * MapBlock.WORLD_BLOCK_SIZE + MapBlock.WORLD_BLOCK_SIZE / 2 - 0.5f * MapBlock.BLOCK_SCALE,
                        row * MapBlock.WORLD_BLOCK_SIZE
                    )
                );
            }
        }
    }

    private void DrawGrid(Vector2 mousePosition)
    {
        if (!McFileManager.DialogShowed)
        {
            float x = Mathf.Clamp(mousePosition.x / MapBlock.BLOCK_SCALE, 0, gridWidth - McToggleTypeOfBlock.Width * MapBlock.BLOCK_SIZE);
            float y = Mathf.Clamp(mousePosition.y / MapBlock.BLOCK_SCALE, 0, gridHeight - McToggleTypeOfBlock.Height * MapBlock.BLOCK_SIZE);

            x = x / gridWidth * HORISONTAL_BLOCK_COUNT;
            y = y / gridHeight * VERTICAL_BLOCK_COUNT;

            x = Mathf.Floor(x);
            y = Mathf.Floor(y);

            selectedBlocksPosition = new Vector2(x, y);
        }

        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                Gizmos.color = DEFAULT_CELLS_EMPTY_GRID_COLOR;

                if (!McToggleTypeOfBlock.BlocksSelected)
                {
                    Vector2 coord = new Vector2(
                        Mathf.Round(mousePosition.x / MapBlock.BLOCK_SCALE) * MapBlock.BLOCK_SCALE,
                        Mathf.Round(mousePosition.y / MapBlock.BLOCK_SCALE) * MapBlock.BLOCK_SCALE
                    );

                    if (controller != null && Vector2.Distance(coord, new Vector2(col * MapBlock.BLOCK_SCALE, row * MapBlock.BLOCK_SCALE)) <= controller.BrushSize * MapBlock.BLOCK_SCALE)
                        Gizmos.color = SELECTEDT_GRID_COLOR;
                }
                else
                {
                    if (
                        (row * MapBlock.BLOCK_SCALE >= selectedBlocksPosition.y * MapBlock.WORLD_BLOCK_SIZE  &&
                        row * MapBlock.BLOCK_SCALE < selectedBlocksPosition.y * MapBlock.WORLD_BLOCK_SIZE + McToggleTypeOfBlock.Height * MapBlock.WORLD_BLOCK_SIZE ) 
                        &&
                        (col * MapBlock.BLOCK_SCALE >= selectedBlocksPosition.x * MapBlock.WORLD_BLOCK_SIZE  &&
                        col * MapBlock.BLOCK_SCALE < selectedBlocksPosition.x * MapBlock.WORLD_BLOCK_SIZE + McToggleTypeOfBlock.Width * MapBlock.WORLD_BLOCK_SIZE )
                    ) Gizmos.color = BLOCK_GRID_COLOR;
                }

                if (!Grid[row][col])
                    Gizmos.DrawWireCube(new Vector2(col * MapBlock.BLOCK_SCALE, row * MapBlock.BLOCK_SCALE), Vector2.one * MapBlock.BLOCK_SCALE);
                else
                    Gizmos.DrawCube(new Vector2(col * MapBlock.BLOCK_SCALE, row * MapBlock.BLOCK_SCALE), Vector2.one * MapBlock.BLOCK_SCALE);
            }
        }
    }

    void DrawInputedBlock(Vector2 mousePosition)
    {
        if (hasInputedBlock)
        {
            float x = Mathf.Clamp(mousePosition.x / MapBlock.BLOCK_SCALE, 0, gridWidth - MapBlock.BLOCK_SIZE);
            float y = Mathf.Clamp(mousePosition.y / MapBlock.BLOCK_SCALE, 0, gridHeight - MapBlock.BLOCK_SIZE);

            x = x / gridWidth * HORISONTAL_BLOCK_COUNT;
            y = y / gridHeight * VERTICAL_BLOCK_COUNT;

            x = Mathf.Floor(x);
            y = Mathf.Floor(y);

            inputedMapBlockPosition = new Vector2(x, y);

            for (int row = 0; row < MapBlock.BLOCK_SIZE; row++)
            {
                for (int col = 0; col < MapBlock.BLOCK_SIZE; col++)
                {
                    if (!inputedMapBlock.Grid[row][col])
                    {
                        Gizmos.color = DEFAULT_CELLS_EMPTY_INPUTED_GRID_COLOR;
                        Gizmos.DrawWireCube(
                            new Vector2(
                                col * MapBlock.BLOCK_SCALE + inputedMapBlockPosition.x * MapBlock.WORLD_BLOCK_SIZE ,
                                row * MapBlock.BLOCK_SCALE + inputedMapBlockPosition.y * MapBlock.WORLD_BLOCK_SIZE 
                            ), 
                            Vector2.one * MapBlock.BLOCK_SCALE
                        );
                    }
                    else
                    {
                        Gizmos.color = DEFAULT_CELLS_FILL_INPUTED_GRID_COLOR;
                        Gizmos.DrawCube(
                            new Vector2(
                                col * MapBlock.BLOCK_SCALE + inputedMapBlockPosition.x * MapBlock.WORLD_BLOCK_SIZE ,
                                row * MapBlock.BLOCK_SCALE + inputedMapBlockPosition.y * MapBlock.WORLD_BLOCK_SIZE 
                            ), 
                            Vector2.one * MapBlock.BLOCK_SCALE
                        );
                    }
                }
            }
        }
    }

    public void InputMapBlock()
    {
        for (int row = 0; row < MapBlock.BLOCK_SIZE; row++)
            Array.Copy(
                inputedMapBlock.Grid[row],
                0,
                Grid[row + (int)inputedMapBlockPosition.y * MapBlock.BLOCK_SIZE],
                (int)inputedMapBlockPosition.x * MapBlock.BLOCK_SIZE,
                MapBlock.BLOCK_SIZE
            );
        ReseteInputMapBlock();
    }

    public void ReseteInputMapBlock()
    {
        hasInputedBlock = false;
    }

    public MapBlock GetSelectedBlocks()
    { 
        MapBlock mapBlock = new MapBlock();        

        mapBlock.Grid = new bool[MapBlock.BLOCK_SIZE][];
        for (int row = 0; row < MapBlock.BLOCK_SIZE; row++)
        {
            mapBlock.Grid[row] = new bool[MapBlock.BLOCK_SIZE];
            Array.Copy(
                Grid[row + (int)selectedBlocksPosition.y * MapBlock.BLOCK_SIZE],
                (int)selectedBlocksPosition.x * MapBlock.BLOCK_SIZE,
                mapBlock.Grid[row],
                0,
                MapBlock.BLOCK_SIZE
            );
        }
        return mapBlock;
    }

    public void FillCell(Vector2 coordinates, float brushSize, bool fill)
    {
        Vector2 _coordinates =   new Vector2(
            Mathf.Round(coordinates.x / MapBlock.BLOCK_SCALE), 
            Mathf.Round(coordinates.y / MapBlock.BLOCK_SCALE)
        );
        
        if (_coordinates.x >= 0 && _coordinates.x <= gridWidth && _coordinates.y >= 0 && _coordinates.y <= gridHeight)
        {
            int minCol = (int)Mathf.Clamp((_coordinates.x - brushSize), 0f, gridWidth - 1);
            int maxCol = (int)Mathf.Clamp((_coordinates.x + brushSize), 0f, gridWidth - 1);

            int minRow = (int)Mathf.Clamp((_coordinates.y - brushSize), 0f, gridHeight - 1);
            int maxRow = (int)Mathf.Clamp((_coordinates.y + brushSize), 0f, gridHeight - 1);

            for (int row = minRow; row <= maxRow; row++)
            {
                for (int col = minCol; col <= maxCol; col++)
                {
                    float distance = Vector2.Distance(_coordinates, new Vector2(col, row));
                    if (distance <= brushSize)
                        Grid[row][col] = fill;
                }
            }
        }        
    }

    private void OnDrawGizmos()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));

        if (Grid != null)
            DrawGrid(mousePosition);

        Gizmos.color = BLOCK_GRID_COLOR;

        DrawBlockAreas();

        DrawInputedBlock(mousePosition);
    }
}
