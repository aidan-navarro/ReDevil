using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThanksForPlayingScreen : MonoBehaviour
{
    // menu selection integer
    //private string sceneID;

    private static RespawnManager respawn;
    // Start is called before the first frame update
    void Start()
    {
        respawn = FindObjectOfType<RespawnManager>();
        //sceneID = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenuScreen()
    {
        Destroy(respawn.gameObject);
        LoadingData.sceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScreen");
    }
}
