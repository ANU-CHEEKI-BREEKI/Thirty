using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class DSFlags : IResetable, IMergeable
{
    [SerializeField] bool isFirstStartGame;
    [SerializeField] bool needTraining;
    //для туториала костыль
    [SerializeField] GameManager.SceneIndex avalaibleTutorialLevel;

    public bool IsFirstStartGame { get { return isFirstStartGame; } set { isFirstStartGame = value; } }
    public bool NeedTraining { get { return needTraining; } set { needTraining = value; } }
    public GameManager.SceneIndex AvalaibleTutorialLevel { get { return avalaibleTutorialLevel; } set { avalaibleTutorialLevel = value; } }
    
    [SerializeField] MarketHelp marketHelp;
    public MarketHelp MarketHelpFlags { get { return marketHelp; } }

    public DSFlags()
    {
        Reset();
    }

    public void Reset()
    {
        isFirstStartGame = true;
        needTraining = true;

        marketHelp = new MarketHelp();

        avalaibleTutorialLevel = GameManager.SceneIndex.LEVEL_TUTORIAL_1;
    }

    public void Merge(object data)
    {
        var d = data as DSFlags;

        isFirstStartGame = d.isFirstStartGame || isFirstStartGame;
        needTraining = d.needTraining || needTraining;

        if (d.avalaibleTutorialLevel > avalaibleTutorialLevel)
            avalaibleTutorialLevel = d.avalaibleTutorialLevel;
    }

    [Serializable]
    public class MarketHelp
    {
        /// <summary>
        /// Обзор вкладок сцены (кнопок панели для открытия вкладок и назначения вкладок)
        /// </summary>
        public bool needOverviewHelp = true;
        [Space]
        //обзор всех вклаок внутри
        public bool needMarketPlaceHelp = true;
        public bool needHospitalHelp = true;
        public bool needStudiesHelp = true;
        public bool needTrainingwHelp = true;
        public bool needDonateHelp = true;
    }
}
