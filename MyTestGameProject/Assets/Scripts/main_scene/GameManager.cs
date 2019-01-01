﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum SceneIndex { MAIN_MENU, MARKET, LEVEL, LOADING_SCREEN, LEVEL_TUTORIAL_1, LEVEL_TUTORIAL_2, LEVEL_TUTORIAL_3, TESTING_LEVEL }
    public static GameManager Instance { get; private set; }

    //КОСТЫЛЬ ЕБАННЫЙ
    public string command = string.Empty;

    public event Action<bool> OnGamePased = (paused) => { };
    bool gamePaused;
    public bool GamePaused { get { return gamePaused; } }

    [SerializeField] LevelInfo currentLevel;
    public LevelInfo CurrentLevel { get { return currentLevel; } private set { currentLevel = value; } }

    [Header("Savable Data")]
    [SerializeField] SavablePlayerData savablePlayerData = new SavablePlayerData();
    public SavablePlayerData SavablePlayerData { get { return savablePlayerData; } }

    [Header("Time manage")]
    [SerializeField] [Range(0, 1)] float timeScale = 1;

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

    public event Action OnBackButtonPressed = ()=>{ };

    Squad squad;
    public Squad PlayerSquad { get { return squad; } }

    public ISavingManager SavingManager { get; set; }

    /// <summary>
    /// первый агрумент - номер сцены, на которую будет совершен переход. 
    /// второй агрумент - номер текущего игрового уровня.
    /// третий аргумент - номер текущей сцены
    /// </summary>
    public event Action<SceneIndex, int, SceneIndex> BeforeLoadLevel;

    void Awake()
    {
        if (Instance == null)
        {
            InitGameManager();
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.LEVEL)
            Instance.PauseGame();
        else
            Instance.ResumeGame();
    }

    private void InitGameManager()
    {
        Instance = this;
        CurrentLevel = new LevelInfo();
        defFixedDeltaTime = Time.fixedDeltaTime;
        Application.logMessageReceived += OnUnhendeledException;

        Localization.SetLanguage(SystemLanguage.Russian);

        var auds = SavablePlayerData.Settings.audioSettings;
        auds.generalVolume.Value = 0;

        bool debug = false;
#if DEBUG
        debug = true;
#endif
        var mes = LocalizedStrings.log_in;

        ModalInfoPanel.Instance.Add(mes);

        Action<bool> onLogInCompleted = (succes) =>
        {
            ModalInfoPanel.Instance.Remove(mes);

            if (succes)
            {
                SavingManager = new GPSSavingManager();
                GPSWrapper.Achivement.Unlock(GPSConstants.achievement_welcome, null);
            }
            else
            {
                SavingManager = new PlayerPrefsSavingManager();
                Toast.Instance.Show(LocalizedStrings.cant_log_in);
            }

            //Это событие вызывается перед событием неудачи действия в GPSWrapper,
            //поэтому в нем назнаем другой менеджер
            //а в самом менеджере просто пробуем сохранить/загрузить ещё разок
            GPSWrapper.OnPlayerLoggedInValueChanged += (logIn) =>
            {
                if (logIn)
                {
                    SavingManager = new GPSSavingManager();
                    var tempSaves = savablePlayerData.Copy() as SavablePlayerData;

                    Action onLoad = null;
                    onLoad = () =>
                    {
                        savablePlayerData.OnLoaded -= onLoad;
                        savablePlayerData.Merge(tempSaves);
                    };
                    savablePlayerData.OnLoaded += onLoad;
                    savablePlayerData.Load();
                }
                else
                {
                    SavingManager = new PlayerPrefsSavingManager();
                }
            };

            DownloadSaves();
        };
        GPSWrapper.LogInPlayer(debug, onLogInCompleted);

        BeforeLoadLevel += (nextScene, currentLevel, currentScene) =>
        {
            savablePlayerData.PlayerProgress.Save();

            SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.MUSIC, 1.5f);
            SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.FX, 1.5f);
            SoundManager.Instance.StopPlayingChannel(SoundManager.SoundType.UI, 1.5f);

            if (nextScene == SceneIndex.MARKET)
            {
                if (command == "loadSquad")
                {
                    squad.Load(GameManager.Instance.SavablePlayerData.PlayerProgress.Squad);
                    CurrentLevel.SetValues(GameManager.Instance.SavablePlayerData.PlayerProgress.Level);
                    command = string.Empty;
                }
            }
        };

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        var index = (SceneIndex)scene.buildIndex;

        switch (index)
        {
            case SceneIndex.MAIN_MENU:
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.SoundClipsContainer.Music.MusicMainMenu, true), SoundManager.SoundType.MUSIC);
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.SoundClipsContainer.FX.BonfireMainMenu, true, 0.1f), SoundManager.SoundType.FX);
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.SoundClipsContainer.FX.SharpingSwordMainMenu, true, 0.2f), SoundManager.SoundType.FX);
                InitPlayer();

                break;
            case SceneIndex.MARKET:
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.SoundClipsContainer.Music.MusicMarket, false), SoundManager.SoundType.MUSIC);
                InitPlayer();

                break;
            case SceneIndex.LEVEL:
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.SoundClipsContainer.Music.MusicLevel, true), SoundManager.SoundType.MUSIC);

                FadeScreen.Instance.FadeOnStartScene = false;
                PauseGame();
                Ground.Instance.OnGenerationDone += ResumeGame;
                Ground.Instance.OnGenerationDone += InitPlayer;
                Ground.Instance.OnGenerationDone += FadeScreen.Instance.FateOnStartScene;
                Ground.Instance.GeneradeMap(rows, cols);

                break;
            case SceneIndex.LOADING_SCREEN:
                break;
            case SceneIndex.LEVEL_TUTORIAL_1:
            case SceneIndex.LEVEL_TUTORIAL_2:
            case SceneIndex.LEVEL_TUTORIAL_3:
                SoundManager.Instance.PlaySound(new SoundChannel.ClipSet(SoundManager.Instance.SoundClipsContainer.Music.MusicLevel, true), SoundManager.SoundType.MUSIC);

                FadeScreen.Instance.FadeOnStartScene = false;
                PauseGame();
                Ground.Instance.OnWorkDone += ResumeGame;
                Ground.Instance.OnWorkDone += FadeScreen.Instance.FateOnStartScene;
                Ground.Instance.RecalcMatrixByCurrentBlocks();

                break;
            default:

                FadeScreen.Instance.FadeOnStartScene = false;
                PauseGame();
                Ground.Instance.OnWorkDone += ResumeGame;
                //Ground.Instance.OnWorkDone += InitPlayer;
                Ground.Instance.OnWorkDone += FadeScreen.Instance.FateOnStartScene;
                Ground.Instance.RecalcMatrixByCurrentBlocks();

                break;
        }
    }

    public void DownloadSaves()
    {
        savablePlayerData.Load();
    }

    public void ResetPlayerTempProgressValues()
    {
        savablePlayerData.PlayerProgress.ResetTempValues();
    }

    public void ApplyPlayerTempProgressValues()
    {
        savablePlayerData.PlayerProgress.ApplyTempValues();

        ResetPlayerTempProgressValues();    
    }

    void OnUnhendeledException(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            Instance.PauseGame();
            DialogBox.Instance
                .SetTitle(LocalizedStrings.unhendeled_exception)
                .SetText(condition + "\r\n\r\n" + stackTrace, true)
                .AddButton(LocalizedStrings.got_it, () => { Instance.ResumeGame(); DialogBox.Instance.Hide(); })
                .Show(Vector2.zero, this);
        }
    }

    public void SaveAndQuit()
    {
        ResetPlayerTempProgressValues();

        if(GPSWrapper.PlayerLoggedIn)
        {
            Action onSaved = null;
            onSaved = () =>
            {
                savablePlayerData.OnSaved -= onSaved;
                GPSWrapper.LogOutPlayer();
                Savelocal();
            };
            savablePlayerData.OnSaved += onSaved;
            savablePlayerData.Save();
        }
        else
        {
            Savelocal();
        }
    }

    void Savelocal()
    {
        SavingManager = new PlayerPrefsSavingManager();
        Action onSaved = null;
        onSaved = () =>
        {
            savablePlayerData.OnSaved -= onSaved;
            Debug.Log("Quit");
            Application.Quit();            
        };
        savablePlayerData.OnSaved += onSaved;
        savablePlayerData.Save();
    }

    public void InitPlayer()
    {
        squad = Squad.playerSquadInstance;

        //сбрасываем модификаторы
        var mods = squad.StatsModifiers;
        foreach (var m in mods)
            squad.RemoveStatsModifier(m);
        var modst = squad.TerrainStatsModifiers;
        foreach (var mt in modst)
            squad.RemoveTerrainStatsModifier(mt);

        Vector2 pos;
        Quaternion rot;
        
        //выставляем начаьную позицию отряда
        if (SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.LEVEL)
        {
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
            squad.EndLookRotation = rot;
            squad.Path = null;

            squad.ResetUnitPositions();
            
            //выставляем камеру на отряд
            Camera.main.transform.position = new Vector3(
                squad.PositionsTransform.position.x,
                squad.PositionsTransform.position.y,
                Camera.main.transform.position.z
            );
        }

        var progress = GameManager.Instance.SavablePlayerData.PlayerProgress;

        squad.SetUnitsStats(progress.Stats);
        var skill = progress.Skills.firstSkill.Skill;
        squad.Inventory.FirstSkill.Skill = skill;
        if (skill != null)
            squad.Inventory.FirstSkill.SkillStats = skill.CalcUpgradedStats(progress.Skills.skills.Find((t) => { return t.Id == skill.Id; }).Upgrades);

        skill = progress.Skills.secondSkill.Skill;
        squad.Inventory.SecondSkill.Skill = skill;
        if (skill != null)
            squad.Inventory.SecondSkill.SkillStats = skill.CalcUpgradedStats(progress.Skills.skills.Find((t) => { return t.Id == skill.Id; }).Upgrades);
    }

    void Update()
    {
        if (!GamePaused)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = defFixedDeltaTime * timeScale;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            BackButton();
    }

    void BackButton()
    {
        switch ((SceneIndex)SceneManager.GetActiveScene().buildIndex)
        {
            case SceneIndex.MAIN_MENU:  
                //в самой сцене подписываемся на событие (кнопка выхода с игры)
                break;
            case SceneIndex.MARKET:
                DialogBox.Instance
                    .SetTitle(LocalizedStrings.quit_to_main_menu_title)
                    .SetText(LocalizedStrings.quit_to_main_menu_assert)
                    .AddButton(LocalizedStrings.yes, GameManager.Instance.LoadMainMenu)
                    .AddCancelButton(LocalizedStrings.no)
                    .Show();
                break;
            case SceneIndex.LOADING_SCREEN:
                break;
            case SceneIndex.LEVEL_TUTORIAL_1:
            case SceneIndex.LEVEL_TUTORIAL_2:
            case SceneIndex.LEVEL_TUTORIAL_3:
            case SceneIndex.TESTING_LEVEL:
            case SceneIndex.LEVEL:
                SetPauseGame(!GamePaused);
                break;
            default:
                break;
        }
        OnBackButtonPressed.Invoke();
    }

    public void SetPauseGame(bool pause)
    {
        if (pause)
            PauseGame();
        else
            ResumeGame();
    }

    public void PauseGame()
    {
        var old = gamePaused;
        gamePaused = true;
        Time.timeScale = 0;

        if (old != gamePaused)
            OnGamePased.Invoke(gamePaused);
    }

    public void ResumeGame()
    {
        var old = gamePaused;
        gamePaused = false;
        Time.timeScale = timeScale;

        if(old != gamePaused)
            OnGamePased.Invoke(gamePaused);
    }

    public void LoadNextLevel()
    {
        ResetPlayerTempProgressValues();

        CurrentLevel.NextLevel();

        ReconfigureLevelSettings();        

        var firstSkill = Squad.playerSquadInstance.Inventory.FirstSkill;
        var secondSkill = Squad.playerSquadInstance.Inventory.SecondSkill;
        if (firstSkill.Skill != null)
            firstSkill.SkillStats = firstSkill.Skill.CalcUpgradedStats(savablePlayerData.PlayerProgress.Skills.skills.Find((t) => { return t.Id == firstSkill.Skill.Id; }).Upgrades);
        if (secondSkill.Skill != null)
            secondSkill.SkillStats = secondSkill.Skill.CalcUpgradedStats(savablePlayerData.PlayerProgress.Skills.skills.Find((t) => { return t.Id == secondSkill.Skill.Id; }).Upgrades);

        LoadScene(SceneIndex.LEVEL);
    }

    public void LoadMarket()
    {
        LoadScene(SceneIndex.MARKET);
    }

    public void LoadMainMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.MARKET)
        {
            var gm = GameManager.Instance.SavablePlayerData.PlayerProgress;
            gm.Squad.SetSquadValues(Squad.playerSquadInstance);
            gm.Level.SetValues(GameManager.Instance.CurrentLevel);
        }

        CurrentLevel.Reset();

        if (Squad.playerSquadInstance != null)
        {
            Destroy(Squad.playerSquadInstance.gameObject);
            Squad.playerSquadInstance = null;
        }

        ReconfigureLevelSettings();

        LoadScene(SceneIndex.MAIN_MENU);
    }

    public void LoadTutorialLevel(SceneIndex index)
    {
        if (index != GameManager.SceneIndex.LEVEL_TUTORIAL_1 &&
            index != GameManager.SceneIndex.LEVEL_TUTORIAL_2 &&
            index != GameManager.SceneIndex.LEVEL_TUTORIAL_3)
            throw new Exception("даный уровень не обучающий. невозможно его загручить как обучающий.");

        if (Squad.playerSquadInstance != null)
        {
            Destroy(Squad.playerSquadInstance.gameObject);
            Squad.playerSquadInstance = null;
        }
        LoadScene(index);
    }

    public void LoadTestingLevel(SceneIndex index)
    {
        if (Squad.playerSquadInstance != null)
        {
            Destroy(Squad.playerSquadInstance.gameObject);
            Squad.playerSquadInstance = null;
        }
        LoadScene(index);
    }

    void LoadScene(SceneIndex index)
    {
        if (BeforeLoadLevel != null)
            BeforeLoadLevel(index, CurrentLevel.Level, (SceneIndex)SceneManager.GetActiveScene().buildIndex);

        LoadingScreenManager.NextLevel = index;
        SceneManager.LoadScene((int)SceneIndex.LOADING_SCREEN);
    }

    private void ReconfigureLevelSettings()
    {
        if (CurrentLevel.Level == 1)
        {
            rows = 2;
            cols = 2;
        }
        else
        {
            if (CurrentLevel.Level % 2 == 0)
            {
                if (rows < cols)
                    rows++;
                else
                    cols++;
            }
        }
    }
    
    [ContextMenu("ResetAllSettings")]
    public void ResetAllSettings()
    {
        savablePlayerData.Reset();
    }

    [ContextMenu("ResetAudioSettings")]
    public void ResetAudioSettings()
    {
        savablePlayerData.Settings.audioSettings.Reset();
    }

    [ContextMenu("ResetCommonSettings")]
    public void ResetCommonSettings()
    {
        savablePlayerData.Settings.commonSettings.Reset();
    }

    [ContextMenu("ResetGraphixSettings")]
    public void ResetGraphixSettings()
    {
        savablePlayerData.Settings.graphixSettings.Reset();
    }

    [ContextMenu("ResetAllProgress")]
    public void ResetAllProgress()
    {
        savablePlayerData.PlayerProgress.Reset();
    }

    [ContextMenu("ResetScore")]
    public void ResetScore()
    {
        savablePlayerData.PlayerProgress.Score.Reset();
    }

    [ContextMenu("ResetStats")]
    public void ResetStats()
    {
        savablePlayerData.PlayerProgress.Stats.Reset();
    }

    [ContextMenu("ResetSkills")]
    public void ResetSkills()
    {
        savablePlayerData.PlayerProgress.Skills.Reset();
    }

    [ContextMenu("ResetLevel")]
    public void ResetLevel()
    {
        savablePlayerData.PlayerProgress.Level.Reset();
    }

    [ContextMenu("ResetAllowedEquipment")]
    public void ResetAllowedEquipment()
    {
        savablePlayerData.PlayerProgress.Equipment.Reset();
    }
}
