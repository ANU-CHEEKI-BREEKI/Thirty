using System.Collections.Generic;
using UnityEngine;

public static class Labirinth
{
    
    static System.Random rnd;
    //размеры последней созданной вспомагаельной матрицы. чтоб не пересоздавать матрицу
    static int labirintHeight = 0;
    //размеры последней созданной вспомагаельной матрицы. чтоб не пересоздавать матрицу
    static int labirintWindth = 0;

    //вспомагательная матрица для поиска пути (в ней прописываются длины путей)
    static float[][] pathMatrix;
    //хз
    static List<Vector2> oldLabelCell;
    //хз
    static List<Vector2> newLabelCell;
    //хз
    static List<Vector2> tempList;

    /// <summary>
    /// Создание лабиринта по алгоритму Еллера
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="rightWalls"></param>
    /// <param name="bottomWalls"></param>
    /// <param name="seed"></param>
    /// <param name="rightWallsChanse"></param>
    /// <param name="bottomWallsChanse"></param>
    /// <param name="willHaveLoops"></param>
    public static void CreateLabirinth(
        int width, int height, out bool[][] rightWalls, out bool[][] bottomWalls, 
        int? seed, float rightWallsChanse, float bottomWallsChanse, bool willHaveLoops)
    {
        if (seed != null)
            rnd = new System.Random((int)seed);
        else
            rnd = new System.Random();

        rightWalls = new bool[height][];
        for (int row = 0; row < height; row++)
            rightWalls[row] = new bool[width - 1];

        bottomWalls = new bool[height - 1][];
        for (int row = 0; row < height - 1; row++)
            bottomWalls[row] = new bool[width];

        int?[,] sets = new int?[height, width];

        for (int row = 0; row < height; row++)
        {
            // step - 2 Join any cells not members of a set to their own unique set

            //generade list of possible sets names
            List<int> setsNams = new List<int>(width);
            for (int col = 0; col < width; col++)
                setsNams.Add(col);
            
            //delete all names what already uses
            for (int col = 0; col < width; col++)
                setsNams.RemoveAll(p => p == sets[row, col]);

            //join cells without set to unique set names
            for (int col = 0; col < width; col++)
            {
                if (sets[row, col] == null)
                {
                    sets[row, col] = setsNams[0];
                    setsNams.RemoveAt(0);
                }
            }

            // step 3 - Create right-walls, mo from left to right
            for (int col = 0; col < width - 1; col++)
            {
                if (rnd.NextDouble() <= rightWallsChanse)
                {
                    rightWalls[row][col] = true;
                } else
                {
                    if (!willHaveLoops && sets[row, col + 1] == sets[row, col])
                        rightWalls[row][col] = true;
                    else
                        sets[row, col + 1] = sets[row, col];
                }
            }

            if (row < height - 1)
            {
                // step 4 - Create bottom-walls, moving from left to right
                int wallCount = 0;
                int cellCount = 0;
                int? cellSet = sets[row, 0];
                for (int col = 0; col < width; col++)
                {
                    if (sets[row, col] != cellSet)
                    {
                        if (cellCount == wallCount)
                        {
                            int r = rnd.Next(cellCount);
                            bottomWalls[row][col - r - 1] = false;
                        }
                        cellSet = sets[row, col];
                        cellCount = 0;
                        wallCount = 0;
                    }

                    cellCount++;
                    if (rnd.NextDouble() <= bottomWallsChanse)
                    {
                        bottomWalls[row][col] = true;
                        wallCount++;
                    }

                    if (col == width - 1)
                    {
                        if (cellCount == wallCount)
                        {
                            int r = rnd.Next(cellCount);
                            bottomWalls[row][col - r] = false;
                        }
                        cellSet = sets[row, col];
                        cellCount = 0;
                        wallCount = 0;
                    }

                }

                // step 5 - Decide to keep adding rows, or stop and complete the maze
              
                // new row
                for (int col = 0; col < width; col++)
                {
                    //next line right and bottom walls already deleted by default
                    if (bottomWalls[row][col])
                        sets[row + 1, col] = null;
                    else
                        sets[row + 1, col] = sets[row, col];
                }
            }

            if (row == height - 1)
            {
                // end creating
                for (int col = 0; col < width - 1; col++)
                {
                    if (sets[row, col] != sets[row, col + 1])
                    {
                        rightWalls[row][col] = false;                        

                        int? t = sets[row, col + 1];
                        while (col < width - 1 && sets[row, col + 1] == t)
                        {
                            sets[row, col + 1] = sets[row, col];
                            col++;
                        }
                        col--;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Поиск пути в лабиринте из ячейки start к ячейке finish. Алгоритм поиска - волновой (Алгоритм Ли)
    /// </summary>
    /// <param name="labirinth">Матрица проходимости, где true - если ячейка занята и пройти нельзя, false - ячейка пустая и пройти можно</param>
    /// <param name="start">Интексы ячейки с которой начинается путь. x - столбец, y - строка</param>
    /// <param name="finish">Индексы ячейки к торорой нужно найти путь. x - столбец, y - строка</param>
    /// <param name="scale">Масштаб, к которому нужно привести полученный координаты</param>
    /// <returns>один из искомых путей. null - если пути нет</returns>
    public static List<Vector3> FindPathLee(bool[][] labirinth, Vector2 start, Vector2 finish, int scale)
    {
        start = new Vector2((int)start.x, (int)start.y);
        finish = new Vector2((int)finish.x, (int)finish.y);

        if (labirinth[(int)start.y][(int)start.x] || labirinth[(int)finish.y][(int)finish.x])
            return null;

        List<Vector3> resultPath = new List<Vector3>();

        CreateLabirinthPathMatrix(labirinth);

        oldLabelCell.Add(start);

        pathMatrix[(int)start.y][(int)start.x] = 0;

        //распространяем волну
        do
        {
            foreach (var item in oldLabelCell)
            {
                int row = (int)item.y;
                int col = (int)item.x;

                if (row > 0)
                {
                    if (col > 0)
                        if (pathMatrix[row - 1][col - 1] == -1  && !labirinth[row - 1][col - 1] && !(new Vector2(col - 1, row - 1).Equals(start)) && !PathLeeExistNearWall(labirinth, new Vector2(col - 1, row - 1)))
                        {
                            pathMatrix[row - 1][col - 1] = pathMatrix[row][col] + 1;
                            newLabelCell.Add(new Vector2(col - 1, row - 1));
                        }

                    if (col < labirinth[row].Length - 1)
                        if (pathMatrix[row - 1][col + 1] == -1 && !labirinth[row - 1][col + 1] && !(new Vector2(col + 1, row - 1).Equals(start)) && !PathLeeExistNearWall(labirinth, new Vector2(col + 1, row - 1)))
                        {
                            pathMatrix[row - 1][col + 1] = pathMatrix[row][col] + 1;
                            newLabelCell.Add(new Vector2(col + 1, row - 1));
                        }

                    if (pathMatrix[row - 1][col] == -1 && !labirinth[row - 1][col] && !(new Vector2(col, row -1).Equals(start)))
                    {
                        pathMatrix[row - 1][col] = pathMatrix[row][col] + 1;
                        newLabelCell.Add(new Vector2(col, row - 1));
                    }
                }

                if (row < labirinth.Length - 1)
                {
                    if (col > 0)
                        if (pathMatrix[row + 1][col - 1] == -1 && !labirinth[row + 1][col - 1] && !(new Vector2(col - 1, row + 1).Equals(start)) && !PathLeeExistNearWall(labirinth, new Vector2(col - 1, row + 1)))
                        {
                            pathMatrix[row + 1][col - 1] = pathMatrix[row][col] + 1;
                            newLabelCell.Add(new Vector2(col - 1, row + 1));
                        }

                    if (col < labirinth[row].Length - 1)
                        if (pathMatrix[row + 1][col + 1] == -1 && !labirinth[row + 1][col + 1] && !(new Vector2(col + 1, row + 1).Equals(start)) && !PathLeeExistNearWall(labirinth, new Vector2(col + 1, row + 1)))
                        {
                            pathMatrix[row + 1][col + 1] = pathMatrix[row][col] + 1;
                            newLabelCell.Add(new Vector2(col + 1, row + 1));
                        }

                    if (pathMatrix[row + 1][col] == -1 && !labirinth[row + 1][col] && !(new Vector2(col, row + 1).Equals(start)))
                    {
                        pathMatrix[row + 1][col] = pathMatrix[row][col] + 1;
                        newLabelCell.Add(new Vector2(col, row + 1));
                    }
                }

                if (col > 0)
                    if (pathMatrix[row][col - 1] == -1 && !labirinth[row][col - 1] && !(new Vector2(col - 1, row).Equals(start)))
                    {
                        pathMatrix[row][col - 1] = pathMatrix[row][col] + 1;
                        newLabelCell.Add(new Vector2(col - 1, row));
                    }

                if (col < labirinth[row].Length - 1)
                    if (pathMatrix[row][col + 1] == -1 && !labirinth[row][col + 1] && !(new Vector2(col + 1, row).Equals(start)))
                    {
                        pathMatrix[row][col + 1] = pathMatrix[row][col] + 1;
                        newLabelCell.Add(new Vector2(col + 1, row));
                    }
            }

            tempList = oldLabelCell;
            oldLabelCell = newLabelCell;
            newLabelCell = tempList;
            newLabelCell.Clear();

        } while (pathMatrix[(int)finish.y][(int)finish.x] == -1 && oldLabelCell.Count > 0);

        if (pathMatrix[(int)finish.y][(int)finish.x] == -1)
            return null;

        //востанавливаем путь
        Vector2 cell = finish;
        Vector2 res;
        resultPath.Add(cell * scale);

        while (!cell.Equals(start))
        {
            int row = (int)cell.y;
            int col = (int)cell.x;

            if (col > 0)
                if (pathMatrix[row][col - 1] == pathMatrix[row][col] - 1 && !labirinth[row][col - 1])
                {
                    res = new Vector2(col - 1, row);
                    resultPath.Add(res * scale);
                    cell = res;
                    continue;
                }

            if (col < labirinth[row].Length - 1)
                if (pathMatrix[row][col + 1] == pathMatrix[row][col] - 1 && !labirinth[row][col + 1])
                {
                    res = new Vector2(col + 1, row);
                    resultPath.Add(res * scale);
                    cell = res;
                    continue;
                }

            if (row > 0)
            {
                if (pathMatrix[row - 1][col] == pathMatrix[row][col] - 1 && !labirinth[row - 1][col])
                {
                    res = new Vector2(col, row - 1);
                    resultPath.Add(res * scale);
                    cell = res;
                    continue;
                }

                if (col > 0)
                    if (pathMatrix[row - 1][col - 1] == pathMatrix[row][col] - 1 && !labirinth[row - 1][col - 1])
                    {
                        res = new Vector2(col - 1, row - 1);
                        resultPath.Add(res * scale);
                        cell = res;
                        continue;
                    }

                if (col < labirinth[row].Length - 1)
                    if (pathMatrix[row - 1][col + 1] == pathMatrix[row][col] - 1 && !labirinth[row - 1][col + 1])
                    {
                        res = new Vector2(col + 1, row - 1);
                        resultPath.Add(res * scale);
                        cell = res;
                        continue;
                    }

                
            }

            if (row < labirinth.Length - 1)
            {
                if (pathMatrix[row + 1][col] == pathMatrix[row][col] - 1 && !labirinth[row + 1][col])
                {
                    res = new Vector2(col, row + 1);
                    resultPath.Add(res * scale);
                    cell = res;
                    continue;
                }

                if (col > 0)
                    if (pathMatrix[row + 1][col - 1] == pathMatrix[row][col] - 1 && !labirinth[row + 1][col - 1])
                    {
                        res = new Vector2(col - 1, row + 1);
                        resultPath.Add(res * scale);
                        cell = res;
                        continue;
                    }

                if (col < labirinth[row].Length - 1)
                    if (pathMatrix[row + 1][col + 1] == pathMatrix[row][col] - 1 && !labirinth[row + 1][col + 1])
                    {
                        res = new Vector2(col + 1, row + 1);
                        resultPath.Add(res * scale);
                        cell = res;
                        continue;
                    }
            }
        }

        Vector3 t;
        for (int i = 0; i < resultPath.Count / 2; i++)
        {
            t = resultPath[i];
            resultPath[i] = resultPath[resultPath.Count - 1 - i];
            resultPath[resultPath.Count - 1 - i] = t;
        }

        return resultPath;
    }

    static void CreateLabirinthPathMatrix(bool[][] labirinth)
    {
        bool h = false, l = false;

        if (labirinth.Length != labirintHeight)
        {
            pathMatrix = new float[labirinth.Length][];
            l = true;
        }

        for (int row = 0; row < pathMatrix.Length; row++)
        {
            if (labirinth[row].Length != labirintWindth)
            {
                pathMatrix[row] = new float[labirinth[row].Length];
                h = true;
            }

            for (int col = 0; col < pathMatrix[row].Length; col++)
            {
                pathMatrix[row][col] = -1;
            }
        }

        if (l || h)
        {
            oldLabelCell = new List<Vector2>(labirintHeight * labirintWindth);
            newLabelCell = new List<Vector2>(labirintHeight * labirintWindth);
            tempList = new List<Vector2>(labirintHeight * labirintWindth);
        }
        else
        {
            oldLabelCell.Clear();
            newLabelCell.Clear();
            tempList.Clear();
        }

        //это было дописано без полного понимания работы алгоритма через год после создания остального кода этого класса
        labirintHeight = labirinth.Length;
        labirintWindth = labirinth[0].Length;

    }

    /// <summary>
    /// Проверка есть ли возле передаваемой ячейки стена.
    /// </summary>
    /// <param name="labirinth">Матрица проходимости, где true - если ячейка занята и пройти нельзя, false - ячейка пустая и пройти можно</param>
    /// <param name="target">Интексы проверяемой ячейки. x - столбец, y - строка</param>
    /// <returns></returns>
    static bool PathLeeExistNearWall(bool[][] labirinth, Vector2 target)
    {
        bool result = false;

        int row = (int)target.y;
        int col = (int)target.x;

        if(row > 0)
        {
            if (col > 0)
                if (labirinth[row - 1][col - 1])
                    result = true;

            if(col < labirinth[row].Length - 1)
                if (labirinth[row - 1][col + 1])
                    result = true;

            if (labirinth[row - 1][col])
                result = true;
        }

        if (row < labirinth.Length - 1)
        {
            if (col > 0)
                if (labirinth[row + 1][col - 1])
                    result = true;

            if (col < labirinth[row].Length - 1)
                if (labirinth[row + 1][col + 1])
                    result = true;

            if (labirinth[row + 1][col])
                result = true;
        }

        if (col > 0)
            if (labirinth[row][col - 1])
                result = true;

        if (col < labirinth[row].Length - 1)
            if (labirinth[row][col + 1])
                result = true;

        if (labirinth[row][col])
            result = true;

        return result;
    }
}
