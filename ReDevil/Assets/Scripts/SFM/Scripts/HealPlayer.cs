using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{
    public int healValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerFSMController pc = other.GetComponent<PlayerFSMController>();
            pc.HealPlayer(healValue);
            pc.UpdateHealthHud();
            Destroy(gameObject);
        }
    }
}
