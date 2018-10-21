using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteInEditMode]
public class FlexibleGridLayout : MonoBehaviour
{
    [Serializable]
    public class FPadding
    {
        [Range(0, 1)] public  float fLeft = 1;
        [Range(0, 1)] public  float fRight = 1;
        [Range(0, 1)] public  float fTop = 1;
        [Range(0, 1)] public  float fBottom =1;
    }

    public bool rectangle = true;
    public bool sameSpace = true;
    public int colCount = 7;
    [Range(0, 1)] public float fCellWidth = 1;
    [Range(0, 1)] public float fCellHeight = 1;
    public FPadding fPadding;
    float fSpaceHorisontal;
    public float FSpaceHorisontal { get { return fSpaceHorisontal; } }
    float fSpaceVertical;
    public float FSpaceVertical { get { return fSpaceVertical; } }

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        ResetValues();
        SetSize();
    }

#if UNITY_EDITOR
    void Update()
    {
        ResetValues();
        SetSize();
    }
#endif

    void ResetValues()
    {
        var group = GetComponent<GridLayoutGroup>();
        var rect = (transform as RectTransform).rect;

        group.constraintCount = colCount;
        group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        int cellcount = group.constraintCount;
        if (cellcount < 1) cellcount = 1;
        float w = cellcount * fCellWidth;
        float padW = fPadding.fLeft + fPadding.fRight;

        float coef = 1 / (w + padW);
        if (coef < 1)
            fCellWidth *= coef;

        fSpaceHorisontal = 1 - (w + padW);
        if (cellcount > 1)
            fSpaceHorisontal /= cellcount - 1;
        else
            fSpaceHorisontal = 0;

        if (fSpaceHorisontal < 0)
            fSpaceHorisontal = 0;


        int rowCount = group.transform.childCount / colCount;
        if (group.transform.childCount % colCount != 0)
            rowCount += 1;

        float h = rowCount * fCellHeight;
        float padH = fPadding.fTop + fPadding.fBottom;

        float coefH = 1 / (h + padH);
        if (coef < 1)
            fCellHeight *= coef;
        fSpaceVertical = 1 - (h + padH);
        if (fSpaceVertical < 0)
            fSpaceVertical = 0;

        if (rowCount > 1)
            fSpaceVertical /= rowCount - 1;
        else
            fSpaceVertical = 0;
    }

    void SetSize()
    {
        var group = GetComponent<GridLayoutGroup>();
        var rect = (transform as RectTransform).rect;

        var cellSize = group.cellSize;
        cellSize.x = fCellWidth * rect.width;
        if (rectangle)
            cellSize.y = cellSize.x;
        else
            cellSize.y = fCellHeight * rect.height;
        group.cellSize = cellSize;

        var padding = group.padding;
        padding.top = (int)(fPadding.fTop * rect.height);
        padding.bottom = (int)(fPadding.fBottom * rect.height);
        padding.left = (int)(fPadding.fLeft * rect.width);
        padding.right = (int)(fPadding.fRight * rect.width);
        group.padding = padding;

        var space = group.spacing;
        space.x = fSpaceHorisontal * rect.width;
        if (sameSpace)
            space.y = space.x;
        else
            space.y = fSpaceVertical * rect.height;
        group.spacing = space;
    }
}
