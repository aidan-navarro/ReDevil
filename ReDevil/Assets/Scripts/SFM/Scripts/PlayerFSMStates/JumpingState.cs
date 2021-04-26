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

        bool grounded = pc.GetisGrounded();
        pc.UpdateState("Jump");

        if (grounded)
        {
            //Player has begun to jump
            Vector2 newVel = rig.velocity;
            newVel.y = pc.jumpPower;
            rig.velocity = newVel;

            Debug.Log("Player State: Jumping");
            hasJumped = true;
        }
        
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        //player has died.  transition to dead
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();

        //knockback transition
        if (!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }

        //attack transition
        if (Input.GetButtonDown("Attack") && pc.vertical <= -0.45f)
        {
            pc.PerformTransition(Transition.AirDownStrike);
        }

        if (pc.health <= 0)
        {
            Debug.Log("Transition to Dead");
            pc.PerformTransition(Transition.NoHealth);
        }

        if (hasJumped)
        {
            pc.PerformTransition(Transition.Airborne);
        }

    }
}
