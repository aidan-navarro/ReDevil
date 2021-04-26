using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : FSMState
{
    private bool hasJumped;

    //Constructor
    public WallJumpState()
    {
        stateID = FSMStateID.WallJumping;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();


            //Player has begun to jump
            Vector2 newVel = pc.rig.velocity;
            newVel.y = pc.jumpPower;
            newVel.x = (pc.jumpPower / 2) * -pc.direction;
            pc.rig.velocity = newVel;

        pc.UpdateState("Wall Jumping");
        hasJumped = true;

    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();

        //knockback transition
        if (!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }


        if (hasJumped)
        {
            pc.PerformTransition(Transition.Airborne);
        }

        //dead transition
        if (pc.health <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
