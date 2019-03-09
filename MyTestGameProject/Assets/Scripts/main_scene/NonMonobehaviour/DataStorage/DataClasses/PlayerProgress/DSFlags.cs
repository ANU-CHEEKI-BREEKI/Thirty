using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class DSFlags : IResetable, IMergeable
{
    [SerializeField] OnceFlag isDefaultLanguageNotSelected;
    [SerializeField] OnceFlag isTrainingStartedAtLeastOnce;
    [SerializeField] OnceFlag isTutorialCompleted;
    //для туториала костыль
    [SerializeField] GameManager.SceneIndex avalaibleTutorialLevel;
        
    public OnceFlag IsDefaultLanguageSelected { get { return isDefaultLanguageNotSelected; } set { isDefaultLanguageNotSelected = value; } }
    public OnceFlag IsTrainingStartedAtLeastOnce { get { return isTrainingStartedAtLeastOnce; } set { isTrainingStartedAtLeastOnce = value; } }
    public GameManager.SceneIndex AvalaibleTutorialLevel { get { return avalaibleTutorialLevel; } set { avalaibleTutorialLevel = value; } }
    public OnceFlag IsTutorialCompleted { get { return isTutorialCompleted; } set { isTutorialCompleted = value; } }

    [SerializeField] MarketHelp marketHelp;
    public MarketHelp MarketHelpFlags { get { return marketHelp; } }

    public DSFlags()
    {
        Reset();
    }

    public void Reset()
    {
        isDefaultLanguageNotSelected = new OnceFlag();
        isTrainingStartedAtLeastOnce = new OnceFlag();
        isTutorialCompleted = new OnceFlag();

        marketHelp = new MarketHelp();

        avalaibleTutorialLevel = GameManager.SceneIndex.LEVEL_TUTORIAL_1;
    }

    public void Merge(object data)
    {
        var d = data as DSFlags;

        isDefaultLanguageNotSelected.Merge(d.isDefaultLanguageNotSelected);
        isTrainingStartedAtLeastOnce.Merge(d.isTrainingStartedAtLeastOnce);
        isTutorialCompleted.Merge(d.isTutorialCompleted);

        if (d.avalaibleTutorialLevel > avalaibleTutorialLevel)
            avalaibleTutorialLevel = d.avalaibleTutorialLevel;
    }

    [Serializable]
    public class OnceFlag : IMergeable
    {
        [SerializeField] bool flag;
        [SerializeField] bool isOlreadySet;

        public bool Flag { get { return flag; } set { flag = value; isOlreadySet = true; } }
        public bool IsOlreadySet { get { return isOlreadySet; } }

        public void Merge(object data)
        {
            OnceFlag fl = data as OnceFlag;
            if (fl == null) return;

            flag = flag || fl.flag;
            isOlreadySet = isOlreadySet || fl.isOlreadySet;
        }
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
