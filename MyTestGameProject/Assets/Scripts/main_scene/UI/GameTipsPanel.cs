﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTipsPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI tipsText;
    [SerializeField] RectTransform tipsImagePlace;
    [Header("Tips")]
    [SerializeField] SOTip[] tips;

    [SerializeField] bool setTipOnStart = true;

    private void Start()
    {
        if (setTipOnStart)
        {
            SOTip tip = tips[Random.Range(0, tips.Length)];
            SetTip(tip);
        }
    }

    [ContextMenu("SetTips")]
    public void SetTip(SOTip tip)
    {
        ResetImagePasePanel();

        if (tip.IsLocalisedText)
            tipsText.text = Localization.GetString(tip.TipText);
        else
            tipsText.text = tip.TipText;
        
        //
        if (tip.AnimatedImageOriginal == null)
        {
            SetImage(tip.FirstImage, tip.Direction);
            SetImage(tip.SecondImage, tip.Direction);
            LayoutRebuilder.MarkLayoutForRebuild(tipsImagePlace);
        }
        else
        {
            SetAnimatedImage(tip.AnimatedImageOriginal);
        }

        if (GetComponent<LayoutGroup>() != null)
            ResetTipPanelLayout(true);

        if(tip.Direction == SOTip.Directions.HORISONTAL)
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

            tipsText.alignment = TextAlignmentOptions.Midline;
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

            tipsText.alignment = TextAlignmentOptions.Top;
        }
    }

    public void ResetTipPanelLayout(bool immediate = false)
    {
        var layout = GetComponent<LayoutGroup>();

        if (immediate)
            DestroyImmediate(layout);
        else
            Destroy(layout);
    }

    void SetImage(Sprite image, SOTip.Directions dir, bool preserveAspect = true)
    {
        if (image != null)
        {
            var go = new GameObject();
            go.transform.SetParent(tipsImagePlace, false);
            var img = go.AddComponent<Image>();
            img.sprite = image;
            img.preserveAspect = preserveAspect;

            if (dir == SOTip.Directions.HORISONTAL)
            {
                var el = go.AddComponent<LayoutElement>();
                el.preferredWidth = 200;
                var fit = go.AddComponent<ContentSizeFitter>();
                fit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }
    }

    void SetAnimatedImage(GameObject origin)
    {
        Instantiate(origin, tipsImagePlace);
    }

    void ResetImagePasePanel()
    {
        int cnt = tipsImagePlace.childCount;
        if (cnt > 0)
            for (int i = 0; i < cnt; i++)
                Destroy(tipsImagePlace.GetChild(i).gameObject);
    }
}
