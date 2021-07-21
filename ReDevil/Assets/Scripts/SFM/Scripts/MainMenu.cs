using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;

//This script holds all functions that are used on the main menu buttons
public class MainMenu : MonoBehaviour, GameplayControls.IMenuActions
{
    public GameplayControls controls;

    public GameObject credits, title, play, quit, creditsButton;

    public Button closeCredits, playButton;

    public bool creditsToggled = false;

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
        title.SetActive(false);
        play.SetActive(false);
        quit.SetActive(false);
        creditsButton.SetActive(false);
        credits.SetActive(true);

        closeCredits.Select();
    }

    public void QuitOption()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void CreditsOverlay()
    {
        title.SetActive(true);
        play.SetActive(true);
        quit.SetActive(true);
        creditsButton.SetActive(true);
        credits.SetActive(false);

        playButton.Select();
    }

}
