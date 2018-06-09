using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode] [RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutCellWidth : MonoBehaviour
{
    GridLayoutGroup layout;
    RectTransform thisTransform;
    LayoutGroup parentLayout;
    RectTransform parentTransform;

    void Start ()
    {
        layout = GetComponent<GridLayoutGroup>();
        thisTransform = (RectTransform)transform;

        parentTransform = (RectTransform)thisTransform.parent;
        parentLayout = parentTransform.GetComponent<LayoutGroup>();
    }

    private void Update()
    {
        
        if(layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        layout.cellSize = new Vector2(
            (parentTransform.rect.width 
                - layout.padding.left 
                - layout.padding.right 
                - layout.spacing.x 
                - parentLayout.padding.left 
                - parentLayout.padding.right
            ) / layout.constraintCount,
            layout.cellSize.y
        );
    }
}
