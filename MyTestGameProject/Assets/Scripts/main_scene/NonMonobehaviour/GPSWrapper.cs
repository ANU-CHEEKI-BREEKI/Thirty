using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GPSWrapper
{
    static public bool PlayerLoggedIn { get; private set; } = false;

    static public void LogInPlayer(bool debug, Action<bool> onLogInCompleted)
    {
        var config = new PlayGamesClientConfiguration
            .Builder()
            .EnableSavedGames()// enables saving game progress.
            .Build();
        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = debug;
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((b)=>
        {
            PlayerLoggedIn = b;
            if(onLogInCompleted != null)
                onLogInCompleted.Invoke(b);
        });
    }

    static public void SignOutPlayer()
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
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, onSavedGameWritten);
    }

    static public void OpenSavedGame(string filename, Action<SavedGameRequestStatus, ISavedGameMetadata> onSavedGameOpened)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            onSavedGameOpened
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

    public static void ShowAchivementsGUI(Action<bool> onAchOpened)
    {
        if(PlayerLoggedIn)
            Social.ShowAchievementsUI();
        if (onAchOpened != null)
            onAchOpened.Invoke(PlayerLoggedIn);
    }
}
