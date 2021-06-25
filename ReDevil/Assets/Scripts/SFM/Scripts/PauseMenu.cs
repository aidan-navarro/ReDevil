using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject retry, restart, withdraw;

    private static RespawnManager respawn;
    private static LoadingScreen loadingScreen;
    // menu selection integer
    private string sceneID;

    private PlayerFSMController pc;

    // Start is called before the first frame update
    void Start()
    {
        respawn = FindObjectOfType<RespawnManager>();
        loadingScreen = FindObjectOfType<LoadingScreen>();
        sceneID = null;
        pc = FindObjectOfType<PlayerFSMController>();

    }
    public void RestartOption()
    {
        pc.UnPause();

        sceneID = respawn.sceneID;
        respawn.respawnPoint = respawn.startingPoint;

       // SceneManager.LoadScene(sceneID);
        loadingScreen.LoadLevelAsync(sceneID);
    }

    public void SampleDebugSpawn()
    {
        pc.UnPause();

       // SceneManager.LoadScene("SampleScene");
        loadingScreen.LoadLevelAsync("SampleScene");
    }
    public void MainMenu()
    {
        pc.UnPause();

       // SceneManager.LoadScene("MainMenu");
        loadingScreen.LoadLevelAsync("MainMenu");
    }


}
