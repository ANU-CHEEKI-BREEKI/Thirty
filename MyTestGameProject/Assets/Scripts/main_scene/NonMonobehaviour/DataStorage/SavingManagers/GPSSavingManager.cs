using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GPSSavingManager : ISavingManager
{
    class Game
    {
        public ISavedGameMetadata gameData;
        public Action onGameOpened;
        public Action onGameCantOpen;
        public Type type;
        public bool IsOpen { get { return gameData != null && gameData.IsOpen; } }
    }

    Dictionary<string, Game> games = new Dictionary<string, Game>();

    public override void SaveData<T>(string name, object data)
    {
        var jsonStr = JsonUtility.ToJson((T)data);

        if (!games.ContainsKey(name))
            games.Add(name, new Game());

        Game game = games[name];

        if (game.IsOpen)
        {
            GPSWrapper.SaveGame(
                game.gameData, 
                Encoding.ASCII.GetBytes(jsonStr), 
                game.gameData.TotalTimePlayed + (DateTime.Now - game.gameData.LastModifiedTimestamp), 
                OnSavedGameWritten
            );
        }
        else
        {
            game.onGameOpened = () =>
            {
                GPSWrapper.SaveGame(
                    game.gameData, 
                    Encoding.ASCII.GetBytes(jsonStr), 
                    game.gameData.TotalTimePlayed + (DateTime.Now - game.gameData.LastModifiedTimestamp),
                    OnSavedGameWritten
                );
                game.type = typeof(T);
                game.onGameOpened = null;
            };
            GPSWrapper.OpenSavedGame(name, OnSavedGameOpened);
        }
    }

    public override void LoadData<T>(string name)
    {
        if (!games.ContainsKey(name))
            games.Add(name, new Game());

        Game game = games[name];

        if (game.IsOpen)
        {
            GPSWrapper.LoadGameData(game.gameData, OnSavedGameDataRead);
        }
        else
        {
            game.onGameOpened = () =>
            {
                GPSWrapper.LoadGameData(game.gameData, OnSavedGameDataRead);
                game.type = typeof(T);
                game.onGameOpened = null;
            };
            GPSWrapper.OpenSavedGame(name, OnSavedGameOpened);
        }
    }

    public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        var g = games[game.Filename];

        if (status == SavedGameRequestStatus.Success)
        {
            g.gameData = game;
            if (g.onGameOpened != null) g.onGameOpened.Invoke();
        }
        else
        {
            g.gameData = null;
            if (g.onGameCantOpen != null) g.onGameCantOpen.Invoke();
        }
    }

    public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            CallOnDataSaved(game.Filename, true);
            Toast.Instance.Show("Прогресс сохранён: " + game.Filename);
        }
        else
        {
            Toast.Instance.Show("Не удалось сохранить прогресс: " + game.Filename);
            CallOnDataSaved(game.Filename, false);
        }
    }

    public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data, ISavedGameMetadata game)
    {
        var g = games[game.Filename];
        if (status == SavedGameRequestStatus.Success)
        {
            Toast.Instance.Show("Прогресс загружен: " + game.Filename);

            var jsonStr = Encoding.ASCII.GetString(data);
            var loadedData = JsonUtility.FromJson(jsonStr, g.type);

            CallOnDataLoaded(g.gameData.Filename, loadedData);
        }
        else
        {
            CallOnDataLoaded(g.gameData.Filename, null);
        }
    }
}
