using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

//This script holds all functions that are used on the main menu buttons
public class MainMenu : MonoBehaviour
{
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
