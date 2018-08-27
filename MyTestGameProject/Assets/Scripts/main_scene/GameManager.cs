using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum SceneIndex { MAIN_MENU, MARKET, LEVEL, LOADING_SCREEN, LEVEL_TUTORIAL_1, LEVEL_TUTORIAL_2, LEVEL_TUTORIAL_3, TESTING_LEVEL }
    public static GameManager Instance { get; private set; }
    
    bool gamePaused;
    public bool GamePaused { get { return gamePaused; } private set { gamePaused = value; } }

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

    public LevelInfo CurrentLevel { get; private set; }

    Squad squad;
    public Squad PlayerSquad { get { return squad; } }

    public ISavingManager SavingManager { get; set; } = new PlayerPrefsSavingManager();

    /// <summary>
    /// первый агрумент - номер сцены, на которую будет совершен переход. второй агрумент - номер игрового уровня
    /// </summary>
    public event Action<SceneIndex, int> BeforeLoadLevel;

    void Awake()
    {
        Localization.SetLanguage(SystemLanguage.Russian);
        //LocalizedStrings.SetLanguage(SystemLanguage.English);

        if (Instance == null)
        {
            Instance = this;
            CurrentLevel = new LevelInfo();
            defFixedDeltaTime = Time.fixedDeltaTime;

            Application.logMessageReceived += OnUnhendeledException;

            DownloadSaves();

            BeforeLoadLevel += (a, b) =>
            {
                ResetPlayerTempScore();
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
                Pause();
                Ground.Instance.OnGenerationDone += Resume;
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
                Pause();
                Ground.Instance.OnWorkDone += Resume;
                Ground.Instance.OnWorkDone += FadeScreen.Instance.FateOnStartScene;
                Ground.Instance.RecalcMatrixByCurrentBlocks();

                break;
            default:

                FadeScreen.Instance.FadeOnStartScene = false;
                Pause();
                Ground.Instance.OnWorkDone += Resume;
                //Ground.Instance.OnWorkDone += InitPlayer;
                Ground.Instance.OnWorkDone += FadeScreen.Instance.FateOnStartScene;
                Ground.Instance.RecalcMatrixByCurrentBlocks();

                break;
        }
    }

    public void DownloadSaves()
    {
        settings.Load();
        playerProgress.Load();
    }

    public void ResetPlayerTempScore()
    {
        playerProgress.Score.ResetTempValues();
    }

    void OnUnhendeledException(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
        {
            Instance.Pause();
            DialogBox.Instance
                .SetTitle("Необработаное системное сообщение")
                .SetText(condition + "\r\n\r\n" + stackTrace, true)
                .AddButton("Ясно", () => { Instance.Resume(); DialogBox.Instance.Hide(); })
                .SetSize(900, 600)
                .Show(Vector2.zero, this);
        }
    }

    private void OnApplicationQuit()
    {
        settings.Save();

        ResetPlayerTempScore();

        playerProgress.Save();
    }

    public void InitPlayer()
    {
        squad = Squad.playerSquadInstance;


        var mods = squad.StatsModifiers;
        foreach (var m in mods)
            squad.RemoveStatsModifier(m);
        var modst = squad.TerrainStatsModifiers;
        foreach (var mt in modst)
            squad.RemoveTerrainStatsModifier(mt);

        Vector2 pos;
        Quaternion rot;

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

            Camera.main.transform.position = new Vector3(
                squad.PositionsTransform.position.x,
                squad.PositionsTransform.position.y,
                Camera.main.transform.position.z
            );
        }

        var progress = GameManager.Instance.PlayerProgress;

        squad.SetUnitsStats(progress.Stats);
        var skill = progress.Skills.firstSkill;
        squad.Inventory.FirstSkill.Skill = skill;
        if (skill != null)
            squad.Inventory.FirstSkill.SkillStats = skill.CalcUpgradedStats(progress.Skills.skills.Find((t) => { return t.Id == skill.Id; }).Upgrades);

        skill = progress.Skills.secondSkill;
        squad.Inventory.SecondSkill.Skill = progress.Skills.secondSkill;
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
        CurrentLevel.NextLevel();

        ReconfigureLevelSettings();        

        var firstSkill = Squad.playerSquadInstance.Inventory.FirstSkill;
        var secondSkill = Squad.playerSquadInstance.Inventory.SecondSkill;
        if (firstSkill.Skill != null)
            firstSkill.SkillStats = firstSkill.Skill.CalcUpgradedStats(playerProgress.Skills.skills.Find((t) => { return t.Id == firstSkill.Skill.Id; }).Upgrades);
        if (secondSkill.Skill != null)
            secondSkill.SkillStats = secondSkill.Skill.CalcUpgradedStats(playerProgress.Skills.skills.Find((t) => { return t.Id == secondSkill.Skill.Id; }).Upgrades);

        LoadScene(SceneIndex.LEVEL);
    }

    public void LoadMarket()
    {
        LoadScene(SceneIndex.MARKET);
    }

    public void LoadMainMenu()
    {
        CurrentLevel = new LevelInfo();

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
            BeforeLoadLevel(index, CurrentLevel.Level);

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
    
    public class LevelInfo
    {
        public Ground.GroundType GroundType { get; private set; } = Ground.GroundType.GRASSLAND;
        /// <summary>
        /// Текущий уровень на текущем типе уровней
        /// </summary>
        public int Level { get; private set; }
        /// <summary>
        /// Максимальное количество уровней на одном типе уровней
        /// </summary>
        public int MaxLevel { get { return 3; } }
        /// <summary>
        /// Текущий уровень включая все уровни всех типов (типа сквозная нумерация)
        /// </summary>
        public int WholeLevel { get; private set; }
        /// <summary>
        /// Суммарное максимальное количество уровней
        /// </summary>
        public int MaxWholeLevel { get; private set; }

        public float WholeLevelT { get { return (float)WholeLevel / MaxWholeLevel; } }

        public LevelInfo()
        {
            Level = 0;
            WholeLevel = 0;
            GroundType = Ground.GroundType.GRASSLAND;
            MaxWholeLevel = Enum.GetValues(typeof(Ground.GroundType)).Length * MaxLevel;
        }       

        public void NextLevel()
        {
            WholeLevel++;
            Level++;
            if (Level >= MaxLevel)
            {
                var v = (Enum.GetValues(typeof(Ground.GroundType)) as Ground.GroundType[]).ToList();
                var i = v.IndexOf(GroundType);
                if (i < v.Count - 1)
                {
                    GroundType = v[i + 1];
                    Level = 1;
                }
            }
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
        playerProgress.Score.Reset();
        playerProgress.Score.Save();
    }

    [ContextMenu("ResetStats")]
    public void ResetStats()
    {
        playerProgress.Stats.Reset();
        playerProgress.Stats.Save();
    }

    [ContextMenu("ResetSkills")]
    public void ResetSkills()
    {
        playerProgress.Skills.Reset();
        playerProgress.Skills.Save();
    }
}
