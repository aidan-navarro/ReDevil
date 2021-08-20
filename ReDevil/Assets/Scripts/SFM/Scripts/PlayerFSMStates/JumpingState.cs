using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : FSMState
{
    private bool hasJumped;

    //Constructor
    public JumpingState()
    {
        stateID = FSMStateID.Jumping;
        hasJumped = false;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        Animator anim = pc.GetComponent<Animator>();
        anim.SetBool("ResetIdle", false);
        pc.SetNoFrictionMaterial();

        pc.TouchingFloorCeilingWall();
        bool grounded = pc.GetisGrounded();

        pc.UpdateState("Jump");

        if(grounded)
        {
            ////Player has begun to jump
            //// SET THIS IN THE ANIMATION
            //Vector2 newVel = rig.velocity;
            //newVel.y = pc.GetJumpPower();
            //rig.velocity = newVel;

            //pc.soundManager.PlayJump();
            //Debug.Log("Player State: Jumping");
            hasJumped = true;
            anim.SetTrigger("Jump");
            anim.SetInteger("DashDirection", 0);


        }

    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        //player has died.  transition to dead
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();
        bool grounded = pc.GetisGrounded();

        //knockback transition
        if (!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }
        if (hasJumped && !grounded)
        {
            pc.PerformTransition(Transition.Airborne);
        }
        //attack transition
        // note: vertical has not been used yet
        //       Vertical is being used when the analog stick is being pointed downward.
        if (pc.GetAttackButtonDown() && pc.moveVector.y <= -0.45f) 
        {
            pc.PerformTransition(Transition.AirDownStrike);
        }

        if (pc.GetHealth() <= 0)
        {
            Debug.Log("Transition to Dead");
            pc.PerformTransition(Transition.NoHealth);
        }

    }

}
