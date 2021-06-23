using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DeathScreen : MonoBehaviour
{
    // we're not doing anything with these
    public GameObject retryButton, restartButton, withdrawButton;

    private static RespawnManager respawn;

    // menu selection integer
    private string sceneID;

    // Start is called before the first frame update
    void Start()
    {
        respawn = FindObjectOfType<RespawnManager>();
        sceneID = null;

        //start by clearing the latest selection
        EventSystem.current.SetSelectedGameObject(null);
        //set to the first button in the pause menu
        EventSystem.current.SetSelectedGameObject(retryButton);
    }

    public void RetryOption()
    {
        sceneID = respawn.sceneID;
        SceneManager.LoadScene(sceneID);
    }

    public void RestartOption()
    {
        sceneID = respawn.sceneID;
        respawn.respawnPoint = respawn.startingPoint;
        SceneManager.LoadScene(sceneID);
    }

    public void WithdrawOption()
    {
        Destroy(respawn.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
