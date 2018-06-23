using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SquadInfoCellPresenter : MonoBehaviour, IPresenter
{
    CanvasGroup cg;
    Image img;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        img = transform.GetChild(0).GetComponent<Image>();
    }

    public void Present(params object[] param)
    {
        if(param != null && param.Length > 0 && param[0] is Sprite)
        {
            cg.alpha = 1;
            img.sprite = param[0] as Sprite;
        }
        else
        {
            cg.alpha = 0;
        }
    }
}
