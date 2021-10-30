using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using System;
using UnityEngine.Playables;

public class CineMachineSwitcher : MonoBehaviour
{

    [SerializeField]
    private InputAction action;

    [SerializeField]
    private CinemachineVirtualCamera vcam1; //player cam

    [SerializeField]
    private CinemachineVirtualCamera vcam2; //room cam

    [SerializeField]

    private CinemachineVirtualCamera vcam3; // oni cam

    [SerializeField]
    private GameObject fadeOutCutsceneHolder;

    [SerializeField]
    private PlayableDirector cutsceneManager;

    public GameObject invisiWall;

    public GameObject oniHealthBar;

    public Collider2D touchBox;

    public bool canContinue = false;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private string ReturnSceneName;

    private OniFSMController oniBoss;
    private PlayerFSMController player;
    [SerializeField]
    private GameObject playerHUD;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        canContinue = false;
        oniBoss = FindObjectOfType<OniFSMController>();
        if (oniBoss != null)
        {
            oniBoss.OnOniBeginEnraged += StartOniEnragedCutscene;
            oniBoss.OnOniBeginBreak += StartOniBreakCutscene;
            oniBoss.OnOniBeginDeath += StartOniDeathCutscene;
        }
        player = FindObjectOfType<PlayerFSMController>();
    }

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            vcam1.Priority = 0;
            vcam2.Priority = 1;
            if (oniBoss != null)
            {
                StartCoroutine(IntroOniCutscene());
                StartCoroutine(FillHealthBar(oniBoss, waitTime));
            }
            else
            {
                StartCoroutine(FadeOutCutscene());
            }

        }
    }

    public void StartOniEnragedCutscene()
    {
        StartCoroutine(EnragedOniCutscene());
    }

    public IEnumerator IntroOniCutscene()
    {
        touchBox.enabled = false;
        invisiWall.gameObject.SetActive(true);
        oniHealthBar.gameObject.SetActive(true);
        playerHUD.SetActive(false);
        player.GetComponent<PlayerInput>().enabled = false;
        yield return new WaitForSeconds(waitTime);
        canContinue = true;
        oniBoss.OnOniBossStart?.Invoke();
        player.GetComponent<PlayerInput>().enabled = true;
        playerHUD.SetActive(true);
    }

    public IEnumerator EnragedOniCutscene()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        playerHUD.SetActive(false);
        yield return new WaitForSeconds(oniBoss.oniEnragedCutsceneHolder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        oniBoss.OnOniEndEnraged?.Invoke();
        player.GetComponent<PlayerInput>().enabled = true;
        playerHUD.SetActive(true);
    }

    private void StartOniBreakCutscene()
    {
        StartCoroutine(OniBreakCutscene());
    }

    private IEnumerator OniBreakCutscene()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        playerHUD.SetActive(false);
        vcam1.Priority = 0;
        vcam3.Priority = 1;
        yield return new WaitForSeconds(oniBoss.oniBreakCutsceneHolder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        oniBoss.OnOniEndBreak?.Invoke();
        vcam1.Priority = 1;
        vcam3.Priority = 0;
        player.GetComponent<PlayerInput>().enabled = true;
        playerHUD.SetActive(true);
    }

    private void StartOniDeathCutscene()
    {
        //StartCoroutine(OniDeathCutscene());
        cutsceneManager.Play();
    }

    private IEnumerator OniDeathCutscene()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        playerHUD.SetActive(false);
        oniHealthBar.gameObject.SetActive(false);
        oniBoss.oniDeathCutsceneHolder.GetComponent<Animator>().Play("EndCutscene");
        StatsTrackerScript.instance.OnLevelCompleted();
        StartCoroutine(OniFadeAway(oniBoss, oniBoss.oniDeathCutsceneHolder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length));
        yield return new WaitForSeconds(oniBoss.oniDeathCutsceneHolder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene(ReturnSceneName, LoadSceneMode.Single); // Return to the regular level;
    }

    private IEnumerator FillHealthBar(OniFSMController oni, float maxTime)
    {
        float timer = 0f;

        do
        {
            oni.SetHealth(Mathf.Lerp(0, oni.GetMaxHealth(), timer / maxTime));
            timer += Time.deltaTime;
            yield return null;
        } 
        while (timer < maxTime);
    }

    private IEnumerator OniFadeAway(OniFSMController oni, float maxTime)
    {
        float timer = 0f;
        SpriteRenderer oniSprite = oni.GetComponent<SpriteRenderer>();
        Color oniColor = oniSprite.color;

        do
        {
            oniColor.a = (Mathf.Lerp(1, 0, timer / maxTime));
            timer += Time.deltaTime;
            oniSprite.color = oniColor;
            yield return null;
        }
        while (timer < maxTime);
    }

    public IEnumerator FadeOutCutscene()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        playerHUD.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        fadeOutCutsceneHolder.gameObject.SetActive(true);
        fadeOutCutsceneHolder.GetComponent<Animator>().Play("FadeOut");
        StatsTrackerScript.instance.OnLevelCompleted();
        yield return new WaitForSeconds(fadeOutCutsceneHolder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene(ReturnSceneName, LoadSceneMode.Single); // Return to the regular level;

    }



}
