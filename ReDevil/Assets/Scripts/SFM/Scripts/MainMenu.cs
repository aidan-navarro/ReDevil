using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MainMenu : MonoBehaviour
{

    public GameObject playButton, creditsButton, quitButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButtonDown("Submit"))
        //{
        //    EventSystem.current.SetSelectedGameObject(null);
        //    EventSystem.current.SetSelectedGameObject(playButton);
        //    PlayOption();
        //    CreditsOption();
        //    QuitOption();
        //}
    }
    public void PlayOption()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void CreditsOption()
    {
        Debug.Log("Credits overlay opens here");

    }

    public void QuitOption()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
        
    }
}
