using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneTimeWait : MonoBehaviour
{
    public GameObject retryButton, restartButton, withdrawButton;

    

    private static RespawnManager respawn;

    // Start is called before the first frame update
    void Start()
    {
        respawn = FindObjectOfType<RespawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(retryButton);
            RetryOption();
            RestartOption();
            WithdrawOption();
        }
    }

    public void RetryOption()
    {
        SceneManager.LoadScene(0);
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
