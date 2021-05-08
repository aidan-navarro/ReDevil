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
        }
    }
}
