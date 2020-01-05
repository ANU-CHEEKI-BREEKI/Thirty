using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// всё что с рекламой
/// </summary>
public static class GADWrapper
{
    #region consts

    public enum RewardedAdId
    {
        ID__TEST,
        ID_FREE_GOLD
    }
    public enum InterstitialAdId
    {
        ID__TEST,
        ID_ALL_KIND_OF_INTERSTITIAL_ADS
    }

    private static class Const
    {
        public const string ANDROID_APP_ID = "ca-app-pub-3678745180251733~6721050083";

        private static string[] revardedAdIds = new[]
        {
           "ca-app-pub-3940256099942544/5224354917",//ID__TEST
           "ca-app-pub-3678745180251733/7117114831" //ID_FREE_GOLD
        };
        private static string[] interstitialAdIds = new[]
        {
            "ca-app-pub-3940256099942544/1033173712",//ID__TEST
            "ca-app-pub-3678745180251733/8504735542" //ID_ALL_KIND_OF_INTERSTITIAL_ADS
        };

        public static string GetId(RewardedAdId revardedId)
        {
            return revardedAdIds[(int)revardedId];
        }

        public static string GetId(InterstitialAdId interstitialId)
        {
            return interstitialAdIds[(int)interstitialId];
        }
    }

    #endregion

    //private static bool debug;
    /// <summary>
    /// сколько одинаковых блоков рекламы может быть загружено одновременно(паралельно)
    /// <para>это нужно, например, чтобы после закрытия рекламного окна, уже было готово ещё одно</para>
    /// </summary>
    private static int maximumSameRevardedAds;
    private static int maximumSameInterstitialAds;

    //нужен именно список, чтобы потом можно было удалить обертки в собитиях
    private static List<RewardedAdWrap> rewardedAds = new List<RewardedAdWrap>();
    private static List<InterstitialAdWrap> interAds = new List<InterstitialAdWrap>();

    public class AdWrap
    {
        public Action onAdClosed;

        public float oldGameVolume;
        public bool oldGamePaused;

        public bool reloadAfterClosing;
        public bool loadOneMoreOnOpening;
        public int tryReloadOnLoadingFailed;
    }

    public static void Initialize(int maximumSameRevardedAds = 2, int maximumSameInterstitialAds = 1)
    {
        //GADWrapper.debug = debug;
        GADWrapper.maximumSameRevardedAds = maximumSameRevardedAds;
        GADWrapper.maximumSameInterstitialAds = maximumSameInterstitialAds;

#if UNITY_ANDROID
        var appId = Const.ANDROID_APP_ID;
#else
        var appId = "unexpected_platform";
#endif
        MobileAds.Initialize(appId);
    }

    #region RevarderAds

    /// <summary>
    /// 
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="onRewardEarned"></param>
    /// <returns>false если что то пошло не так. true если началась загрузка</returns>
    public static bool LoadRewardedAd(RewardedAdId adId, int tryReloadOnLoadingFailed = 0)
    {
        //Debug.Log($"advertising dubig is {debug}");

        ////это для тестирования
        //if (debug)
        //    adId = RewardedAdId.ID__TEST;

        var adCount = rewardedAds.Count(a => a.adId == adId);

        Debug.Log($"count of olready loaded advertising {adId} is: {adCount}. and max is: {GADWrapper.maximumSameRevardedAds}");

        if (adCount >= GADWrapper.maximumSameRevardedAds)
            return false;

        var ad = new RewardedAd(Const.GetId(adId));
        rewardedAds.Add
        (
            new RewardedAdWrap()
            {
                adId = adId,
                ad = ad,
                tryReloadOnLoadingFailed = tryReloadOnLoadingFailed
            }
        );

        ad.OnAdClosed += Ad_OnAdClosed;
        ad.OnAdFailedToLoad += Ad_OnAdFailedToLoad;
        ad.OnAdFailedToShow += Ad_OnAdFailedToShow;
        ad.OnAdLoaded += Ad_OnAdLoaded;
        ad.OnAdOpening += Ad_OnAdOpening;
        ad.OnUserEarnedReward += Ad_OnUserEarnedReward;

        //Use AdRequest.Builder.addTestDevice("B36E541FF471B2ADC7410D9DEAC3A651") to get test ads on this device.
        ad.LoadAd(new AdRequest
            .Builder()
            .AddTestDevice("B36E541FF471B2ADC7410D9DEAC3A651")
            .Build()
        );

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="onRewardEarned"></param>
    /// <param name="loadOneMoreOnOpening">загрузить ещё один такой же блок рекламы при открытии этого (чтобы после закрытия УЖЕ был готов к показу)</param>
    /// <param name="reloadAfterClosing">загрузить такой же блок рекламы после закрытия этого</param>
    /// <param name="tryReshowingOnShowingFailed"></param>
    /// <returns>показалась ли рекламка</returns>
    public static bool ShowRewardedAd(RewardedAdId adId, Action<Reward> onRewardEarned = null, bool loadOneMoreOnOpening = false, bool reloadAfterClosing = false, int tryReshowingOnShowingFailed = 0)
    {
        //это для тестирования
        //if (debug)
        //    adId = RewardedAdId.ID__TEST;

        var thisIdAds = rewardedAds.Where(a => a.adId == adId);
        var loadedAd = thisIdAds.Where(r => r.ad.IsLoaded()).FirstOrDefault();

        if (!thisIdAds.Any())
        {
            Toast.Instance.Show(LocalizedStrings.reward_ad_not_loaded);
            return false;
        }

        if (loadedAd == null)
        {
            Toast.Instance.Show(LocalizedStrings.reward_ad_not_loaded_yet);
            return false;
        }

        loadedAd.onRewardEarned = onRewardEarned;
        loadedAd.loadOneMoreOnOpening = loadOneMoreOnOpening;
        loadedAd.reloadAfterClosing = reloadAfterClosing;
        loadedAd.tryReshowingOnShowingFailed = tryReshowingOnShowingFailed;

        loadedAd.ad.Show();
        
        return true;
    }

    #region RewardedAdsEvents

    private static void Ad_OnUserEarnedReward(object sender, Reward e)
    {
        var adWrap = rewardedAds.Where(d => d.ad == sender).SingleOrDefault();
        if (adWrap == null) return;

        try
        {
            adWrap.onRewardEarned?.Invoke(e);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    private static void Ad_OnAdOpening(object sender, EventArgs e)
    {
        var adWrap = rewardedAds.Where(d => d.ad == sender).SingleOrDefault();
        if (adWrap == null) return;

        try
        {
            var gm = GameManager.Instance;
            var audio = gm.SavablePlayerData.Settings.audioSettings;
            adWrap.oldGameVolume = audio.generalVolume.Value;
            adWrap.oldGamePaused = gm.GamePaused;

            audio.generalVolume.Value = 0;
            gm.PauseGame();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        if (adWrap.loadOneMoreOnOpening)
            LoadRewardedAd(adWrap.adId, adWrap.tryReloadOnLoadingFailed);
    }

    private static void Ad_OnAdLoaded(object sender, EventArgs e)
    {
    }

    private static void Ad_OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        Toast.Instance.Show(LocalizedStrings.reward_ad_show_error);

        var adWrap = rewardedAds.Where(d => d.ad == sender).SingleOrDefault();
        if (adWrap == null) return;

        if (adWrap.tryReshowingOnShowingFailed > 0)
            ShowRewardedAd(adWrap.adId, adWrap.onRewardEarned, adWrap.loadOneMoreOnOpening, adWrap.reloadAfterClosing, adWrap.tryReshowingOnShowingFailed - 1);
        else
            rewardedAds.Remove(adWrap);
    }

    private static void Ad_OnAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        var adWrap = rewardedAds.Where(d => d.ad == sender).SingleOrDefault();
        if (adWrap == null) return;

        rewardedAds.Remove(adWrap);

        if (adWrap.tryReloadOnLoadingFailed > 0)
            LoadRewardedAd(adWrap.adId, adWrap.tryReloadOnLoadingFailed - 1);
    }

    private static void Ad_OnAdClosed(object sender, EventArgs e)
    {
        var adWrap = rewardedAds.Where(d => d.ad == sender).SingleOrDefault();
        if (adWrap == null) return;

        rewardedAds.Remove(adWrap);

        try
        {
            var gm = GameManager.Instance;
            var audio = gm.SavablePlayerData.Settings.audioSettings;
            audio.generalVolume.Value = adWrap.oldGameVolume;
            gm.SetPauseGame(adWrap.oldGamePaused, false);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        if (adWrap.reloadAfterClosing)
            LoadRewardedAd(adWrap.adId, adWrap.tryReloadOnLoadingFailed);
    }

    #endregion

    public class RewardedAdWrap : AdWrap
    {
        public RewardedAdId adId;
        public RewardedAd ad;
        public Action<Reward> onRewardEarned;

        public int tryReshowingOnShowingFailed;
    }

    #endregion

    #region InterstitialAds
    public static bool LoadInterstitialAd(InterstitialAdId adId, int tryReloadOnLoadingFailed = 0)
    {
        //это для тестирования
        //if (debug)
        //    adId = InterstitialAdId.ID__TEST;

        if (interAds.Where(rad => rad.adId == adId).ToList().Count >= GADWrapper.maximumSameInterstitialAds)
            return false;

        var ad = new InterstitialAd(Const.GetId(adId));
        interAds.Add
        (
            new InterstitialAdWrap()
            {
                adId = adId,
                ad = ad,
                tryReloadOnLoadingFailed = tryReloadOnLoadingFailed
            }
        );

        ad.OnAdClosed += Ad_OnInterstitialAdClosed;
        ad.OnAdFailedToLoad += Ad_OnInterstitialAdFailedToLoad;
        ad.OnAdLeavingApplication += Ad_OnInterstitialAdLeavingApplication;
        ad.OnAdLoaded += Ad_OnInterstitialAdLoaded;
        ad.OnAdOpening += Ad_OnInterstitialAdOpening;

        ad.LoadAd(new AdRequest
            .Builder()
            .AddTestDevice("B36E541FF471B2ADC7410D9DEAC3A651")
            .Build()
        );

        return true;
    }

    public static bool ShowInterstitialAd(InterstitialAdId adId, bool loadOneMoreOnOpening = false, bool reloadAfterClosing = false)
    {
        //это для тестирования
        //if (debug)
        //    adId = InterstitialAdId.ID__TEST;

        var sameId = interAds.Where(ad => ad.adId == adId).ToList();
        var loaded = sameId.Where(ad => ad.ad.IsLoaded()).ToList();

        if (sameId.Count == 0)
            return false;

        if (loaded.Count == 0)
            return false;

        var adWrap = loaded[0];
        adWrap.loadOneMoreOnOpening = loadOneMoreOnOpening;
        adWrap.reloadAfterClosing = reloadAfterClosing;

        adWrap.ad.Show();

        return true;
    }

    #region InterstitialEvents

    private static void Ad_OnInterstitialAdOpening(object sender, EventArgs e)
    {
        var adWrap = interAds.Where(d => d.ad == sender).SingleOrDefault();
        if (adWrap == null) return;

        try
        {
            var gm = GameManager.Instance;
            var audio = gm.SavablePlayerData.Settings.audioSettings;
            adWrap.oldGameVolume = audio.generalVolume.Value;
            adWrap.oldGamePaused = gm.GamePaused;

            audio.generalVolume.Value = 0;
            gm.PauseGame();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        if (adWrap.loadOneMoreOnOpening)
            LoadInterstitialAd(adWrap.adId, adWrap.tryReloadOnLoadingFailed);
    }

    private static void Ad_OnInterstitialAdLoaded(object sender, EventArgs e)
    {
    }

    private static void Ad_OnInterstitialAdLeavingApplication(object sender, EventArgs e)
    {
    }

    private static void Ad_OnInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        var adWrap = interAds.Where(d => d.ad == sender).SingleOrDefault();
        if (adWrap == null) return;

        interAds.Remove(adWrap);

        adWrap.ad.Destroy();

        if (adWrap.tryReloadOnLoadingFailed > 0)
            LoadInterstitialAd(adWrap.adId, adWrap.tryReloadOnLoadingFailed - 1);
    }

    private static void Ad_OnInterstitialAdClosed(object sender, EventArgs e)
    {
        var adWrap = interAds.Where(d => d.ad == sender).SingleOrDefault();
        if (adWrap == null) return;

        try
        {
            var gm = GameManager.Instance;
            var audio = gm.SavablePlayerData.Settings.audioSettings;

            audio.generalVolume.Value = adWrap.oldGameVolume;
            gm.SetPauseGame(adWrap.oldGamePaused, false);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        interAds.Remove(adWrap);
        adWrap.ad.Destroy();

        if (adWrap.reloadAfterClosing)
            LoadInterstitialAd(adWrap.adId, adWrap.tryReloadOnLoadingFailed);
    }

    #endregion

    public class InterstitialAdWrap : AdWrap
    {
        public InterstitialAdId adId;
        public InterstitialAd ad;
    }

    #endregion
}
