using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrainWalker
{
    /// <summary>
    /// Size of gameobject in %/100
    /// <para>1 - это нормальный размер (100%)</para>
    /// </summary>
    float Scale { get; set; }

    Vector2 Position { get; }
}
