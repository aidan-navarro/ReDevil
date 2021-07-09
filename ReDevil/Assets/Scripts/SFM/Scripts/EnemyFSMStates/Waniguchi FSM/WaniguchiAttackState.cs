using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniguchiAttackState : FSMState
{
    //bool hasLanded;

    bool isGrounded;
    bool atkStart; // utilize attack start for 

    //Constructor
    public WaniguchiAttackState()
    {
        stateID = FSMStateID.WaniguchiAttacking;
        //hasLanded = false;

        isGrounded = false;
        atkStart = false;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        WaniguchiFSMController ec = npc.GetComponent<WaniguchiFSMController>();
        Animator anim = ec.GetComponent<Animator>();
        ec.TouchingFloor();
        isGrounded = ec.GetisGrounded(); 
        
        //reset variables
        //if (hasLanded)
        //{
        //    hasLanded = false;
        //}
        if (atkStart && !ec.attacking) // reset
        {
            atkStart = false;
        }

        // this is fine
        if (!atkStart)
        {
            //set an initial velocity for the waniguchi to jump
            ec.WaniguchiAttack();
            atkStart = true;

            ec.attacking = atkStart;
        }

       
        //if(atkStart) // here's the issue right now
        //{
        //    ec.TouchingFloor();  // this is getting called first.  NEed to check instead if we're going into the midair state, then do THIS check in that state
        //    //if waniguchi has landed
        //    if(ec.GetisGrounded())
        //    {
        //ec.TouchingFloor();
        //bool isGrounded = ec.GetisGrounded();

        ////ec.SetCurrentPos(ec.rig.position); // all we've set is the position, we haven't done anything with the velocity
        ////ec.rig.position = ec.GetCurrentPos();
        //// ec.attacking = false;
        //hasLanded = ec.GetisGrounded();
        //    }
        //}

    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        WaniguchiFSMController ec = npc.GetComponent<WaniguchiFSMController>();
        Animator anim = ec.GetComponent<Animator>();

        // new change: set the attacking method to transition over to airborne
        ec.TouchingFloor();
        isGrounded = ec.GetisGrounded();
        //Debug.Log("GroundCheck: " + hasLanded);
        
        if (atkStart && !isGrounded) // this is not getting called
        {
            Debug.Log("Waniguchi jump: " + atkStart); // this is getting called too quickly
            ec.PerformTransition(Transition.WaniguchiAirborne);
            atkStart = false;
        } else
        {
            Debug.Log("Not going into the Airborne State");
        }

        if (ec.GetEnemyFlinch() && ec.health > 0)
        {
            Debug.Log("flinch Transition");
            anim.SetBool("Flinch", ec.GetEnemyFlinch());
            anim.Play("Flinch", -1, 0.0f);
            ec.PerformTransition(Transition.WaniguchiFlinch);
        }

        //dead transition
        else if (ec.health <= 0)
        {

            ec.PerformTransition(Transition.WaniguchiDead);
        }
    }
}
