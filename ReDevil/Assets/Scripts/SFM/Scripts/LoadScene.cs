using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string sceneName;

    private RespawnManager respawn;
    // Start is called before the first frame update
    void Start()
    {
        respawn = FindObjectOfType<RespawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(respawn != null)
            {
                Destroy(respawn.gameObject);
            }
            else
            {
                respawn = FindObjectOfType<RespawnManager>();
                Destroy(respawn.gameObject);
            }
            LoadingData.sceneToLoad = sceneName;
            SceneManager.LoadScene("LoadingScreen");
        }
    }
}
