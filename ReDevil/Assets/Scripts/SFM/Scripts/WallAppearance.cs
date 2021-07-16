using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAppearance : MonoBehaviour
{
    public bool characterIn;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            characterIn = true;
            animator.SetBool("PlayerIn", true);

            //Raquel if you see this code this is what I was talking about doing for your pickup items
            //Make a temporary variable (PlayerFSMController pc)
            //and set its value to be the object you just confirmed is the player you collided with
            //Then call its function
            //PlayerFSMController pc = other.GetComponent<PlayerFSMController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            characterIn = false;
            animator.SetBool("PlayerIn", false);

            //Raquel if you see this code this is what I was talking about doing for your pickup items
            //Make a temporary variable (PlayerFSMController pc)
            //and set its value to be the object you just confirmed is the player you collided with
            //Then call its function
            //PlayerFSMController pc = other.GetComponent<PlayerFSMController>();
        }
    }
}
