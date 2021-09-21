using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public struct LevelStats
{
    public string levelName;
    public float bestClearTime;
    public int LevelAPoints;
    public int LevelBPoints;
    public int LevelCPoints;
    public int LevelDPoints;
}


public class ResultsScript : MonoBehaviour
{
    [SerializeField]
    private float pointsIncreaseTime = 1.0f;
    [Header("Points Value")]
    [SerializeField]
    private int missionClearPoints;
    [SerializeField]
    private int pointsPerHealth;
    [SerializeField]
    private int pointsPerSoul;
    [SerializeField]
    private int pointsPerEnemy;
    [SerializeField]
    private int maxClearTimePoints;
    [SerializeField]
    private int pointsPerRetry;
    [SerializeField]
    private List<LevelStats> levelStatsList;
    [Header("CodeNames")]
    [SerializeField]
    private string NO_CHECKPOINTS;
    [SerializeField]
    private string NO_DAMAGE;
    [SerializeField]
    private string SLAYER;
    [SerializeField]
    private string PACIFIST;
    [SerializeField]
    private string SPEEDRUNNER;
    [Header("DisplayLine")]
    [SerializeField]
    private GameObject scoreDisplayLine;
    [SerializeField]
    private GameObject missionDisplayLine;
    [SerializeField]
    private GameObject heathDisplayLine;
    [SerializeField]
    private GameObject soulDisplayLine;
    [SerializeField]
    private GameObject enemiesDisplayLine;
    [SerializeField]
    private GameObject clearTimeDisplayLine;
    [SerializeField]
    private GameObject retryDisplayLine;
    [SerializeField]
    private GameObject totalDisplayLine;
    [SerializeField]
    private GameObject levelDisplayLine;
    [SerializeField]
    private GameObject codeNameDisplayLine;
    [Header("Points Display")]
    [SerializeField]
    private TextMeshProUGUI scorePointsDisplay;
    [SerializeField]
    private TextMeshProUGUI missionPointsDisplay;
    [SerializeField]
    private TextMeshProUGUI heathPointsDisplay;
    [SerializeField]
    private TextMeshProUGUI soulPowerPointsDisplay;
    [SerializeField]
    private TextMeshProUGUI enemiesPointsDisplay;
    [SerializeField]
    private TextMeshProUGUI clearTimePointsDisplay;
    [SerializeField]
    private TextMeshProUGUI retryPointsDisplay;
    [SerializeField]
    private TextMeshProUGUI totalPointsDisplay;
    [Header("Result Display")]
    [SerializeField]
    private TextMeshProUGUI missionResultDisplay;
    [SerializeField]
    private TextMeshProUGUI healthResultDisplay;
    [SerializeField]
    private TextMeshProUGUI soulPowerResultDisplay;
    [SerializeField]
    private TextMeshProUGUI enemyResultDisplay;
    [SerializeField]
    private TextMeshProUGUI clearTimeResultDisplay;
    [SerializeField]
    private TextMeshProUGUI retryResultDisplay;
    [SerializeField]
    private TextMeshProUGUI levelResultDisplay;
    [SerializeField]
    private TextMeshProUGUI codeNameResultDisplay;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private string ThanksForPlayingSceneName;

    private int scorePoints;
    private int healthPoints;
    private int soulPoints;
    private int enemyPoints;
    private int clearTimePoints;
    private int retryPoints;
    private int totalPoints;
    private GameStats gameresults;
    private LevelStats levelStats;

    // Start is called before the first frame update
    void Start()
    {
        CalculatePoints();
        UpdateDisplayText();
    }

    private IEnumerator ChangePointsDisplay(TextMeshProUGUI pointsDisplay, int endScore, int startScore = 0)
    {
        float timer = 0;

        while (timer < pointsIncreaseTime)
        {
            timer += Time.deltaTime;
            pointsDisplay.text = ((int)Mathf.Lerp(startScore, endScore, timer / pointsIncreaseTime)).ToString() + "P";
            yield return new WaitForEndOfFrame();
        }
    }

    private void CalculatePoints()
    {
        gameresults = StatsTrackerScript.instance.getCurrentGameStats();
        foreach(LevelStats levelStat in levelStatsList)
        {
            if (levelStat.levelName == gameresults.levelName)
            {
                levelStats = levelStat;
                break;
            }
        }

        scorePoints = gameresults.score;
        healthPoints = (int)gameresults.currentHealth * pointsPerHealth;
        soulPoints = (int)gameresults.currentSoulAmount * pointsPerSoul;
        enemyPoints = Mathf.Abs(gameresults.numEnemies - gameresults.totalNumEnemies) * pointsPerEnemy;
        clearTimePoints = (int)Mathf.Abs(levelStats.bestClearTime / gameresults.gameTime * maxClearTimePoints);
        retryPoints = gameresults.numRetries * pointsPerRetry;
        totalPoints = scorePoints + healthPoints + soulPoints + enemyPoints + clearTimePoints + retryPoints;
    }
    private void UpdateDisplayText()
    {
        healthResultDisplay.text = gameresults.currentHealth.ToString() + "HP";
        soulPowerResultDisplay.text = gameresults.currentSoulAmount.ToString();
        enemyResultDisplay.text = Mathf.Abs(gameresults.numEnemies - gameresults.totalNumEnemies).ToString() + "/" + gameresults.totalNumEnemies.ToString();
        int minutes = (int)(gameresults.gameTime / 60);
        int seconds = (int)(gameresults.gameTime - minutes * 60);
        clearTimeResultDisplay.text = minutes.ToString() + "'" + seconds.ToString();
        retryResultDisplay.text = gameresults.numRetries.ToString();
        string levelGrade = "D-";
        if (levelStats.LevelAPoints <= totalPoints)
        {
            levelGrade = "A";
        }
        else if (levelStats.LevelBPoints <= totalPoints)
        {
            levelGrade = "B";
        }
        else if (levelStats.LevelCPoints <= totalPoints)
        {
            levelGrade = "C";
        }
        else if (levelStats.LevelDPoints <= totalPoints)
        {
            levelGrade = "D";
        }
        levelResultDisplay.text = levelGrade;
        string codeName = "Red"; // Default Placeholder
        if (gameresults.gameTime <= levelStats.bestClearTime)
        {
            gameresults.achivementList[Achivements.NO_CHECKPOINTS] = true;
            codeName = NO_CHECKPOINTS;
        }
        else if (gameresults.achivementList[Achivements.PACIFIST])
        {
            codeName = PACIFIST;
        }
        else if (gameresults.achivementList[Achivements.SLAYER])
        {
            codeName = SLAYER;
        }
        else if (gameresults.achivementList[Achivements.NO_DAMAGE])
        {
            codeName = NO_DAMAGE;
        }
        else if (gameresults.achivementList[Achivements.NO_CHECKPOINTS])
        {
            codeName = NO_CHECKPOINTS;
        }

        codeNameResultDisplay.text = codeName;
    }

    public void UpdateScorePoints()
    {
        scoreDisplayLine.SetActive(true);
        StartCoroutine(ChangePointsDisplay(scorePointsDisplay, gameresults.score));
    }

    public void UpdateMissionPoints()
    {
        missionDisplayLine.SetActive(true);
        StartCoroutine(ChangePointsDisplay(missionPointsDisplay, missionClearPoints));
    }

    public void UpdateHealthPoints()
    {
        heathDisplayLine.SetActive(true);
        StartCoroutine(ChangePointsDisplay(heathPointsDisplay, healthPoints));
    }

    public void UpdateSoulPoints()
    {
        soulDisplayLine.SetActive(true);
        StartCoroutine(ChangePointsDisplay(soulPowerPointsDisplay, soulPoints));
    }

    public void UpdateEnemyPoints()
    {
        enemiesDisplayLine.SetActive(true);
        StartCoroutine(ChangePointsDisplay(enemiesPointsDisplay, enemyPoints));
    }

    public void UpdateClearTimePoints()
    {
        clearTimeDisplayLine.SetActive(true);
        StartCoroutine(ChangePointsDisplay(clearTimePointsDisplay, clearTimePoints));
    }

    public void UpdateRetryPoints()
    {
        retryDisplayLine.SetActive(true);
        StartCoroutine(ChangePointsDisplay(retryPointsDisplay, retryPoints));
    }

    public void UpdateTotalPoints()
    {
        totalDisplayLine.SetActive(true);
        StartCoroutine(ChangePointsDisplay(totalPointsDisplay, totalPoints));
    }

    public void UpdateLevel()
    {
        levelDisplayLine.SetActive(true);
    }

    public void UpdateCodeName()
    {
        codeNameDisplayLine.SetActive(true);
        animator.enabled = false;
    }

    public void ThanksForPlayingScene()
    {
        SceneManager.LoadScene(ThanksForPlayingSceneName);
    }

}
