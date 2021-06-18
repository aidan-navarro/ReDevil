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
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerFSMController pc = collision.GetComponent<PlayerFSMController>();
            vcam1.Priority = 0;
            vcam2.Priority = 1;
            invisiWall.gameObject.SetActive(true);
        }
    }
}
