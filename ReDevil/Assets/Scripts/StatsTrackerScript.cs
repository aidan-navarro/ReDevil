using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

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
    public string levelName;
    public int score;
    public float gameTime;
    public float damageTaken;
    public float currentHealth;
    public float currentSoulAmount;
    public int numRetries;
    public bool usedCheckpoint; // Used a checkpoint to continue game and not passed a checkpoint
    public int numEnemies; // Active Enemies
    public int totalNumEnemies; // Total Enemies Within A Level
    public Dictionary<Achivements, bool> achivementList;
    public Dictionary<string, bool> enemyAliveStatusList; // Which enemies are still alive and which are dead (used by the respawn manager to deactivate enemies that are "dead" when the level reloads)
    public Dictionary<string, bool> importantEnemiesAliveStatusList; // This other list is for important enemies that the player must defeat in other to progress through the level (related to the PACIFIST achievement)

    public void CopyStats(GameStats other)
    {
        levelName = other.levelName;
        score = other.score;
        gameTime = other.gameTime;
        damageTaken = other.damageTaken;
        currentHealth = other.currentHealth;
        currentSoulAmount = other.currentSoulAmount;
        numRetries = other.numRetries;
        usedCheckpoint = other.usedCheckpoint;
        numEnemies = other.numEnemies;
        totalNumEnemies = other.totalNumEnemies;
        achivementList = new Dictionary<Achivements, bool>(other.achivementList);
        enemyAliveStatusList = new Dictionary<string, bool>(other.enemyAliveStatusList);
        importantEnemiesAliveStatusList = new Dictionary<string, bool>(other.importantEnemiesAliveStatusList);
    }
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
    private List<string> levelSceneNames; // Which Scenes are actual game levels which the Stats&Respawn script needs to reset for
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
            RespawnManager.instance.SetEnemyStatus(GetEnemyStatusList());
            RespawnManager.instance.SetPlayerStatus(getCurrentGameStats());
        }
    }

    public void OnEnemyDeath(string enemyName, int enemyPoints, Vector3 enemyWorldPosition)
    {
        currentGameStats.score += enemyPoints;
        if (currentGameStats.enemyAliveStatusList.ContainsKey(enemyName))
        {
            currentGameStats.numEnemies--;
            currentGameStats.enemyAliveStatusList[enemyName] = false;
            Debug.Log(savedGameStats.enemyAliveStatusList[enemyName]);
        }
        else if (currentGameStats.importantEnemiesAliveStatusList.ContainsKey(enemyName))
        {
            currentGameStats.numEnemies--;
            currentGameStats.importantEnemiesAliveStatusList[enemyName] = false;
            Debug.Log(savedGameStats.importantEnemiesAliveStatusList[enemyName]);
        }
        PointsPopUp pointsPopUp = Instantiate(PointsUpPrefab).GetComponent<PointsPopUp>();
        pointsPopUp.transform.position = enemyWorldPosition;
        pointsPopUp.ChangePoints(enemyPoints);

        player.UpdateScoreDisplay();
    }

    public void OnDamageTaken(float damage)
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
        savedGameStats.CopyStats(currentGameStats);
    }
    public void OnResetLevel()
    {
        levelSceneName = "None";
    }
    public void OnResetCheckpoint()
    {
        SetIsTrackingTime(false);
        savedGameStats.usedCheckpoint = true;
        savedGameStats.numRetries++;
        currentGameStats.CopyStats(savedGameStats);
    }

    private void ResetStatTracker()
    {
        currentGameStats = new GameStats();
        currentGameStats.achivementList = new Dictionary<Achivements, bool>();
        currentGameStats.enemyAliveStatusList = new Dictionary<string, bool>();
        currentGameStats.importantEnemiesAliveStatusList = new Dictionary<string, bool>();

        currentGameStats.levelName = SceneManager.GetActiveScene().name;

        foreach (Achivements achivementIndex in System.Enum.GetValues(typeof(Achivements)))
        {
            currentGameStats.achivementList.Add(achivementIndex, false);
        }
        currentGameStats.achivementList[Achivements.NO_CHECKPOINTS] = true;
        currentGameStats.achivementList[Achivements.PACIFIST] = true;
        currentGameStats.achivementList[Achivements.NO_DAMAGE] = true;

        foreach (EnemyFSMController enemy in FindObjectsOfType<EnemyFSMController>())
        {
            if (enemy.importantEnemy)
            {
                currentGameStats.importantEnemiesAliveStatusList.Add(enemy.gameObject.name, true);
            }
            else
            {
                currentGameStats.enemyAliveStatusList.Add(enemy.gameObject.name, true);
            }
            currentGameStats.numEnemies++;
            currentGameStats.totalNumEnemies++;
        }

        OnCheckPointHit();
        DontDestroyOnLoad(gameObject);
        SetIsTrackingTime(true);
    }

    public void OnLevelCompleted()
    {
        SetIsTrackingTime(false);
        bool importantEnemiesAllKilled = true;
        
        // Set the status of the achievements
        if (currentGameStats.usedCheckpoint)
        {
            currentGameStats.achivementList[Achivements.NO_CHECKPOINTS] = false;
        }
        foreach(KeyValuePair<string, bool> enemyStatus in currentGameStats.enemyAliveStatusList)
        {
            if (enemyStatus.Value != true)
            {
                currentGameStats.achivementList[Achivements.PACIFIST] = false;
                break;
            }
        }
        foreach(KeyValuePair<string, bool> importantEnemyStatus in currentGameStats.importantEnemiesAliveStatusList)
        {
            if (importantEnemyStatus.Value == true)
            {
                importantEnemiesAllKilled = false;
                break;
            }
        }

        if (importantEnemiesAllKilled && currentGameStats.achivementList[Achivements.PACIFIST])
        {
            currentGameStats.achivementList[Achivements.SLAYER] = true;
        }
    }


    public GameStats getCurrentGameStats()
    {
        return currentGameStats;
    }

    public void SetIsTrackingTime(bool tracking)
    {
        trackTime = tracking;
    }

    public Dictionary<string, bool> GetEnemyStatusList()
    {
        Dictionary<string, bool> totalEnemyList = new Dictionary<string, bool>();
        currentGameStats.enemyAliveStatusList.ToList().ForEach(x => totalEnemyList.Add(x.Key, x.Value));
        currentGameStats.importantEnemiesAliveStatusList.ToList().ForEach(x => totalEnemyList.Add(x.Key, x.Value));
        return totalEnemyList;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single); // Return to the regular level;
    }

}
