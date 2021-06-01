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
    private bool playerCamera = true;

    private void Awake()
    {
       // animator = GetComponent<Animator>();
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
       // action.performed += _ => SwitchPriority();
    }

   //private void SwitchState()
   //{
   //    if (playerCamera)
   //    {
   //        animator.Play("BossRoomCam");
   //    }
   //    else
   //    {
   //        animator.Play("PlayersCam");
   //    }
   //    playerCamera = !playerCamera;
   //}

   //private void SwitchPriority()
   //{
   //    if (playerCamera)
   //    {
   //        vcam1.Priority = 0;
   //        vcam2.Priority = 1;
   //    }
   //    else
   //    {
   //        vcam1.Priority = 1;
   //        vcam2.Priority = 0;
   //    }
   //    playerCamera = !playerCamera;
   //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //when dealing with collisions, if we're checking for a specific thing,
            //make a temporary variable (left of the equal sign)
            //and make it equal whatever element we want from what we collided with
            PlayerFSMController pc = collision.GetComponent<PlayerFSMController>();
            vcam1.Priority = 0;
            vcam2.Priority = 1;
        }
    }
}
