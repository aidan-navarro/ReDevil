using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    public Vector2 respawnPoint;
    public Vector2 startingPoint;

    public bool initialized = false;

    public int rand;

    public string sceneID;

    // Start is called before the first frame update
    void Awake()
    {    
        if (instance == null)
        {
            PlayerFSMController player = FindObjectOfType<PlayerFSMController>();
            respawnPoint = player.transform.position;
            startingPoint = player.transform.position;
            rand = Random.Range(1, 100);
            instance = this;
            initialized = true;
            sceneID = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(gameObject);
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetEnemyStatus(Dictionary<string, bool> enemyList)
    {
        enemyList.ToList().ForEach(x => GameObject.Find(x.Key)?.gameObject.SetActive(x.Value)); // For every enemy in the scene set their active state based on the saved data from the stat tracker
    }

    public void SetPlayerStatus(GameStats gameStats)
    {
        PlayerFSMController player = FindObjectOfType<PlayerFSMController>();

        player.SetHealth(gameStats.currentHealth);
        player.SetSoul(gameStats.currentSoulAmount);
        player.UpdateScoreDisplay();
    }
    
}
