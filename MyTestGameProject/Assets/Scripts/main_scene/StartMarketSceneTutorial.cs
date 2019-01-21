using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMarketSceneTutorial : MonoBehaviour
{
    enum Type { OVERVIEW, MARKET, HOSPITAL, STUDIES, TRAINING, DONATE}

    [SerializeField] Type type;
    [SerializeField] GameObject tutorial;
    [SerializeField] StartMarketSceneTutorial enableNextTutorial;

    void Start()
    {
        bool needTutorial = false;
        var flags = GameManager.Instance.SavablePlayerData.PlayerProgress.Flags.MarketHelpFlags;

        switch (type)
        {
            case Type.OVERVIEW:
                needTutorial = flags.needOverviewHelp;
                flags.needOverviewHelp = false;
                break;
            case Type.MARKET:
                needTutorial = flags.needMarketPlaceHelp;
                flags.needMarketPlaceHelp = false;
                break;
            case Type.HOSPITAL:
                needTutorial = flags.needHospitalHelp;
                flags.needHospitalHelp = false;
                break;
            case Type.STUDIES:
                needTutorial = flags.needStudiesHelp;
                flags.needStudiesHelp = false;
                break;
            case Type.TRAINING:
                needTutorial = flags.needTrainingwHelp;
                flags.needTrainingwHelp = false;
                break;
            case Type.DONATE:
                needTutorial = flags.needDonateHelp;
                flags.needDonateHelp = false;
                break;
        }
        if (needTutorial)
            tutorial?.SetActive(true);
        else
            if (enableNextTutorial != null)
                enableNextTutorial.enabled = true;
    }
}
