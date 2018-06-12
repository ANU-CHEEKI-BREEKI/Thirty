﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum SceneIndex { MAIN_MENU, MARKET, LEVEL, LOADING_SCREEN}
    public static GameManager Instance { get; private set; }
    
    bool gamePaused;
    public bool GamePaused { get { return gamePaused; } private set { gamePaused = value; } }

    public Ground.GroundType GroundType = Ground.GroundType.GRASSLAND;

    [Header("Score")]
    [SerializeField]
    PlayerProgress playerProgress = new PlayerProgress();
    public PlayerProgress PlayerProgress { get { return playerProgress; } }

    [Header("Time manage")]
    [SerializeField]
    [Range(0, 1)]
    float timeScale = 1;

    [Header("Settings")]
    [SerializeField]
    Settings settings = new Settings();
    public Settings Settings { get { return settings; } }

    [Header("Game")]
    [SerializeField]
    bool controlGroundSize = true;
    public bool ControlGroundSize { get { return controlGroundSize; } }
    [SerializeField] int rows = 2;
    public int RowCount { get { return rows; } }
    [SerializeField] int cols = 2;
    public int ColCount { get { return cols; } }

    float defFixedDeltaTime;

    /// <summary>
    /// начальная позиция игрока на крте
    /// </summary>
    [HideInInspector] public Vector2 entranceBlockPosition = Vector2.zero;
    /// <summary>
    /// позиция на карте для перехода на следующий уровень
    /// </summary>
    [HideInInspector] public Vector2 exitBlockPosition = Vector2.zero;
    [HideInInspector] public MapBlock.Direction exitDirection;
    [HideInInspector] public MapBlock.Direction entranceDirection;

    int level;
    public int Level { get { return level; } }

    Squad squad;
    public Squad PlayerSquad { get { return squad; } }

    public ISavingManager SavingManager { get; set; } = new PlayerPrefsSavingManager();

    /// <summary>
    /// первый агрумент - номер сцены, на которую будет совершен переход. второй агрумент - номер игрового уровня
    /// </summary>
    public event Action<SceneIndex, int> BeforeLoadLevel;

    void Awake()
    {
        Debug.Log("Awake");

        Localization.SetLanguage(SystemLanguage.Russian);
        //Localization.SetLanguage(SystemLanguage.English);

        if (Instance == null)
        {
            Instance = this;
            level = 0;
            defFixedDeltaTime = Time.fixedDeltaTime;

            Application.logMessageReceived += OnUnhendeledException;

            DownloadSaves();

            BeforeLoadLevel += (a, b) =>
            {
                if (a == SceneIndex.MAIN_MENU)
                    ResetPlayerNonmoneyScore();
                playerProgress.Save();

                SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.MUSIC, 1.5f);
                SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.FX, 1.5f);
                SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.UI, 1.5f);
            };

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.LEVEL)
            Instance.Pause();
        else
            Instance.Resume();
    }

    private void Start()
    {
        Debug.Log("Start");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnLevelLoaded    " + scene.name + "    " + mode);

        var index = (SceneIndex)scene.buildIndex;

        switch (index)
        {
            case SceneIndex.MAIN_MENU:
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.MusicMainMenu, true), SoundManager.SoundType.MUSIC);
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.BonfireMainMenu, true, 0.1f), SoundManager.SoundType.FX);
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.SharpingSwordMainMenu, true, 0.2f), SoundManager.SoundType.FX);
                break;
            case SceneIndex.MARKET:
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.MusicMarket, false), SoundManager.SoundType.MUSIC);
                break;
            case SceneIndex.LEVEL:
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.MusicLevel, true), SoundManager.SoundType.MUSIC);

                Pause();
                Ground.Instance.OnGenerationDone += Resume;
                Ground.Instance.OnGenerationDone += InitPlayer;
                Ground.Instance.GeneradeMap(rows, cols);

                break;
            case SceneIndex.LOADING_SCREEN:
                break;
            default:
                break;
        }
    }

    public void DownloadSaves()
    {
        settings.Load();
        playerProgress.Load();
    }

    /// <summary>
    /// Золото сбрасывать НЕЛЬЗЯ !!! Это покупная валюта!
    /// </summary>
    public void ResetPlayerNonmoneyScore()
    {
        playerProgress.score.expirience.Value = 0;
        playerProgress.score.silver.Value = 0;
    }

    void OnUnhendeledException(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
        {
            DialogBox.Instance
                .SetTitle("Необработаное системное сообщение")
                .SetText(condition + "\r\n\r\n" + stackTrace, true)
                .AddCancelButton("Ясно")
                .SetSize(900, 600)
                .Show(Vector2.zero, this);

            Instance.OnApplicationPause(true);
        }
    }

    private void OnApplicationQuit()
    {
        settings.Save();

        ResetPlayerNonmoneyScore();

        playerProgress.Save();
    }

    public void InitPlayer()
    {
        squad = Squad.playerSquadInstance;

        Vector2 pos;
        Quaternion rot;

        MapBlock block = Ground.Instance.MiniGrid[(int)entranceBlockPosition.y][(int)entranceBlockPosition.x].block;

        if (block.Exit == MapBlock.Direction.LEFT)
        {
            pos = new Vector2(
                MapBlock.WORLD_BLOCK_SIZE * 0.1f,
                MapBlock.WORLD_BLOCK_SIZE * 0.5f - MapBlock.BLOCK_SCALE * 0.5f
            );
            rot = Quaternion.identity * Quaternion.Euler(0, 0, -90);
        }
        else
        {
            pos = new Vector2(
                MapBlock.WORLD_BLOCK_SIZE * 0.5f - MapBlock.BLOCK_SCALE * 0.5f,
                MapBlock.WORLD_BLOCK_SIZE * 0.1f
            );
            rot = Quaternion.identity;
        }

        squad.PositionsTransform.position = pos;
        squad.PositionsTransform.rotation = rot;
        squad.endLookRotation = rot;
        squad.GoTo(null);

        squad.ResetUnitPositions();

        Camera.main.transform.position = new Vector3(
            squad.PositionsTransform.position.x,
            squad.PositionsTransform.position.y,
            Camera.main.transform.position.z
        );

    }

    void Update()
    {
        if (!GamePaused)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = defFixedDeltaTime * timeScale;
        }
    }

    public void SetPause(bool pause)
    {
        if (pause)
            Pause();
        else
            Resume();
    }

    public void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        GamePaused = false;
        Time.timeScale = timeScale;
    }

    public void LoadNextLevel()
    {
        level++;

        ReconfigureLevelSettings();

        if (BeforeLoadLevel != null)
            BeforeLoadLevel(SceneIndex.LEVEL, level);

        var firstSkill = Squad.playerSquadInstance.Inventory.FirstSkill;
        var secondSkill = Squad.playerSquadInstance.Inventory.SecondSkill;
        if (firstSkill.Skill != null)
            firstSkill.SkillStats = firstSkill.Skill.CalcUpgradedStats(playerProgress.skills.skills.Find((t) => { return t.Id == firstSkill.Skill.Id; }).Upgrades);
        if (secondSkill.Skill != null)
            secondSkill.SkillStats = secondSkill.Skill.CalcUpgradedStats(playerProgress.skills.skills.Find((t) => { return t.Id == secondSkill.Skill.Id; }).Upgrades);

        LoadScene(SceneIndex.LEVEL);
    }

    public void LoadMarket()
    {
        if (BeforeLoadLevel != null)
            BeforeLoadLevel(SceneIndex.MARKET, level);

        LoadScene(SceneIndex.MARKET);
    }

    public void LoadMainMenu()
    {
        level = 0;

        if (Squad.playerSquadInstance != null)
        {
            Destroy(Squad.playerSquadInstance.gameObject);
            Squad.playerSquadInstance = null;
        }

        ReconfigureLevelSettings();

        if (BeforeLoadLevel != null)
            BeforeLoadLevel(SceneIndex.MAIN_MENU, level);

        LoadScene(SceneIndex.MAIN_MENU);
    }

    void LoadScene(SceneIndex index)
    {
        LoadingScreenManager.NextLevel = index;
        SceneManager.LoadScene((int)SceneIndex.LOADING_SCREEN);
    }

    private void ReconfigureLevelSettings()
    {
        if (level == 0)
        {
            rows = 1;
            cols = 2;
        }
        else
        {
            if (level % 2 == 0)
            {
                if (rows < cols)
                    rows++;
                else
                    cols++;
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            var go = GameObject.Find("PauseToggle");
            if (go != null)
                go.GetComponent<UnityEngine.UI.Toggle>().isOn = pause;
        }
    }
    

    [ContextMenu("ResetAllSettings")]
    public void ResetAllSettings()
    {
        settings = new Settings();
        settings.Save();
    }

    [ContextMenu("ResetAudioSettings")]
    public void ResetAudioSettings()
    {
        settings.audioSettings = new AudioSettings();
        settings.audioSettings.Save();
    }

    [ContextMenu("ResetCommonSettings")]
    public void ResetCommonSettings()
    {
        settings.commonSettings = new CommonSettings();
        settings.commonSettings.Save();
    }

    [ContextMenu("ResetGraphixSettings")]
    public void ResetGraphixSettings()
    {
        settings.graphixSettings = new GraphixSettings();
        settings.graphixSettings.Save();
    }

    [ContextMenu("ResetAllProgress")]
    public void ResetAllProgress()
    {
        playerProgress = new PlayerProgress();
        playerProgress.Save();
    }

    [ContextMenu("ResetScore")]
    public void ResetScore()
    {
        playerProgress.score = new DSPlayerScore();
        playerProgress.score.Save();
    }

    [ContextMenu("ResetStats")]
    public void ResetStats()
    {
        playerProgress.stats = new DSUnitStats();
        playerProgress.stats.Save();
    }

    [ContextMenu("ResetSkills")]
    public void ResetSkills()
    {
        playerProgress.skills = new DSPlayerSkills();
        playerProgress.skills.Save();
    }
}
