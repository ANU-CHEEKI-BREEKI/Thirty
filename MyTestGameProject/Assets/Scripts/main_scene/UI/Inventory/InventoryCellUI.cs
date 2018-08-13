using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryCellUI : MonoBehaviour
{
    [SerializeField] Sprite newCellSprite;
    [SerializeField] Color newCellColor;
    [Space]
    [SerializeField] Sprite wornCellSprite;
    [SerializeField] Color wornCellColor;
    [Space]
    [SerializeField] Sprite damagedCellSprite;
    [SerializeField] Color damagedCellColor;

    Image image;
    Transform thisTransform;

    private void Awake()
    {
        image = GetComponent<Image>();
        thisTransform = transform;
    }

    public void Present()
    {
        image.sprite = newCellSprite;
        image.color = newCellColor;

        if (thisTransform.childCount > 0)
        {
            var drag = thisTransform.GetChild(thisTransform.childCount - 1).GetComponent<Drag>();
            if(drag != null)
            {
                var stack = drag.Stack;
                if(stack.Stats != null && stack.Stats is EquipmentStats)
                {
                    var stats = (EquipmentStats)stack.Stats;
                    switch (stats.ItemDurability)
                    {
                        case EquipmentStats.Durability.DAMAGED:
                            image.sprite = damagedCellSprite;
                            image.color = damagedCellColor;
                            break;
                        case EquipmentStats.Durability.WORN:
                            image.sprite = wornCellSprite;
                            image.color = wornCellColor;
                            break;
                    }
                }
            }
        }
    }
}
