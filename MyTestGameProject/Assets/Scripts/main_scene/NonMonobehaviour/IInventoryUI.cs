using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AInventoryUI : MonoBehaviour
{
    abstract public void RefreshUI();

    /// <summary>
    /// Создаем контейнер итема в указаной ячейке, устанавливаем иконку и её цвет
    /// </summary>
    /// <param name="origin">оригинал итема в указаной ячейке</param>
    /// <param name="cell">ячейка</param>
    /// <param name="stack"></param>
    /// <param name="canDrag">можно ли еретаскивать этот контейнер</param>
    /// <returns>контейнер итема в указаной ячейке</returns>
    virtual public GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    {
        GameObject go = null;
        
        if (cell.childCount > 0)
            Destroy(cell.GetChild(cell.childCount - 1).gameObject);

        if (stack.MainProperties != null)
        {
            go = Instantiate(origin, cell);

            Image img = go.GetComponent<Image>();
            img.sprite = stack.MainProperties.Value.Icon;

            if (stack.MainProperties.Value.Currency == DSPlayerScore.Currency.GOLD)
                img.color = new Color(1, 1, 0);

            var drag = go.GetComponent<Drag>();
            drag.CanDrag = canDrag;
        }
        return go;
    }
}
