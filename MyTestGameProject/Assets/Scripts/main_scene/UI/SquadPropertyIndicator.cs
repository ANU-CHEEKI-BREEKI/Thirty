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

    public void Present(params object[] arr)
    {
        if (arr.Length > 0)
        {
            if(img != null)
                img.sprite = arr[0] as Sprite;

            if (arr.Length > 1)
                desc = (Description)arr[1];

            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }
}
