using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTipsPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI tipsText;
    [SerializeField] Image tipsImage1;
    [SerializeField] Image tipsImage2;
    [Header("Tips")]
    [SerializeField] SOTip[] tips;

    private void Start()
    {
        SetTips();
    }

    void SetTips()
    {
        SOTip tip = tips[Random.Range(0, tips.Length)];

        if(tip.IsLocalisedText)
            tipsText.text = Localization.GetString(tip.TipText);
        else
            tipsText.text = tip.TipText;

        tipsImage1.gameObject.SetActive(false);
        tipsImage2.gameObject.SetActive(false);
        if (tip.Images.Length > 0)
        {
            tipsImage1.sprite = tip.Images[0];
            tipsImage1.gameObject.SetActive(true);
            if (tip.Images.Length == 2)
            {
                tipsImage2.sprite = tip.Images[1];
                tipsImage2.gameObject.SetActive(true);
            }
        }

        {
            var layout = GetComponent<LayoutGroup>();
            DestroyImmediate(layout);
        }

        if(tip.Direction == SOTip.Directions.HORISONTL)
        {
            var layout = gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 20;
            layout.padding.left = 100;
            layout.padding.right = 100;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;
        }
        else
        {
            var layout = gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 10;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;
        }


    }
}
