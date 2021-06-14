using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    public Vector2 respawnPoint;

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
            Debug.Log("Set Respawnpoint: " + respawnPoint);
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

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
