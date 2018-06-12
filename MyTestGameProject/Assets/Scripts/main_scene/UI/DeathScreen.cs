using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DeathScreen : MonoBehaviour
{
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
        FadeScreen.FadeOn(cg, 2, this, () => { cg.blocksRaycasts = true; GameManager.Instance.Pause(); });
    }
}
