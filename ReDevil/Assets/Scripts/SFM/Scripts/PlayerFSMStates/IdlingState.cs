﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlingState : FSMState
{

    //Constructor
    public IdlingState()
    {
        stateID = FSMStateID.Idling;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();

        //stop momentum movement
        Vector2 newMoveSpeed = Vector2.zero;
        newMoveSpeed.y = rig.velocity.y;
        rig.velocity = newMoveSpeed;

        pc.TouchingFloorOrWall();

        pc.CheckDashInput();
        pc.UpdateState("Idle");
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        pc.horizontal = Input.GetAxis("Horizontal");
        pc.vertical = Input.GetAxis("Vertical");

        bool grounded = pc.GetisGrounded();
        bool cD = pc.GetCanDash();
        bool dashAllowed = pc.GetDashInputAllowed();
        bool onWall = pc.GetisTouchingWall();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();

        //knockback transition
        if(!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }

        //attack transition
        if (Input.GetButtonDown("Attack") && grounded)
        {
            pc.PerformTransition(Transition.GroundAttack1);
        }

        //dash transition
        // to add: Implement the ability to do the ground dash

        if ((pc.leftTriggerDown || pc.rightTriggerDown) && cD && dashAllowed)
        {
            if (pc.GetisGrounded())
            {
                pc.PerformTransition(Transition.DashAttack);
            }
            else
            {
                pc.PerformTransition(Transition.Dash);
            }
        }

        // ground dash transition

        //jump transition
        if (Input.GetButtonDown("Jump") && onWall)
        {
            pc.PerformTransition(Transition.WallJump);
        }
        else if (Input.GetButtonDown("Jump"))
        {
            pc.PerformTransition(Transition.Jump);
        }

        //move transition
        else if (pc.horizontal < 0f || pc.horizontal > 0f)
        {
            pc.PerformTransition(Transition.Move);
        }

        

        //dead transition
        if (pc.health <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }



    }
}
