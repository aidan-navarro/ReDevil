using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Achivements
{
    NO_CHECKPOINTS,
    NO_DAMAGE,
    SLAYER,
    PACIFIST,
    SPEEDRUNNER,
}

[System.Serializable]
public struct GameStats
{
    public int score;
    public float gameTime;
    public int damageTaken;
    public float currentHealth;
    public float currentSoulAmount;
    public bool usedCheckpoint; // Used a checkpoint to continue game and not passed a checkpoint
    public Dictionary<Achivements, bool> achivementList;
    public int numEnemies; // Active Enemies
    public int totalNumEnemies; // Total Enemies Within A Level
    public Dictionary<string, bool> enemyAliveStatusList; // Which enemies are still alive and which are dead (used by the respawn manager to deactivate enemies that are "dead" when the level reloads)
}

public class StatsTrackerScript : MonoBehaviour
{
    #region Singleton
    public static StatsTrackerScript instance;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    [SerializeField]
    private List<string> levelSceneNames; // Which Scenes are actual game levels which the Stats&Respawn script need to reset
    [SerializeField]
    private GameObject PointsUpPrefab;
    private string levelSceneName;
    private bool trackTime = false;
    private GameStats savedGameStats;
    private GameStats currentGameStats;
    private PlayerFSMController player;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void Update()
    {
        if (trackTime)
        {
            currentGameStats.gameTime += Time.deltaTime;
        }
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<PlayerFSMController>();

        if (levelSceneName != scene.name && levelSceneNames.Contains(scene.name)) // When the player enters a new level
        {
            levelSceneName = scene.name;
            RespawnManager.instance.sceneID = scene.name;
            ResetStatTracker();
        }
        else if (levelSceneName == scene.name)
        {
            SetIsTrackingTime(true);
        }
    }

    public void OnEnemyDeath(string enemyName, int enemyPoints, Vector3 enemyWorldPosition)
    {
        currentGameStats.score += enemyPoints;
        if (currentGameStats.enemyAliveStatusList.ContainsKey(enemyName))
        {
            currentGameStats.numEnemies--;
            currentGameStats.enemyAliveStatusList[enemyName] = false;
        }
        PointsPopUp pointsPopUp = Instantiate(PointsUpPrefab).GetComponent<PointsPopUp>();
        pointsPopUp.transform.position = enemyWorldPosition;
        pointsPopUp.ChangePoints(enemyPoints);

        player.UpdateScoreDisplay();
    }

    public void OnDamageTaken(int damage)
    {
        currentGameStats.damageTaken += damage;

        if (currentGameStats.achivementList[Achivements.NO_DAMAGE])
        {
            currentGameStats.achivementList[Achivements.NO_DAMAGE] = false;
        }
    }
    public void OnCheckPointHit()
    {
        currentGameStats.currentHealth = player.GetHealth();
        currentGameStats.currentSoulAmount = player.GetSoul();
        savedGameStats = currentGameStats;
    }
    public void OnResetLevel()
    {
        levelSceneName = "None";
    }
    public void OnResetCheckpoint()
    {
        SetIsTrackingTime(false);
        savedGameStats.usedCheckpoint = true;
        currentGameStats = savedGameStats;
    }

    private void ResetStatTracker()
    {
        currentGameStats = new GameStats();
        currentGameStats.achivementList = new Dictionary<Achivements, bool>();
        currentGameStats.enemyAliveStatusList = new Dictionary<string, bool>();

        foreach (Achivements achivementIndex in System.Enum.GetValues(typeof(Achivements)))
        {
            currentGameStats.achivementList.Add(achivementIndex, false);
        }
        currentGameStats.achivementList[Achivements.NO_CHECKPOINTS] = true;
        currentGameStats.achivementList[Achivements.PACIFIST] = true;
        currentGameStats.achivementList[Achivements.NO_DAMAGE] = true;

        foreach (EnemyFSMController enemy in FindObjectsOfType<EnemyFSMController>())
        {
            currentGameStats.enemyAliveStatusList.Add(enemy.gameObject.name, true);
            currentGameStats.numEnemies++;
            currentGameStats.totalNumEnemies++;
        }

        savedGameStats = currentGameStats;
        DontDestroyOnLoad(gameObject);
        SetIsTrackingTime(true);
    }

    public void OnLevelCompleted()
    {
        SetIsTrackingTime(false);
        
        // Set the status of the achievements
    }


    public GameStats getCurrentGameStats()
    {
        return currentGameStats;
    }

    public void SetIsTrackingTime(bool tracking)
    {
        trackTime = tracking;
    }
}
