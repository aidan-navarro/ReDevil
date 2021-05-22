using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlideState : FSMState
{
    //Constructor
    public WallSlideState()
    {
        stateID = FSMStateID.WallSliding;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();

        pc.TouchingFloorOrWall();
        Vector3 wallInitPos = pc.transform.position;


        if (rig.velocity.y < 0f)
        {
            Vector2 newVel = rig.velocity;
            newVel.y = -pc.GetMoveSpeed() / pc.slideSpeed;
            rig.velocity = newVel;
        }

        patk.firstDashContact = false;
        // touching the wall will reset the air dash count
        pc.ResetAirDashCount();
        pc.CheckAirDash();

        pc.UpdateState("Wall Sliding");
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();

        bool grounded = pc.GetisGrounded();
        bool onWall = pc.GetisTouchingWall();
        int currentDir = pc.direction;
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();

        //knockback transition
        if (!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }


        //jump transition
        if (pc.GetJumpButtonDown())
        {
            pc.PerformTransition(Transition.WallJump);
        }

        //transition to airborne, just letting go of the wall
        if ((pc.moveVector.x < 0f && currentDir == 1) || (pc.moveVector.x > 0f && currentDir == -1) || !onWall)
        {
            pc.PerformTransition(Transition.Airborne);
        }

        if (grounded && onWall)
        {
            pc.PerformTransition(Transition.Idle);
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
