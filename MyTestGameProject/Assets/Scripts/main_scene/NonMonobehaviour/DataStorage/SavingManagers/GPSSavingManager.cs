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
        public Type type;
    }

    Dictionary<string, Game> games = new Dictionary<string, Game>();

    public override void SaveData<T>(string name, object data)
    {
        var jsonStr = JsonUtility.ToJson((T)data);

        if (!games.ContainsKey(name))
            games.Add(name, new Game());

        Game game = games[name];

        GPSWrapper.OpenSavedGame(name, (status, gameData) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                game.gameData = gameData;
                game.type = typeof(T);
                GPSWrapper.SaveGame(
                    game.gameData,
                    Encoding.ASCII.GetBytes(jsonStr),
                    game.gameData.TotalTimePlayed + (DateTime.Now - game.gameData.LastModifiedTimestamp),
                    OnSavedGameWritten
                );
            }
            else
            {
                CallOnDataSaved(name, false);
            }
        });
    }

    public override void LoadData<T>(string name)
    {
        if (!games.ContainsKey(name))
            games.Add(name, new Game());

        Game game = games[name];

        GPSWrapper.OpenSavedGame(name, (status, gameData) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                game.gameData = gameData;
                game.type = typeof(T);
                GPSWrapper.LoadGameData(game.gameData, OnSavedGameDataRead);
            }
            else
            {
                CallOnDataLoaded(name, null, false);
            }
        });
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
            Toast.Instance.Show("Не удалось сохранить прогресс");
            CallOnDataSaved(null, false);
        }
    }

    public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            var g = games[game.Filename];
            Toast.Instance.Show("Прогресс загружен: " + game.Filename);

            var jsonStr = Encoding.ASCII.GetString(data);
            var loadedData = JsonUtility.FromJson(jsonStr, g.type);

            CallOnDataLoaded(g.gameData.Filename, loadedData, true);
        }
        else
        {
            Toast.Instance.Show("Не удалось загрузить прогресс");
            CallOnDataLoaded(null, null, false);
        }
    }
}
