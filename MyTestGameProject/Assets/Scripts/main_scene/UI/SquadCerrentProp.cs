using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadCerrentProp : MonoBehaviour
{
    [SerializeField] CanvasGroup inFight;
    [SerializeField] CanvasGroup inRanks;
    [SerializeField] CanvasGroup inPhalanx;
    [SerializeField] CanvasGroup inShields;

    Squad squad;

    void Start ()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

        HideCG(inFight);

        squad = Squad.playerSquadInstance;

        squad.OnInFightFlagChanged += PlayerSquadInstance_OnInFightFlagChanged;
        squad.OnFormationChanged += PlayerSquadInstance_OnFormationChanged;

        ShowFormationIco(squad.CurrentFormation);
    }

    void OnDestroy()
    {
        if (squad != null)
        {
            Squad.playerSquadInstance.OnInFightFlagChanged -= PlayerSquadInstance_OnInFightFlagChanged;
            Squad.playerSquadInstance.OnFormationChanged -= PlayerSquadInstance_OnFormationChanged;
        }
    }

    private void PlayerSquadInstance_OnFormationChanged(FormationStats.Formations formation)
    {
        ShowFormationIco(formation);
    }

    void ShowFormationIco(FormationStats.Formations formation)
    {
        switch (formation)
        {
            case FormationStats.Formations.RANKS:

                inRanks.gameObject.SetActive(true);
                inPhalanx.gameObject.SetActive(false);
                inShields.gameObject.SetActive(false);

                break;

            case FormationStats.Formations.PHALANX:

                inRanks.gameObject.SetActive(false);
                inPhalanx.gameObject.SetActive(true);
                inShields.gameObject.SetActive(false);

                break;

            case FormationStats.Formations.RISEDSHIELDS:

                inRanks.gameObject.SetActive(false);
                inPhalanx.gameObject.SetActive(false);
                inShields.gameObject.SetActive(true);

                break;
        }
    }

    private void PlayerSquadInstance_OnInFightFlagChanged(bool newValue)
    {
        if (newValue)
            ShowCG(inFight);
        else
            HideCG(inFight);
    }

    void ShowCG(CanvasGroup cg)
    {
        inFight.alpha = 1;
        inFight.blocksRaycasts = true;
    }

    void HideCG(CanvasGroup cg)
    {
        inFight.alpha = 0;
        inFight.blocksRaycasts = false;
    }
}
