using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject retry, restart, withdraw;

    [SerializeField] private TextMeshProUGUI text;

    // don't know where the respawn manager is
    private static RespawnManager respawn;

    // menu selection integer
    private string sceneID;

    private PlayerFSMController pc;

    // Start is called before the first frame update
    void Start()
    {
        respawn = FindObjectOfType<RespawnManager>();
        sceneID = null;
        pc = FindObjectOfType<PlayerFSMController>();

    }
    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == retry)
        {
            text.SetText("Retry the level from the last reached checkpoint");
        }
        else if(EventSystem.current.currentSelectedGameObject == restart)
        {
            text.SetText("Restart the level from the beginning");
        }
        else
        {
            text.SetText("Return to the main menu");
        }
    }

    public void RestartOption()
    {
        //pc.SetIsPaused(false);
        //pc.UnPause();

        sceneID = respawn.sceneID;
        //respawn.respawnPoint = respawn.startingPoint;

        //loadingScreen.LoadLevelAsync(sceneID);
        LoadingData.sceneToLoad = sceneID;
        SceneManager.LoadScene("LoadingScreen");
    }

    //public void SampleDebugSpawn()
    //{
    //    //pc.SetIsPaused(false);
    //    //pc.UnPause();

    //    Destroy(respawn);
    //    LoadingData.sceneToLoad = "SampleScene";
    //    SceneManager.LoadScene("LoadingScreen");
    //    //loadingScreen.LoadLevelAsync("SampleScene");
    
    //}

    public void RestartLevelOption()
    {
        //pc.SetIsPaused(false);
        //pc.UnPause();
        sceneID = respawn.sceneID;

        Destroy(respawn);
        LoadingData.sceneToLoad = sceneID;
        SceneManager.LoadScene("LoadingScreen");
        //loadingScreen.LoadLevelAsync("SampleScene");

    }
    public void MainMenu()
    {
        Destroy(respawn);
        pc.SetIsPaused(false);
        pc.UnPause();

        SceneManager.LoadScene("MainMenu");
    }
}
