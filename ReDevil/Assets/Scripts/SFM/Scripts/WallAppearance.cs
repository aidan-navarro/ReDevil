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
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            characterIn = false;
            animator.SetBool("PlayerIn", false);
        }
    }
}
