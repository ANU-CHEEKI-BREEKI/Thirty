using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GPSWrapper
{
    static public event Action<bool> OnPlayerLoggedInValueChanged;
    static bool playerLoggedIn;
    static public bool PlayerLoggedIn
    {
        get
        {
            return playerLoggedIn;
        }
        private set
        {
            var old = playerLoggedIn;
            playerLoggedIn = value;

            if (old != value && OnPlayerLoggedInValueChanged != null)
                OnPlayerLoggedInValueChanged.Invoke(value);
        }
    }

    static public void LogInPlayer(bool debug, Action<bool> onLogInCompleted)
    {
        if (!PlayerLoggedIn)
        {
            var config = new PlayGamesClientConfiguration
                .Builder()
                .EnableSavedGames()// enables saving game progress.
                .Build();
            PlayGamesPlatform.InitializeInstance(config);

            PlayGamesPlatform.DebugLogEnabled = debug;
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate((b) =>
            {
                PlayerLoggedIn = b;
                if (onLogInCompleted != null)
                    onLogInCompleted.Invoke(b);
            });
        }
        else
        {
            if (onLogInCompleted != null)
                onLogInCompleted.Invoke(true);
        }
    }

    static public void LogOutPlayer()
    {
        PlayGamesPlatform.Instance.SignOut();
        PlayerLoggedIn = false;
    }

    static public void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime, Action<SavedGameRequestStatus, ISavedGameMetadata> onSavedGameWritten)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, (status , data) =>
        {
            if (status != SavedGameRequestStatus.Success)
                PlayerLoggedIn = false;

            if(onSavedGameWritten != null)
                onSavedGameWritten.Invoke(status, data);
        });
    }

    static public void OpenSavedGame(string filename, Action<SavedGameRequestStatus, ISavedGameMetadata> onSavedGameOpened)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
             (status, data) =>
             {
                 if (status != SavedGameRequestStatus.Success)
                     PlayerLoggedIn = false;

                 if (onSavedGameOpened != null)
                     onSavedGameOpened.Invoke(status, data);
             }
         );
    }

    static public void LoadGameData(ISavedGameMetadata game, Action<SavedGameRequestStatus, byte[], ISavedGameMetadata> onSavedGameDataRead)
    {
        SavedGameRequestStatus status;
        byte[] data;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, (s, d) => 
        {
            status = s;
            data = d;

            if (status != SavedGameRequestStatus.Success)
                PlayerLoggedIn = false;

            if (onSavedGameDataRead != null)
                onSavedGameDataRead.Invoke(status, data, game);
        });
    }

    static public void ShowSavedGamesUI(Action<SelectUIStatus, ISavedGameMetadata> onSavedGameSelected, Action<bool> onSavesOpened)
    {
        if (PlayerLoggedIn)
        {
            uint maxNumToDisplay = 5;
            bool allowCreateNew = false;
            bool allowDelete = true;

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.ShowSelectSavedGameUI("Select saved game",
                maxNumToDisplay,
                allowCreateNew,
                allowDelete,
                onSavedGameSelected);
        }
        if (onSavesOpened != null)
            onSavesOpened.Invoke(PlayerLoggedIn);
    }

    static public void ShowAchivementsGUI(Action<bool> onAchOpened)
    {
        if(PlayerLoggedIn)
            Social.ShowAchievementsUI();

        if (onAchOpened != null)
            onAchOpened.Invoke(PlayerLoggedIn);
    }
       
    public static class Achivement
    {
        static public void Unlock(string achivementID, Action<bool> callback)
        {
            Social.ReportProgress(achivementID, 100, (succes)=>
            {
                if (succes)
                    Debug.Log("------------" + achivementID + "   unlocked");
                else
                    Debug.Log("------------" + achivementID + "   can't be unlocked");

                if (callback != null)
                    callback.Invoke(succes);
            });
        }

        static public void Show(string achivementID, Action<bool> callback)
        {
            Social.ReportProgress(achivementID, 0, (succes) =>
            {
                if (succes)
                    Debug.Log("------------" + achivementID + "   showed");
                else
                    Debug.Log("------------" + achivementID + "   can't be showed");

                if (callback != null)
                    callback.Invoke(succes);
            });
        }

        static public void ReportProgress(string achivementID, double progress, Action<bool> callback)
        {
            Social.ReportProgress(achivementID, progress, (succes) =>
            {
                if (succes)
                    Debug.Log("------------" + achivementID + "   progress reported: " + progress.ToString(StringFormats.intNumberPercent));
                else
                    Debug.Log("------------" + achivementID + "   progress can't be reported: " + progress.ToString(StringFormats.intNumberPercent));

                if (callback != null)
                    callback.Invoke(succes);
            });
        }

        static public void IncrementProgress(string achivementID, int steps, Action<bool> callback)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(achivementID, steps, (succes) =>
            {
                if (succes)
                    Debug.Log("------------" + achivementID + "   progress incremented: " + steps.ToString(StringFormats.intSignNumber));
                else
                    Debug.Log("------------" + achivementID + "   progress can't be incremented: " + steps.ToString(StringFormats.intSignNumber));

                if (callback != null)
                    callback.Invoke(succes);
            });
        }



        /// <summary>
        /// Отправляет прогресс с задержкой Delay, накапливая знаения. Callback не стакается!!!!
        /// </summary>
        /// <param name="achivementID"></param>
        /// <param name="steps"></param>
        /// <param name="context">Лучше всего подойдет GameManeger.Instance</param>
        public static void IncrementProgressWithDelay(string achivementID, int steps, Action<bool> callback, MonoBehaviour context)
        {
            if (!dict.ContainsKey(achivementID))
            {
                dict.Add(achivementID, steps);
                dictAct.Add(achivementID, callback);
            }
            else
                dict[achivementID] += steps;

            if(cor == null)
                context.StartCoroutine(CorIncrementWithDelay());
        }

        /// <summary>
        ///Delay for IncrementProgressWithDelay
        /// </summary>
        public static float Delay { get; set; } = 1f;

        static Dictionary<string, int> dict = new Dictionary<string, int>();
        static Dictionary<string, Action<bool>> dictAct = new Dictionary<string, Action<bool>>();

        static Coroutine cor;

        /// <summary>
        /// Ждем задержку и стакаем переменные. По окончанию - отправляем
        /// </summary>
        static IEnumerator CorIncrementWithDelay()
        {
            while (dict.Count > 0)
            {
                yield return new WaitForSeconds(Delay);
                var keys = new List<string>(dict.Keys);
                foreach (var key in keys)
                {
                    Debug.Log("------------IncrementProgressWithDelay for achivementID: " + key +" by: " + dict[key]);
                    IncrementProgress(key, dict[key], dictAct[key]);                   
                    dict.Remove(key);
                    dictAct.Remove(key);
                }
            }
            cor = null;
        }
    }
}
