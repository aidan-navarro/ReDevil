using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject retry, restart, withdraw;

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
    public void RestartOption()
    {
        pc.SetIsPaused(false);
        pc.UnPause();

        sceneID = respawn.sceneID;
        respawn.respawnPoint = respawn.startingPoint;
        SceneManager.LoadScene(sceneID);
    }

    public void SampleDebugSpawn()
    {
        pc.SetIsPaused(false);
        pc.UnPause();

        Destroy(respawn);
        SceneManager.LoadScene("SampleScene");
    }
    public void MainMenu()
    {
        pc.SetIsPaused(false);
        pc.UnPause();

        SceneManager.LoadScene("MainMenu");
    }
}
