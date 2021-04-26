using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This state is for midair movement
public class MidairState : FSMState
{
    //Constructor
    public MidairState()
    {
        stateID = FSMStateID.Midair;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("State ID: Midair");
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        pc.horizontal = Input.GetAxis("Horizontal");
        pc.vertical = Input.GetAxis("Vertical");

        pc.UpdateState("In Midair");

        //first check if we are touching a wall or floor
        pc.TouchingFloorOrWall();

        //check if a dash is input
        pc.CheckDashInput();

        if (pc.horizontal > 0f)
        {
            pc.direction = 1;
            pc.facingLeft = false;
            Vector2 newMoveSpeed = Vector2.right * pc.moveSpeed;
            newMoveSpeed.y = rig.velocity.y;

            rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * pc.airControl);

            pc.FlipPlayer();
        }
        else if (pc.horizontal < 0f)
        {
            pc.direction = -1;
            pc.facingLeft = true;
            Vector2 newMoveSpeed = Vector2.left * pc.moveSpeed;
            newMoveSpeed.y = rig.velocity.y;

            rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * pc.airControl);

            pc.FlipPlayer();

        }
        else //no input detected.  stop speed and set bool to not moving to transition to idle
        {
            //stop momentum movement
            Vector2 newMoveSpeed = Vector2.zero;
            newMoveSpeed.y = rig.velocity.y;
            rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * pc.airControl);
        }
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        pc.horizontal = Input.GetAxis("Horizontal");
        pc.vertical = Input.GetAxis("Vertical");

        bool grounded = pc.GetisGrounded();
        bool onWall = pc.GetisTouchingWall();
        bool cD = pc.GetCanDash();
        bool dashAllowed = pc.GetDashInputAllowed();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();

        //knockback transition
        if (!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }

        //attack transition
        if (Input.GetButtonDown("Attack") && pc.vertical <= -0.45f )
        {
            pc.PerformTransition(Transition.AirDownStrike);
        }

        //dash transition
        if ((pc.leftTriggerDown || pc.rightTriggerDown) && cD && dashAllowed)
        {
            pc.PerformTransition(Transition.Dash);
        }


        if (onWall)
        {
            pc.PerformTransition(Transition.WallSlide);
        }

        //have to track if we are not touching a wall as well because of some weird ass bug where if you latch onto a wall
        //sometimes it also determines your touching the ground
        else if (grounded)
        {
            pc.PerformTransition(Transition.Idle);
        }

        //dead transition
        if (pc.health <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
