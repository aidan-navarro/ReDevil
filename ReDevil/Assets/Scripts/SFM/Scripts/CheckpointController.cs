using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public Sprite cpNotTriggered;
    public Sprite cpTriggered;
    private SpriteRenderer cpSpriteRenderer;
    public bool cpReached;

    // Start is called before the first frame update
    void Start()
    {
        cpSpriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            cpSpriteRenderer.sprite = cpTriggered;
            cpReached = true;

            //Raquel if you see this code this is what I was talking about doing for your pickup items
            //Make a temporary variable (PlayerFSMController pc)
            //and set its value to be the object you just confirmed is the player you collided with
            //Then call its function
            //PlayerFSMController pc = other.GetComponent<PlayerFSMController>();
        }
    }
}
