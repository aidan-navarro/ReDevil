using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

//This script holds all functions that are used on the main menu buttons
public class MainMenu : MonoBehaviour, GameplayControls.IMenuActions
{
    public GameplayControls controls;

    public void Awake()
    {
        controls = new GameplayControls();
        controls.Menu.Select.performed += ctx => OnSelect(ctx);
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed Select Button");
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void PlayOption()
    {
        LoadingData.sceneToLoad = "Tutorial Level";
        SceneManager.LoadScene("LoadingScreen");
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
