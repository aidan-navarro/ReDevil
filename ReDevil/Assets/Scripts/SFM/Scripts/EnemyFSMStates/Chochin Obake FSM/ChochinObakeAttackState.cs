using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChochinObakeAttackState : FSMState
{
    bool bulletFired;
   
    //Constructor
    public ChochinObakeAttackState()
    {
        stateID = FSMStateID.ChochinObakeAttacking;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        bulletFired = false;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        ChochinObakeFSMController ec = npc.GetComponent<ChochinObakeFSMController>();
        Animator anim = ec.GetComponent<Animator>();

        if (!bulletFired)
        {
            Vector2 bulletDirection = pc.GetRigidbody2D().position - ec.currentPos;
            Vector2 bulletDirectionNormalized = bulletDirection.normalized;
            if(bulletDirection.x < 1 && bulletDirection.x > -1)
            {
                //do nothing.  maybe anim for enemy not seeing the player
                Debug.Log("Can't see the player");
            }
            else
            {
                // plays the animation for firing, the function to instantiate a bullet is tied to the animation

                ec.UpdatePlayerPos(bulletDirectionNormalized);
                anim.SetTrigger("Firing");
                //Debug.Log(bulletDirection);
                //ec.InstantiateProjectile(ec.bullet, ec.firepoint.position, ec.firepoint.rotation, bulletDirectionNormalized, ec.projectileSpeed);
            }
            
        }

        bulletFired = true;

    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        ChochinObakeFSMController ec = npc.GetComponent<ChochinObakeFSMController>();

        if (bulletFired)
        {
            ec.PerformTransition(Transition.ChochinObakeIdle);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.ChochinOkabeDead);
        }
    }
}
