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
    Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TipsPanel.Instance.Show(desc, transform.position);
    }

    public void Present(Sprite sprite, Description? d)
    {
        if (img != null && sprite != null)
            img.sprite = sprite;

        if (d != null)
            desc = d.Value;

        if (sprite == null && d == null)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
