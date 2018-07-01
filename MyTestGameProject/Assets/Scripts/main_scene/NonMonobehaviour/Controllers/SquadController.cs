using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Контроллер перемещения отряда. Ищет путь и задает его. И т.п.
/// </summary>
public class SquadController
{
    Squad squad;

    Vector2 startFindPath;
    Vector2 endFindPath;
    Quaternion lookRotation;
    List<Vector3> path;
    Vector2 movePos;
    Vector2 squadStartPos;
    public SquadController(Squad squad)
    {
        this.squad = squad;
    }

    Coroutine pathFinding = null;
    //---------------------------------------------------

    /// <summary>
    /// Приказать отряду перемещяться в заданную точку
    /// </summary>
    /// <param name="worldPositionToMove">конечная точка перемещения</param>
    /// <param name="optimazePathByPhysicsCast">нужно ли оптимизировать путь <para>оптимизация заключается в рейкасте и исключении лишних точек пути</para></param>
    public void MoveToPoint(Vector2 worldPositionToMove, bool optimazePathByPhysicsCast = true)
    {
        if (pathFinding != null)
        {
            Labirinth.StopFinding(pathFinding);
            pathFinding = null;
        }

        movePos = worldPositionToMove;

        squadStartPos = GetStartPosition();

        startFindPath = new Vector2(
            Mathf.Round(squadStartPos.x / MapBlock.BLOCK_SCALE),
            Mathf.Round(squadStartPos.y / MapBlock.BLOCK_SCALE)
        );
        endFindPath = new Vector2(
            Mathf.Round((movePos.x - MapBlock.BLOCK_SCALE / 2f) / MapBlock.BLOCK_SCALE),
            Mathf.Round((movePos.y - MapBlock.BLOCK_SCALE / 2f) / MapBlock.BLOCK_SCALE)
        );

        if (Ground.Instance.CanWalk((int)endFindPath.y, (int)endFindPath.x))
        {
            SetStartPosition(squadStartPos);

            //var rhit = Physics2D.CircleCast(
            //    origin: squadStartPos,
            //    radius: squad.SQUAD_LENGTH / 4,
            //    direction: movePos - squadStartPos,
            //    distance: Vector2.Distance(movePos, squadStartPos),
            //    layerMask: Ground.Instance.DirectFindPathLayers.value
            //);

            var rhit = Physics2D.Linecast(
                start: squadStartPos,
                end: movePos,
                layerMask: Ground.Instance.DirectFindPathLayers.value
            );

            if (rhit.collider == null)
            {
                path = new List<Vector3>();
                path.Add(squadStartPos);
                path.Add(movePos);
                SetPath(movePos, optimazePathByPhysicsCast);
            }
            else
            {
                Labirinth.OnWorkDone += (obj) =>
                {
                    path = obj as List<Vector3>;
                    if (path.Count == 0)
                        path = null;
                    else
                    {
                        Labirinth.MoveFromWall(Ground.Instance.Grid, path);
                        Labirinth.ScalePath(path, MapBlock.BLOCK_SCALE, true);
                        SetPath(movePos, optimazePathByPhysicsCast);
                    }
                    pathFinding = null;
                };
                pathFinding = Labirinth.FindPathLee(Ground.Instance.Grid, startFindPath, endFindPath);
            }
        }
    }
    
    /// <summary>
    /// Указывает точку, в которую должен смотреть отряд по окончанию пути перемещения
    /// </summary>
    /// <param name="worldPositionToLook"></param>
    public void RotateAfterMoving(Quaternion rotation)
    {
        squad.EndLookRotation = rotation;
    }

    void SetStartPosition(Vector2 newPos)
    {
        squad.PositionsTransform.position = newPos;
    }

    Vector2 GetStartPosition()
    {
        var res = squad.PositionsTransform.position;
        //
        var angle = Quaternion.LookRotation(Vector3.forward, (Vector3)movePos - squad.PositionsTransform.position).eulerAngles.z;

        float angleLook = squad.PositionsTransform.rotation.eulerAngles.z;
        if (squad.FlipRotation) angleLook += 180;

        var sqrt = Mathf.Sqrt(0.5f);

        var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        var sin2 = Mathf.Sin(angleLook * Mathf.Deg2Rad);

        var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        var cos2 = Mathf.Cos(angleLook * Mathf.Deg2Rad);

        if ((Mathf.Abs(cos2) >= sqrt && cos * cos2 < 0) || (Mathf.Abs(sin2) >= sqrt && sin * sin2 < 0))
        {
            RaycastHit2D rhit = Physics2D.CircleCast(
                squad.CenterSquad,
                0.1f,
                Vector2.zero,
                0,
                Ground.Instance.DirectFindPathLayers
            );

            if (!rhit)
                res = squad.CenterSquad;
        }

        return res;
    }
    
    void SetPath(Vector2 movePos, bool optimaze)
    {
        if (optimaze)
            path = OptimazePath(path);

        squad.Path = path;
        squad.EndMovePosition = movePos;
    }

    /// <summary>
    /// бросает каст с первой точки пока не пересечется со стенами. от последней точки продолжается каст дальше.
    /// в итоге - быстро, но много участком где максимально длинные прямые линии и сразу же короткие изгибы вокруг поворотов
    /// </summary>
    /// <param name="oldPath"></param>
    /// <returns></returns>
    List<Vector3> OptimazePath(List<Vector3> oldPath)
    {
        List<Vector3> newPath = new List<Vector3>();
        newPath.Add(oldPath[0]);

        int current = 0;
        for (int i = 1; i < oldPath.Count - 1; i++)
        {
            var hit = Physics2D.CircleCast(
                origin: oldPath[current],
                radius: squad.SQUAD_LENGTH / 2,
                direction: oldPath[i] - oldPath[current],
                distance: Vector2.Distance(oldPath[current], oldPath[i]),
                layerMask: Ground.Instance.DirectFindPathLayers.value
            );

            if (hit.collider != null)
            {
                current = i - 1;
                newPath.Add(oldPath[current]);
            }
        }

        newPath.Add(oldPath[oldPath.Count - 1]);

        return newPath;
    }
}
