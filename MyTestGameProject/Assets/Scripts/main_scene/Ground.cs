using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public enum GroundType { GRASSLAND }

    public const string PATH_TO_PREFABS = @"Prefabs\MapBlocks\";
    public const string PATH_TO_GRIDS = @"TextAssets\MapBlockPathMatrix\";

    static public Ground Instance { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] LayerMask directFindPathLayers;
    public LayerMask DirectFindPathLayers { get { return directFindPathLayers; } }

    [SerializeField] int rowCountOfBlocks = 5;
    public int RowCountOfBlocks { get { return rowCountOfBlocks; } }
    [SerializeField] int colCountOfBlocks = 6;
    public int ColCountOfBlocks { get { return colCountOfBlocks; } }

    [SerializeField] int seed = 1;
    [SerializeField] bool createRandom = false;
    [SerializeField] [Range(0.1f, 1.0f)] float rightWallsChanse = 0.5f;
    [SerializeField] [Range(0.1f, 1.0f)] float topWallsChanse = 0.5f;
    [SerializeField] bool willHaveLoops = true;

    [Space]
    [SerializeField] [Range(10, 90)] float exitOnColPercentOfMap = 50;
    [SerializeField] [Range(10, 90)] float exitOnRowPercentOfMap = 50;

    bool[][] rightWalls;
    bool[][] topWalls;

    public GroundBlock[][] MiniGrid { get; private set; }

    /// <summary>
    /// if true - wall in this plase, cant walc
    /// if false - empty cell, can walc
    /// </summary>    
    public bool[][] Grid { get; private set; }

    List<SameBlocksContainer> blockContainers;

    int indexType = -1;
    int indexSame = -1;

    System.Random rnd;

    int fullCountOfBlock;
    int loadedBlockCount;

    float progress;
    public float Progress { get { return progress; } }
    public bool WorkIsDone { get { return progress >= 1; } }

    public event Action OnWorkDone;
    public event Action OnGenerationDone;
    public event Action OnRecalcByCurrentBlocks;

    private void Awake()
    {
        Instance = this;

        if (createRandom)
            rnd = new System.Random();
        else
            rnd = new System.Random(seed.GetHashCode());
    }

    public void GeneradeMap(int rowCountOfBlocks, int colCountOfBlocks)
    {
        this.rowCountOfBlocks = rowCountOfBlocks;
        this.colCountOfBlocks = colCountOfBlocks;

        StartCoroutine(LoadMapBlocksCo(() =>
        {
            StartCoroutine(GeneradeMapCo());
        }));
    }

    IEnumerator LoadMapBlocksCo(Action actionOnLoaded)
    {
        progress = 0;

        blockContainers = new List<SameBlocksContainer>();
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>(PATH_TO_GRIDS);
        StringReader fstream;
        int cnt = textAssets.Length;

        for (int i = 0; i < cnt; i++)
        {
            //удаляем номер блока с названия
            string nm = textAssets[i].name.Remove(textAssets[i].name.LastIndexOf(" ("));

            //отсеиваем блоки не подходящие по типу месности
            if (nm.IndexOf(GameManager.Instance.CurrentLevel.GroundType.ToString()) != -1)
            {
                //удаляем тип месности из названия
                nm = nm.Remove(nm.LastIndexOf(" ["));

                //удаляем с имени обозначение выхода, т.к. сам блок содержит нужный флаг. а имя нужно только для пользователя
                if (nm.IndexOf(MapBlock.ExitString) != -1)
                    nm = nm.Remove(nm.LastIndexOf(" ["));

                //ищем контейнер с блоками, имеющими такие же направления проходов
                int index = blockContainers.FindIndex(t => t.Name.Equals(nm));
                if (index < 0)
                {
                    blockContainers.Add(new SameBlocksContainer() { Name = nm, SameBlocks = new List<SameBlocksContainer.Block>() });
                    index = blockContainers.Count - 1;
                }

                //десериализуем блок
                fstream = new StringReader(textAssets[i].text);
                MapBlock block = Tools.FileManagement.Deserialize(fstream);

                if (block.HasExit)
                {
                    //заносим блок по index'у...
                    if (blockContainers[index].ExitBlocks[(int)block.Exit] == null)
                        blockContainers[index].ExitBlocks[(int)block.Exit] = new List<SameBlocksContainer.Block>();

                    blockContainers[index].ExitBlocks[(int)block.Exit].Add(new SameBlocksContainer.Block() { Name = textAssets[i].name, block = block });
                }
                else
                {
                    //заносим блок по index'у...
                    blockContainers[index].SameBlocks.Add(new SameBlocksContainer.Block() { Name = textAssets[i].name, block = block });
                }
            }
            Resources.UnloadAsset(textAssets[i]);
            progress = (float)i / (cnt - 1);
            yield return null;
        }

        progress = 1;

        if (OnWorkDone != null)
        {
            OnWorkDone();
            OnWorkDone = null;
        }

        if (actionOnLoaded != null)
        {
            actionOnLoaded();
            actionOnLoaded = null;
        }
    }

    IEnumerator GeneradeMapCo()
    {
        progress = 0;

        // в игровом мире массивы распологаются снизу вверх, чтобы была привязка элементов
        // к коодринатам игрового мира. по этому, стены лабиринта будут верхние а не нижние
        int? seed;
        if (createRandom) seed = null;
        else seed = this.seed;

        Labirinth.CreateLabirinth(
            colCountOfBlocks,
            rowCountOfBlocks,
            out rightWalls,
            out topWalls,
            seed,
            rightWallsChanse,
            topWallsChanse,
            willHaveLoops
        );

        MiniGrid = new GroundBlock[rowCountOfBlocks][];
        for (int row = 0; row < rowCountOfBlocks; row++)
            MiniGrid[row] = new GroundBlock[colCountOfBlocks];

        Grid = new bool[rowCountOfBlocks * MapBlock.BLOCK_SIZE][];
        for (int row = 0; row < Grid.Length; row++)
            Grid[row] = new bool[colCountOfBlocks * MapBlock.BLOCK_SIZE];

        List<MapBlock.Direction> inclusionsDirection = new List<MapBlock.Direction>();
        List<MapBlock.Direction> exclusionsDirection = new List<MapBlock.Direction>();

        //положение входа и выхода в minigrid
        Vector2 entrance;
        Vector2 exit;


        fullCountOfBlock = rowCountOfBlocks * colCountOfBlocks;
        loadedBlockCount = 0;

        GetEntranceAndExit(out entrance, out exit);

        for (int row = 0; row < rowCountOfBlocks; row++)
        {
            for (int col = 0; col < colCountOfBlocks; col++)
            {
                inclusionsDirection.Clear();
                exclusionsDirection.Clear();

                if (col > 0 && !rightWalls[row][col - 1])
                    inclusionsDirection.Add(MapBlock.Direction.LEFT);
                else
                    exclusionsDirection.Add(MapBlock.Direction.LEFT);

                if (col < colCountOfBlocks - 1 && !rightWalls[row][col])
                    inclusionsDirection.Add(MapBlock.Direction.RIGHT);
                else
                    exclusionsDirection.Add(MapBlock.Direction.RIGHT);

                if (row > 0 && !topWalls[row - 1][col])
                    inclusionsDirection.Add(MapBlock.Direction.BOTTOM);
                else
                    exclusionsDirection.Add(MapBlock.Direction.BOTTOM);

                if (row < rowCountOfBlocks - 1 && !topWalls[row][col])
                    inclusionsDirection.Add(MapBlock.Direction.TOP);
                else
                    exclusionsDirection.Add(MapBlock.Direction.TOP);

                SameBlocksContainer.Block sameBlock;
                //обычный блок
                sameBlock = FindMapBlock(
                    inclusionsDirection,
                    exclusionsDirection
                );

                //если нужен блок начала или конца уровня то ищем заново, но уже зная направления выхода
                //блок для входа
                if (row == entrance.y && col == entrance.x)
                {
                    sameBlock = FindMapBlock(
                        inclusionsDirection,
                        exclusionsDirection,
                        true,
                        GetExitDirection(entrance, false)
                    );
                }
                //блок для выхода
                else if (row == exit.y && col == exit.x)
                {
                    sameBlock = FindMapBlock(
                        inclusionsDirection,
                        exclusionsDirection,
                        true,
                        GetExitDirection(exit, true)
                    );
                }

                sameBlock.block.WorldPosition = new Vector2(col * MapBlock.WORLD_BLOCK_SIZE, row * MapBlock.WORLD_BLOCK_SIZE);

                MiniGrid[row][col] = InstantiateMapBlockObject(sameBlock.block.WorldPosition, sameBlock.Name);
                MiniGrid[row][col].block = sameBlock.block;
                MiniGrid[row][col].posInMinigrid = new Vector2(col, row);

                InsertToGrid(sameBlock.block);

                loadedBlockCount++;
                progress = (float)loadedBlockCount / fullCountOfBlock;

                yield return null;
            }
        }

        progress = 1;

        if (OnWorkDone != null)
        {
            OnWorkDone();
            OnWorkDone = null;
        }
        
        if (OnGenerationDone != null)
            OnGenerationDone();
    }

    public void RecalcMatrixByCurrentBlocks()
    {
        StartCoroutine(RecalcMatrixByCurrentBlocksCo());
    }

    IEnumerator RecalcMatrixByCurrentBlocksCo()
    {
        progress = 0;

        Transform tr = transform;
        int cnt = tr.childCount;
        
        List<GroundBlock> gl = new List<GroundBlock>(cnt);
        int maxX = 0;
        int maxY = 0;

        for (int i = 0; i < cnt; i++)
        {
            var gb = tr.GetChild(i).GetComponent<GroundBlock>();
            gl.Add(gb);
            gb.LoadBlockStateFromFile();
            gb.block.WorldPosition = new Vector2(gb.posInMinigrid.x * MapBlock.WORLD_BLOCK_SIZE, gb.posInMinigrid.y * MapBlock.WORLD_BLOCK_SIZE);

            if (maxX < gb.posInMinigrid.x)
                maxX = (int)gb.posInMinigrid.x;

            if (maxY < gb.posInMinigrid.y)
                    maxY = (int)gb.posInMinigrid.y;

            progress = i / cnt / 2;
            yield return null;
        }

        maxX++;
        maxY++;

        MiniGrid = new GroundBlock[maxY][];
        for (int row = 0; row < maxY; row++)
            MiniGrid[row] = new GroundBlock[maxX];

        Grid = new bool[maxY * MapBlock.BLOCK_SIZE][];
        for (int row = 0; row < Grid.Length; row++)
            Grid[row] = new bool[maxX * MapBlock.BLOCK_SIZE];

        rowCountOfBlocks = maxY;
        colCountOfBlocks = maxX;
        
        for (int i = 0; i < cnt; i++)
        {
            var gb = gl[i];

            MiniGrid[(int)gb.posInMinigrid.y][(int)gb.posInMinigrid.x] = gb;
            InsertToGrid(gb.block);

            progress = i / cnt / 2 + 0.5f;
            yield return null;
        }

        progress = 1;

        if (OnWorkDone != null)
        {
            OnWorkDone();
            OnWorkDone = null;
        }

        if (OnRecalcByCurrentBlocks != null)
        {
            OnRecalcByCurrentBlocks();
            OnRecalcByCurrentBlocks = null;
        }        
    }

    void GetEntranceAndExit(out Vector2 entranceBlockPosition, out Vector2 exitBlockPosition)
    {
        entranceBlockPosition = Vector2.zero;

        do
        {
            int minCol = (int)(colCountOfBlocks * exitOnColPercentOfMap / 100f);
            int colCnt = colCountOfBlocks - minCol;

            int minRow = (int)(rowCountOfBlocks * exitOnRowPercentOfMap / 100f);
            int rowCnt = rowCountOfBlocks - minRow;


            int col = rnd.Next(colCnt) + minCol;
            col = col > colCountOfBlocks - 1 ? colCountOfBlocks - 1 : col;

            int row;
            if (col == colCountOfBlocks - 1 || col == 0)
            {
                row = rnd.Next(rowCnt) + minRow;
                row = row > rowCountOfBlocks - 1 ? rowCountOfBlocks - 1 : row;
            }
            else
            {
                row = rowCountOfBlocks - 1;
            }
            exitBlockPosition = new Vector2(col, row);
        }
        while (entranceBlockPosition == exitBlockPosition);
    }

    MapBlock.Direction GetExitDirection(Vector2 positionInMinigrid, bool isExit)
    {
        List<MapBlock.Direction> allowableDirections = new List<MapBlock.Direction>(2);

        if (positionInMinigrid.y == 0)
            allowableDirections.Add(MapBlock.Direction.BOTTOM);
        else if (positionInMinigrid.y == rowCountOfBlocks - 1)
            allowableDirections.Add(MapBlock.Direction.TOP);

        if (positionInMinigrid.x == 0)
            allowableDirections.Add(MapBlock.Direction.LEFT);
        else if (positionInMinigrid.x == colCountOfBlocks - 1)
            allowableDirections.Add(MapBlock.Direction.RIGHT);

        if (!isExit)
        {
            MapBlock.Direction d = MapBlock.Direction.BOTTOM;
            switch (GameManager.Instance.exitDirection)
            {
                case MapBlock.Direction.BOTTOM:
                    d = MapBlock.Direction.TOP;
                    break;
                case MapBlock.Direction.TOP:
                    d = MapBlock.Direction.BOTTOM;
                    break;
                case MapBlock.Direction.LEFT:
                    d = MapBlock.Direction.RIGHT;
                    break;
                case MapBlock.Direction.RIGHT:
                    d = MapBlock.Direction.LEFT;
                    break;
            }

            if (allowableDirections.Contains(d))
            {
                GameManager.Instance.entranceDirection = d;
                return d;
            }
        }

        int index = rnd.Next(allowableDirections.Count);

        if (isExit)
        {
            GameManager.Instance.exitDirection = allowableDirections[index];
            return GameManager.Instance.exitDirection;
        }
        else
        {
            GameManager.Instance.entranceDirection = allowableDirections[index];
            return GameManager.Instance.entranceDirection;
        }
    }

    private void InsertToGrid(MapBlock block)
    {
        int rowStart = (int)block.WorldPosition.y / MapBlock.BLOCK_SCALE;
        int rowEnd = rowStart + MapBlock.BLOCK_SIZE;

        for (int row = rowStart; row < rowEnd; row++)
            Array.Copy(block.Grid[row - rowStart], 0, Grid[row], (int)block.WorldPosition.x / MapBlock.BLOCK_SCALE, MapBlock.BLOCK_SIZE);
    }

    public SameBlocksContainer.Block FindMapBlock(
        List<MapBlock.Direction> inclusionsDirection,
        List<MapBlock.Direction> exclusionsDirection,
        bool wantedExitBlock = false,
        MapBlock.Direction exitDirection = MapBlock.Direction.BOTTOM
    )
    {
        SameBlocksContainer.Block block = null;

        if (wantedExitBlock)
            foreach (var item in inclusionsDirection)
                if (exitDirection == item)
                    throw new Exception("Направление выхода не может совпадать с направлением проходов.");

        //проходим по всему списку одинаковых по типу блоков
        for (int i = 0; i < blockContainers.Count; i++)
        {
            bool contin = false;

            //проверяем чтобы были все нужные направления
            foreach (var item in inclusionsDirection)
            {
                if (blockContainers[i].Name.IndexOf(item.ToString()) == -1)
                {
                    contin = true;
                    break;
                }
            }

            //если не нашли нужное направление то проверяем следуущий тип блоков
            if (contin) continue;

            //проверяем чтобы небыло ниодного ненужного направления
            foreach (var item in exclusionsDirection)
            {
                if (blockContainers[i].Name.IndexOf(item.ToString()) != -1)
                {
                    contin = true;
                    break;
                }
            }

            //если ненужное направление есть то проверяем следуущий тип блоков
            if (contin) continue;

            //индекс найденного типа
            indexType = i;

            //если нужен блок с выходом
            if (wantedExitBlock)
            {
                //случайнам образом берем индекс среди блоков одинакового типа
                try
                {
                    indexSame = rnd.Next(blockContainers[i].ExitBlocks[(int)exitDirection].Count);
                }
                catch (Exception ex)
                {
                    StringBuilder builder = new StringBuilder();
                    string space = " ";
                    foreach (var item in inclusionsDirection)
                        builder.Append(item.ToString() + space);

                    throw new Exception("Блока с проходами: " + builder.ToString() + " и выходом " + exitDirection + " не существует.");
                }
                block = blockContainers[i].ExitBlocks[(int)exitDirection][indexSame];
            }
            //если нужен обычный блок
            else
            {
                //случайнам образом берем индекс среди блоков одинакового типа
                indexSame = rnd.Next(blockContainers[i].SameBlocks.Count);
                block = blockContainers[i].SameBlocks[indexSame];
            }
            break;
        }

        return block;
    }

    GroundBlock InstantiateMapBlockObject(Vector2 position, string name)
    {
        //я провтыкал с pivot спрайтов, по этому сдигаем их вполовину вправо
        //так как pivot в центре


        //тааааак, кажется я всрал координаты тайлов при их расстановке. теперь надо ссдвинуть еще и на половину тайла. (то есть MapBlock.Scale * 1 / 2)
        position = new Vector2(
            position.x + MapBlock.WORLD_BLOCK_SIZE / 2,// - MapBlock.BLOCK_SCALE / 2,
            position.y + MapBlock.WORLD_BLOCK_SIZE / 2// - MapBlock.BLOCK_SCALE / 2
        );

        GameObject go = Resources.Load(PATH_TO_PREFABS + name) as GameObject;
        go.layer = gameObject.layer;

        return Instantiate(go, position, Quaternion.identity, gameObject.transform).GetComponent<GroundBlock>();
    }

    public bool CanWalk(int row, int col)
    {
        if (row > 0 && col > 0 && Grid.Length > 0 && row < Grid.Length && col < Grid[0].Length)
            return !Grid[row][col];
        else
            return false;
    }       

    private void OnDrawGizmos()
    {
        if (Grid != null)
        {
            for (int row = 0; row < Grid.Length; row++)
            {
                for (int col = 0; col < Grid[row].Length; col++)
                {
                    if (Grid[row][col])
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawWireCube(
                            new Vector2(
                                col * MapBlock.BLOCK_SCALE + MapBlock.BLOCK_SCALE / 2f,
                                row * MapBlock.BLOCK_SCALE + MapBlock.BLOCK_SCALE / 2f
                            ),
                            new Vector3(MapBlock.BLOCK_SCALE, MapBlock.BLOCK_SCALE) / 1.2f
                        );
                    }
                }
            }
        }
    }

    //private void OnGUI()
    //{
    //    if (Grid != null)
    //    {
    //        for (int row = 0; row < Grid.Length; row++)
    //        {
    //            for (int col = 0; col < Grid[row].Length; col++)
    //            {
    //                if (Grid[row][col])
    //                {
    //                    Gizmos.color = Color.black;
    //                    Vector2 pos = new Vector2(
    //                            col * MapBlock.BLOCK_SCALE,
    //                            row * MapBlock.BLOCK_SCALE + MapBlock.BLOCK_SCALE / 2f
    //                        );
    //                    Vector2 p = Camera.main.WorldToScreenPoint(pos);
    //                    GUI.Label(new Rect(p.x, -p.y + Screen.height, 300, 20), string.Format("x={0},y={1}", col, row));
    //                }
    //            }
    //        }
    //    }
    //}
}