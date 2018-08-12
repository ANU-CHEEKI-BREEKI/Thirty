using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Description;

public class SquadPropertyIndicator : MonoBehaviour, IPointerClickHandler
{
    Description desc;
    [SerializeField] Image img;
    [SerializeField] Image background;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        TipsPanel.Instance.Show(desc, transform.position);
    }

    public void Present(Sprite sprite, Description? d, bool? isPositive = null)
    {
        if (img != null && sprite != null)
            img.sprite = sprite;

        if (d != null)
            desc = d.Value;

        if (sprite == null && d == null)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);

        if (background != null)
        {
            if (isPositive != null)
            {
                if (isPositive.Value)
                    background.color = Color.green;
                else
                    background.color = Color.red;
            }
            else
                background.color = Color.black;
        }
    }
}
