using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class DeathScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] TextMeshProUGUI tipsText;
    [SerializeField] Image tipsImage1;
    [SerializeField] Image tipsImage2;
    [Header("Tips")]
    [SerializeField] SOTip[] tips;

    CanvasGroup cg;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 0;

        Squad.playerSquadInstance.OnSquadDestroy += PlayerSquadInstance_OnSquadDestroy;
    }

    private void OnDestroy()
    {
        if(Squad.playerSquadInstance != null)
        Squad.playerSquadInstance.OnSquadDestroy -= PlayerSquadInstance_OnSquadDestroy;
    }

    private void PlayerSquadInstance_OnSquadDestroy()
    {
        SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.MUSIC, 1f);
        SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.UI, 1f);
        SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.FX, 1f);

        SoundManager.Instance.PlaySound(
            new List<SoundChannel.ClipSet>(){
                new SoundChannel.ClipSet(SoundManager.Instance.DeathScreen, false),
                new SoundChannel.ClipSet(SoundManager.Instance.DeathScreenWind, true),
            },
            SoundManager.SoundType.MUSIC,
            100
        );

        SetTips();
        FadeScreen.FadeOn(cg, 2, this, () => { cg.blocksRaycasts = true; GameManager.Instance.Pause(); });
    }

    void SetTips()
    {
        SOTip tip = tips[UnityEngine.Random.Range(0, tips.Length)];

        tipsText.text = Localization.GetString(tip.tipName);

        tipsImage1.gameObject.SetActive(false);
        tipsImage2.gameObject.SetActive(false);
        if (tip.images.Length > 0)
        {
            tipsImage1.sprite = tip.images[0];
            tipsImage1.gameObject.SetActive(true);
            if (tip.images.Length == 2)
            {
                tipsImage2.sprite = tip.images[1];
                tipsImage2.gameObject.SetActive(true);
            }
        }
    }
}
