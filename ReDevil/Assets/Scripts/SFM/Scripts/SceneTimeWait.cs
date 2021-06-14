using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SceneTimeWait : MonoBehaviour
{
    public GameObject retryButton, restartButton, withdrawButton;

    // don't know where the respawn manager is
    private static RespawnManager respawn;

    // menu selection integer
    private string sceneID;

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
        //playerInput.onActionTriggered += OnActionTriggered;
    }

  

    // Update is called once per frame
    void Update()
    {
        //// must change this
        //if (Input.GetButtonDown("Submit"))
        //{
        //    EventSystem.current.SetSelectedGameObject(null);
        //    EventSystem.current.SetSelectedGameObject(retryButton);
        //    RetryOption();
        //    RestartOption();
        //    WithdrawOption();
        //}
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

    public void RetryOption()
    {
        sceneID = respawn.sceneID;
        SceneManager.LoadScene(sceneID);
    }
    
   public void RestartOption()
    {
        
        Destroy(respawn.gameObject);
        SceneManager.LoadScene(0);
    }

    public void WithdrawOption()
    {
        
        Destroy(respawn.gameObject);
        SceneManager.LoadScene(2);
    }




}
