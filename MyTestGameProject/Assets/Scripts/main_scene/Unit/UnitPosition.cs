using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPosition : MonoBehaviour
{
    public Transform ThisTrnsform { get; private set; }

    /// <summary>
    /// Начиная с нуля. Так как индексы в массивах начитнаются с нуля.
    /// <para>x - столбец</para>
    /// <para>y - строка</para>
    /// </summary>
    public Vector2 PositionInArray { get; set; }
    public float Scale { get; set; }
    UnitAnimationController animController;
    Unit unit;
    public Unit Unit
    {
        get
        {
            return unit;
        }
        set
        {
            unit = value;
            animController = unit.GetComponent<UnitAnimationController>();
        }
    }
    public Squad Squad { get; set; }
    /// <summary>
    /// Начиная с единицы (1,2,3,...)
    /// </summary>
    public int RowInPhalanx
    {
        get
        {
            return rowInPhalanx;
        }
        set
        {
            rowInPhalanx = value;
            animController.SortingOrger = -value;
        }
    }
    int rowInPhalanx;



    private void Start()
    {
        ThisTrnsform = transform;
    }
}
