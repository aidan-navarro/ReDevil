using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    // we're not doing anything with these
    public GameObject retryButton, restartButton, withdrawButton;

    [SerializeField]
    private TextMeshProUGUI text;

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

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == retryButton)
        {
            text.SetText("Retry the level from the last reached checkpoint");
        }
        else if (EventSystem.current.currentSelectedGameObject == restartButton)
        {
            text.SetText("Restart the level from the beginning");
        }
        else
        {
            text.SetText("Return to the main menu");
        }
    }

    public void RetryOption()
    {
        sceneID = respawn.sceneID;
        LoadingData.sceneToLoad = sceneID;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void RestartOption()
    {
        sceneID = respawn.sceneID;
        respawn.respawnPoint = respawn.startingPoint;

        LoadingData.sceneToLoad = sceneID;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void WithdrawOption()
    {
        Destroy(respawn.gameObject);
        LoadingData.sceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScreen");
    }
}
