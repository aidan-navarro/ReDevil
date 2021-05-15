using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneTimeWait : MonoBehaviour
{
    public GameObject retryButton, restartButton, withdrawButton;

    

    // Start is called before the first frame update
    void Start()
    {
        
       //StartCoroutine("testWait");
      

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
        Destroy(gameObject);
           SceneManager.LoadScene(0);
     
    }

    public void WithdrawOption()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(2);
    }


    public IEnumerator testWait()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(0);
    }


}
