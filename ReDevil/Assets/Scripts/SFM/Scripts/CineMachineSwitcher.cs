using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class CineMachineSwitcher : MonoBehaviour
{

    [SerializeField]
    private InputAction action;

    [SerializeField]
    private CinemachineVirtualCamera vcam1; //player cam

    [SerializeField]
    private CinemachineVirtualCamera vcam2; //room cam

    private Animator animator;
    [SerializeField]
    private bool playerCamera = true;

    public GameObject invisiWall;

    public GameObject oniHealthBar;

    public Collider2D touchBox;

    public bool canContinue = false;
    public float waitTime;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerFSMController pc = collision.GetComponent<PlayerFSMController>();
            vcam1.Priority = 0;
            vcam2.Priority = 1;
            StartCoroutine("Cutscene");
            StartCoroutine(FillHealthBar(FindObjectOfType<OniFSMController>(), waitTime));

        }
    }

    public IEnumerator Cutscene()
    {
        touchBox.enabled = false;
        invisiWall.gameObject.SetActive(true);
        oniHealthBar.gameObject.SetActive(true);
        FindObjectOfType<PlayerFSMController>().GetComponent<PlayerInput>().enabled = false;
        yield return new WaitForSeconds(waitTime);
        canContinue = true;
        FindObjectOfType<OniFSMController>().OnOniBossStart?.Invoke();
        FindObjectOfType<PlayerFSMController>().GetComponent<PlayerInput>().enabled = true;
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

}
