using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class LoadingScreen : MonoBehaviour, GameplayControls.IMenuActions
{

    [SerializeField] private GameObject m_LoadingScreenObj;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private bool triggerActivation;

    [SerializeField] private TextMeshProUGUI text;

    //PlayerFSMController pc;

    // Player Input
    private GameplayControls menuControls;

    // Async operation is what we'll use for our loading operation
    AsyncOperation async;

    private void Awake()
    {
        menuControls = new GameplayControls();
        menuControls.Menu.Select.started += ctx => triggerActivation = true;
        

        //pc = FindObjectOfType<PlayerFSMController>();

        text.enabled = false;
        triggerActivation = false;

    }
    private void Start()
    {
        Debug.Log(LoadingData.sceneToLoad);
        LoadLevelAsync(LoadingData.sceneToLoad);
    }

    private void OnEnable()
    {
        menuControls.Enable();
    }
    private void OnDisable()
    {
        menuControls.Disable();
    }

    public void OnSelect(InputAction.CallbackContext obj)
    {
        Debug.Log("Get Input");
    }


    // TO DO: place this logic in a separate scene.  carry over a string that will get passed into loadlevelasync\
    public void LoadLevelAsync(string levelName)
    {
        StartCoroutine(LoadLevelAsynchronously(levelName));
    }

    private IEnumerator LoadLevelAsynchronously(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        async.allowSceneActivation = false;
        m_LoadingScreenObj.SetActive(true);

        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f); // operation goes from 0 - 0.9
                                                    // this will give us a 0 - 1 value
            loadingSlider.value = progress;
            if (progress >= 1f)
            {
                text.enabled = true;
                if (triggerActivation)
                {
                    Debug.Log("Load Complete");
                    async.allowSceneActivation = true;
                   
                }
            }
            yield return null; // skip one frame
        }

    }

}
