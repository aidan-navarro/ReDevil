using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulPickUp : MonoBehaviour
{
    public int value;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
           //when dealing with collisions, if we're checking for a specific thing,
           //make a temporary variable (left of the equal sign)
           //and make it equal whatever element we want from what we collided with
            PlayerFSMController pc = collision.GetComponent<PlayerFSMController>();
            pc.AddSoul(value);
            Destroy(gameObject);
        }
    }
}
