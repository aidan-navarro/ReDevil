﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer's notes:
// This script will be used to 
public class GroundDashAttack : FSMState
{

    // state variables
    private bool isGrounded; // need? the check to get in this transition would be if it was grounded already
    private bool onWall; // if the player hits the wall any time during the update
    private float prevGravityScale;
    float dashDistance;
    private bool endDash;
    private bool dashAttackStarted; // bool to check if the attack hitbox has started


    public GroundDashAttack()
    {
        // calling the constructor in the Player FSMController class
        // make a state ID for the ground dash, 
        stateID = FSMStateID.DashAttacking;

        // initialize any grounded variables
        isGrounded = false;

        dashAttackStarted = false;
        endDash = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        //Debug.Log("Initiate Ground Dash Attack");
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>(); // access the rigid body component attached to the player
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        // get the player's input 
        //pc.horizontal = Input.GetAxis("Horizontal");
        //pc.vertical = Input.GetAxis("Vertical");

        bool enterKnockback = pc.GetKbTransition(); 
        // should find a new state for knockback off of grounded dash attack

        pc.UpdateState("Ground Dash Attack");

        pc.TouchingFloorOrWall();

        if (!dashAttackStarted)
        {
            pc.SetCanDash(false);
            pc.SetDashInputAllowed(false);
            dashAttackStarted = true; // so that we don't trigger this again
            endDash = false;

            // store the value of gravity... though this is on the ground so just in case
            prevGravityScale = pc.GetRigidbody2D().gravityScale; 
            pc.GetRigidbody2D().gravityScale = 0;

            pc.SetDashStartPos(pc.transform.position); // utilize some of the existing dash logic

            if (pc.moveVector.x > 0f)
            {
                pc.direction = 1;
                pc.facingLeft = false;


                pc.FlipPlayer();
            }
            else if (pc.moveVector.x < 0f)
            {
                pc.direction = -1;
                pc.facingLeft = true;

                pc.FlipPlayer();

            }
            else //no input detected.  stop speed and set bool to not moving to transition to idle
            {
                //dont change direction
            }
        }

        Vector2 dashSP = pc.GetDashStartPos();

        // dashing total distance
        dashDistance = Mathf.Abs(dashSP.x - pc.transform.position.x);
        pc.GetRigidbody2D().velocity = Vector2.right * pc.direction * pc.dashSpeed; // commit to the dash
        onWall = pc.GetisTouchingWall();


        // logic to end the dashing
        // if (!endDash) means that the dash is still in process
        // and the following steps within this if statement are what can break out of this condition
        if (!endDash)
        {
            // during the middle of the dash check if the 
            // pseudo code
            // if (dash hitbox contact)
            // patk.EndDashAttack()
            if (dashDistance < pc.dashLength)
            {
                patk.StartDashAttack();
            }
            //dashed max distance, end the dash.
            else if (dashDistance >= pc.dashLength)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                patk.EndDashAttack();
                dashAttackStarted = false;
                endDash = true;
            }

            //tweak this maybe, because I may implement a different knockback
            // this still enters the knockback state interestingly enough
            if (enterKnockback)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                patk.EndDashAttack();
                dashAttackStarted = false;
                endDash = true;
            }

            //hit a wall.  end the dash
            if (onWall)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                patk.EndDashAttack();
                dashAttackStarted = false;
                endDash = true;
            }
        }
    }

    // basic reasoning logic for the transitions exiting ground dahs
    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D m_rb = player.GetComponent<Rigidbody2D>(); // attached physics body
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>(); // to reset the transitions out of dash attack
        //pc.horizontal = Input.GetAxis("Horizontal");
        //pc.vertical = Input.GetAxis("Vertical");


        isGrounded = pc.GetisGrounded();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();

        if (endDash)
        {
            if (!invincible && kbTransition)
            {
                Debug.Log("Enter Knockback");
                pc.PerformTransition(Transition.Knockback);
            }

            // just in case
            if (onWall)
            {
                pc.PerformTransition(Transition.WallSlide);
            }

            // moving transition
            if (pc.moveVector.x > 0 || pc.moveVector.x < 0)
            {
                pc.PerformTransition(Transition.Move);
            }

            if (isGrounded && (dashDistance >= pc.dashLength))
            {
                Debug.Log("End Ground Dash");
                pc.PerformTransition(Transition.Idle);
            }
            else if (isGrounded && onWall)
            {
                Debug.Log("End Ground Dash Hit Wall");
                pc.PerformTransition(Transition.Idle);
            }

        }

        // dead transition at any point
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
