using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleTimer : MonoBehaviour
{
    public float immobileTime; //the amount of time in seconds the player is immobile
    public float iFrameTime; //the number of iFrames AFTER being immobile in seconds
    public float dashKnockbackTimer;
    private PlayerFSMController pc;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerFSMController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Turn flamethrower specific items on in 
    public IEnumerator Timer()
    {
        pc.SetInvincible(true);
        pc.SetImmobile(true);
        gameObject.layer = LayerMask.NameToLayer("IFrames");


        //for extra time to move after flinching so that you can't immediately get hit after flinching
        yield return new WaitForSeconds(immobileTime);

        //immobile timer has expired.  player can now move but will remain invinicble for a bit longer
        pc.SetImmobile(false);

        //begin timer for extra seconds on invincibility
        yield return new WaitForSeconds(iFrameTime);


        //invincibility has ended.  Player can now be hit again
        pc.SetInvincible(false);
        pc.SetFlameKB(false);
        //invincibility has ended.  allow knockback to be dealt again
        gameObject.layer = LayerMask.NameToLayer("Default");

    }

    // Testing purposes, timer for the dash attack
    public IEnumerator DashKnockbackTimer()
    {
        // get a delay timer set in the player controller?
        // serialize this???
        yield return new WaitForSeconds(dashKnockbackTimer);
        pc.SetDKBTransition(false);
    }
}
