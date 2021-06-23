using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SceneTimeWait : MonoBehaviour
{
    // we're not doing anything with these
    public GameObject retryButton, restartButton, withdrawButton;

    private static RespawnManager respawn;

    // menu selection integer
    private string sceneID;

    private PlayerFSMController pc;


    //public int GetISelect()
    //{
    //    return m_iSelect;
    //}

    //public void SetISelect(int inISelect)
    //{
    //    m_iSelect = inISelect;
    //}
    //// Player Input
    //private PlayerInput playerInput;
    //private GameplayControls gameplayControls;

    // Start is called before the first frame update

    void Start()
    {
        respawn = FindObjectOfType<RespawnManager>();
        sceneID = null;
        pc = FindObjectOfType<PlayerFSMController>();
        //playerInput.onActionTriggered += OnActionTriggered;
    }

  

    // Update is called once per frame
    void Update()
    {
       // must change this
       if (Input.GetButtonDown("Submit"))
       {
           EventSystem.current.SetSelectedGameObject(null);
           EventSystem.current.SetSelectedGameObject(retryButton);
           RetryOption();
           RestartOption();
           WithdrawOption();
       }
    }

    //private void OnActionTriggered(InputAction.CallbackContext obj)
    //{
    //    if (obj.action.name == gameplayControls.Gameplay.Movement.name)
    //    {
    //        Debug.Log("Test");

    //    }
    //}

    //private void OutputTest(InputAction.CallbackContext obj)
    //{
    //    Debug.Log(obj.valueSizeInBytes);
    //}

    public void MainMenu()
    {
        pc.SetIsPaused(false);
        pc.UnPause();

        SceneManager.LoadScene("MainMenu");
    }

    public void SampleDebugSpawn()
    {
        PlayerFSMController pc = FindObjectOfType<PlayerFSMController>();
        pc.SetIsPaused(false);
        pc.UnPause();

        SceneManager.LoadScene("SampleScene");
    }

    public void RetryOption()
    {
        PlayerFSMController pc = FindObjectOfType<PlayerFSMController>();
        pc.SetIsPaused(false);
        pc.UnPause();

        sceneID = respawn.sceneID;
        SceneManager.LoadScene(sceneID);
    }
    
   public void RestartOption()
    {
        PlayerFSMController pc = FindObjectOfType<PlayerFSMController>();
        pc.SetIsPaused(false);
        pc.UnPause();

        sceneID = respawn.sceneID;
        respawn.respawnPoint = respawn.startingPoint;
        SceneManager.LoadScene(sceneID);
    }

    public void WithdrawOption()
    {
        PlayerFSMController pc = FindObjectOfType<PlayerFSMController>();
        pc.SetIsPaused(false);
        pc.UnPause();

        Destroy(respawn.gameObject);
        SceneManager.LoadScene("MainMenu");
    }




}
