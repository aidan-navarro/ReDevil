﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//state for moving ON THE GROUND.  REFERENCE A DIFFERENT STATE FOR AIR MOVEMENT
public class MoveState : FSMState
{
    private bool isMoving;

    //Constructor
    public MoveState()
    {
        stateID = FSMStateID.Moving;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        isMoving = true;

        pc.TouchingFloorOrWall();
        //pc.CheckDashInput();

        pc.UpdateState("Moving");

        if (pc.moveVector.x > 0f)
        {
            pc.direction = 1;
            pc.facingLeft = false;
            Vector2 newMoveSpeed = Vector2.right * pc.GetMoveSpeed();
            newMoveSpeed.y = rig.velocity.y;

            rig.velocity = newMoveSpeed;

            pc.FlipPlayer();
        }
        else if (pc.moveVector.x < 0f)
        {
            pc.direction = -1;
            pc.facingLeft = true;
            Vector2 newMoveSpeed = Vector2.left * pc.GetMoveSpeed();
            newMoveSpeed.y = rig.velocity.y;

            rig.velocity = newMoveSpeed;

            pc.FlipPlayer();

        }
        else //no input detected.  stop speed and set bool to not moving to transition to idle
        {
            //stop momentum movement
            Vector2 newMoveSpeed = Vector2.zero;
            newMoveSpeed.y = rig.velocity.y;
            rig.velocity = newMoveSpeed;
            isMoving = false;
        }
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();

        bool grounded = pc.GetisGrounded();
        bool cD = pc.GetCanDash();
        bool dashAllowed = pc.GetDashInputAllowed();
        bool onWall = pc.GetisTouchingWall();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();
        bool jumpButtonDown = pc.GetJumpButtonDown();

        //knockback transition
        if (!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }

        //dash transition
        if ((pc.leftTriggerDown || pc.rightTriggerDown) && cD && dashAllowed)
        {
            pc.PerformTransition(Transition.Dash);
        }

        //idle transition
        if (!isMoving)
        {
            pc.PerformTransition(Transition.Idle);
        }

        //jump transition
        //jump transition
        if (jumpButtonDown && onWall)
        {
            pc.PerformTransition(Transition.WallJump);
        }
        else if (jumpButtonDown)
        {
            pc.PerformTransition(Transition.Jump);
        }

        //airborne transition when walking off an edge
        if (!grounded)
        {
            pc.PerformTransition(Transition.Airborne);
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }

    }
}
