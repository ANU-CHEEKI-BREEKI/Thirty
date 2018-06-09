using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MapBlock
{
    public const int WORLD_BLOCK_SIZE = 100;
    public const int BLOCK_SIZE = 20;
    public const int BLOCK_SCALE = 5;

    public static string ExitString = "EXIT ";

    /// <summary>
    /// if true - wall in this plase, cant walc
    /// if false - empty cell, can walc
    /// </summary> 
    [SerializeField] public bool[][] Grid;
    public List<Direction> Entrance;
    public Vector2 WorldPosition;

    public bool HasExit;
    public Direction Exit;

    public override string ToString()
    {
        string res = "";
        res += "[ ";
        if(Entrance != null)
            foreach (var item in Entrance)
                res += "(" + item + ") ";
        res += "]";

        if(HasExit)
        {
            res += " [" + ExitString + Exit + "]";
        }

        return res;
    }

    [Serializable] public enum Direction { BOTTOM, TOP, LEFT, RIGHT }
}
