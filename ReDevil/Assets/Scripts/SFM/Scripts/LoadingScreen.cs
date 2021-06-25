using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    //// creating an instance to carry from scene to scene
    public static LoadingScreen instance;

    [SerializeField] private GameObject m_LoadingScreenObj;
    [SerializeField] private Slider loadingSlider;

    AsyncOperation async;

    private void Start()
    {
        m_LoadingScreenObj.SetActive(false);
    }

    public void LoadLevelAsync(string levelName)
    {
        StartCoroutine(LoadLevelAsynchronously(levelName));
    }

    private IEnumerator LoadLevelAsynchronously(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);

        m_LoadingScreenObj.SetActive(true);

        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f); // operation goes from 0 - 0.9
                                                    // this will give us a 0 - 1 value
            loadingSlider.value = progress;

            //if (progress >= 1f)
            //{
            //    async.allowSceneActivation = true;
            //}
            yield return null; // skip one frame
        }

    }
}
