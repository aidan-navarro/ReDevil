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
        //Debug.Log("Idling");
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        //set friction material
        pc.SlopeCheck();
        pc.SetFrictionMaterial();     

        patk.firstDashContact = false;
        pc.ResetAirDashCount(); // any time we return to idle state, this count gets reset
        pc.UpdateDashIcons();
        pc.TouchingFloorCeilingWall();
        pc.CheckAirDash(); // to reset the air dash
        pc.TouchingInvisibleWall();

        pc.SetKbTransition(false);
        //pc.CheckDashInput();
        pc.UpdateState("Idle");
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();

        bool grounded = pc.GetisGrounded();
        bool cD = pc.GetCanDash();
        bool dashAllowed = pc.GetDashInputAllowed();
        bool onWall = pc.GetisTouchingWall();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();
        bool attackButtonDown = pc.GetAttackButtonDown();
        bool jumpButtonDown = pc.GetJumpButtonDown();
        bool soulAttackButtonDown = pc.GetSoulAttackButtonDown();

        //knockback transition
        if(!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }

        //attack transition
        if (attackButtonDown && grounded)
        {
            pc.PerformTransition(Transition.GroundAttack1);
        }

        if (soulAttackButtonDown && pc.GetSoul() >= player.GetComponent<PlayerAttack>().soulShot.GetComponent<SoulShot>().soulCost)
        {
            pc.PerformTransition(Transition.SoulShot);
        }

        //dash transition
        // to add: Implement the ability to do the ground dash

        if ((pc.leftTriggerDown || pc.rightTriggerDown) && cD && dashAllowed)
        {
            // ground dash transition, if the player is on the ground, register a dash attack
            if (pc.GetisGrounded())
            {
                pc.PerformTransition(Transition.DashAttack);
            }
            //else
            //{
            //    pc.PerformTransition(Transition.Dash);
            //}
        }


        //jump transition
        if (jumpButtonDown && onWall)
        {
            pc.PerformTransition(Transition.WallJump);
        }
        else if (jumpButtonDown)
        {
            pc.PerformTransition(Transition.Jump);
        }

        //move transition
        else if (pc.moveVector.x < 0f || pc.moveVector.x > 0f)
        {
            pc.PerformTransition(Transition.Move);
        }

        //airborne transition since idle state is the default.  In case we start level in midair.
        if(!grounded)
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
