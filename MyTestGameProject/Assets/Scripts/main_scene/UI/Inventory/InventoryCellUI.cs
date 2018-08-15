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
    [Space]
    [SerializeField] Image thisImage;
    [SerializeField] Transform thisTransform;
    [SerializeField] GameObject thisGo;

    private void Awake()
    {
        if(thisImage == null)
            thisImage = GetComponent<Image>();
        if (thisTransform == null)
            thisTransform = transform;
        if (thisGo == null)
            thisGo = gameObject;
    }

    public void Present()
    {
        if (!thisGo.activeInHierarchy)
            return;

        thisImage.sprite = newCellSprite;
        thisImage.color = newCellColor;

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
                            thisImage.sprite = damagedCellSprite;
                            thisImage.color = damagedCellColor;
                            break;
                        case EquipmentStats.Durability.WORN:
                            thisImage.sprite = wornCellSprite;
                            thisImage.color = wornCellColor;
                            break;
                    }
                }
            }
        }
    }
}
