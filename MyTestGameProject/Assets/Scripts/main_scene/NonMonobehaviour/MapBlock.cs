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
    public bool[][] Grid;
    public List<Direction> Entrance;
    [HideInInspector] public Vector2 WorldPosition;
    
    public bool HasExit;
    public Direction Exit;

    [Space]
    public Ground.GroundType type;


    public override string ToString()
    {
        Entrance.Sort();

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

        res += " [" + type.ToString() + "]";

        return res;
    }

    [Serializable] public enum Direction { BOTTOM, TOP, LEFT, RIGHT }

}
