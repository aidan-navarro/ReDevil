using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashAttack : FSMState
{

    private bool isGrounded; // just in case we account for any slanted platforms in the future
    private bool onWall;
    private bool isCeiling;
    private float prevGravityScale;
    private float dashDistance;
    private bool endDash;
    private bool dashAttackStarted;
    private bool touchingInvisWall;

    // force breakout timer
    private float dashTimer;
    private Vector2 posCheck;

    public AirDashAttack()
    {
        stateID = FSMStateID.AirDashAttacking;

        // initialize variables
        isGrounded = false;
        dashAttackStarted = false;
        endDash = false;
        dashTimer = 0;
    }

    public override void Act(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();
        Animator anim = pc.GetComponent<Animator>();

        posCheck = player.position;

        // flamethrower specific 
        // pc.SetKbTransition(false);
        pc.SetFlameKB(false);

        bool enterKnockback = pc.GetKbTransition(); // in the event we get hit by a projectile
        bool enterDashKnock = pc.GetDKBTransition();

        pc.UpdateState("Air Dash Attack");

        // Track Player conditions
        pc.UpdateDashIcons();
        pc.TouchingFloorCeilingWall();
        pc.TouchingInvisibleWall();

        if (!dashAttackStarted)
        {
            pc.SetCanDash(false);
            pc.SetDashInputAllowed(false);
            dashAttackStarted = true;
            endDash = false;
            //Debug.Log("Normal Midair Behaviour");
            if (pc.moveVector.x > 0f)
            {
                pc.direction = 1;
                pc.facingLeft = false;
            }
            else if (pc.moveVector.x < 0f)
            {
                pc.direction = -1;
                pc.facingLeft = true;

            }
            prevGravityScale = pc.GetRigidbody2D().gravityScale;
            pc.GetRigidbody2D().gravityScale = 0;
            anim.SetBool("Dashing", dashAttackStarted);
            pc.SetDashStartPos(pc.transform.position);
         
        }
        // total distance of dash... make a different length?

        // TO DO: replace this current logic 
        Vector2 dashSP = pc.GetDashStartPos();

        // figure out the dash distance for aerial dashing
        // turn dash distance into a magnitude instead of simply an x check

        Vector2 playerPos = new Vector2(pc.transform.position.x, pc.transform.position.y);
        Vector2 dashDiff = dashSP - playerPos;

        // dashDistance = Mathf.Abs(dashSP.x - pc.transform.position.x);
        // instead of checking the x distance, we're instead checking the whole magnitude of the vector
        dashDistance = UsefullFunctions.Vec2Magnitude(dashDiff);
        //Debug.Log("Dash Path -> " + pc.GetDashPath());

        // velocity must also change to account for the dash position that we set
        // create a boolean to lock any change to the dash vector while we dash
        pc.GetRigidbody2D().velocity = pc.GetDashPath() * pc.dashSpeed;
        isGrounded = pc.GetisGrounded();
        onWall = pc.GetisTouchingWall();
        isCeiling = pc.GetisTouchingCeiling();
     
        // during dash
        if (!endDash)
        {
            dashTimer += Time.deltaTime;
            Debug.Log("DashTimer: " + dashTimer);
            // checking the square magnitude of the dash distance, to circumvent a sqrt check
            if (dashDistance < pc.dashLength * pc.dashLength && !patk.airDashAttackContact)
            {
                Vector2 vAbsDash = pc.GetDashPath();
                vAbsDash.x = Mathf.Abs(vAbsDash.x);
                patk.StartAirDashAttack(vAbsDash);
            }

            // Conditions to conclude the dash
            else if (dashDistance >= pc.dashLength * pc.dashLength)
            {
                //Debug.Log("Reached Air Dash Attack distance");
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }
            else if (patk.airDashAttackContact)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }

            if (enterKnockback)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }
            else if (onWall)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                patk.didAirAttack = false;
                endDash = true;
                patk.EndDashAttack();
            }
            // check for if we land on the ground mid dash? 
            // account for slanted platforms down the line?

            else if (isGrounded)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                patk.didAirAttack = false;
                endDash = true;
                patk.EndDashAttack();
            }

            else if (touchingInvisWall)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                patk.EndDashAttack();
                dashAttackStarted = false;
                endDash = true;
            }

            else if (isCeiling)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                patk.EndDashAttack();
                dashAttackStarted = false;
                endDash = true;
            }

           

            //// we must create a condition where if the player is stuck on something, break out of the condition
            //if (dashTimer >= 2)
            //{
            //    pc.SetCanDash(true);
            //    pc.GetRigidbody2D().gravityScale = prevGravityScale;
            //    dashAttackStarted = false;
            //    endDash = true;
            //    patk.EndDashAttack();
            //}
        }

    }

    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();
        Animator anim = pc.GetComponent<Animator>();
        isGrounded = pc.GetisGrounded();
        onWall = pc.GetisTouchingWall();
        touchingInvisWall = pc.GetisTouchingInvisibleWall();

        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();
        bool dkbTransition = pc.GetDKBTransition();

        // end of dash
        if (endDash)
        {
            anim.SetInteger("DashDirection", 0);

            if (!invincible && kbTransition)
            {
                pc.PerformTransition(Transition.Knockback);
            }
            if (patk.airDashAttackContact)
            {
                // check the angle in which the player makes contact with an enemy
                Vector2 checkAtkVector = patk.GetNormalizedAttackVector();
                // vertical hit angle (top or bottom)
                if (Mathf.Abs(checkAtkVector.y) > Mathf.Abs(checkAtkVector.x))
                {
                    Debug.Log("Hitting from the top or bottom");
                    // if the enemy is overhead 
                    if (checkAtkVector.y > 0.0f)
                    {
                        if (!patk.airDashKillingBlow)
                        {
                            pc.AirDashBottomKnockback2(pc.GetDashPath());
                        }
                        else
                        {
                            pc.AirDashKnockback();

                        }
                    }
                    // if the player is overhead, use the regular Dash Knockback function, it's modified to account for off the ground contact
                    else
                    {
                        pc.DashKnockback();
                    }
                } 
                // hitting the enemy from the side
                else
                {
                    // if the first hit of the air dash attack hasn't hit yet
                    if (!patk.airDashKillingBlow)
                    {
                        if (!patk.firstDashContact)
                        {
                            pc.AirDashKnockback();
                            patk.firstDashContact = true;
                        }
                        else
                        {
                            pc.SideDashKnockback(pc.GetDashPath());
                        }
                    } else
                    {
                        pc.AirDashKnockback();
                    }

                }
                pc.SetDKBTransition(true);
                pc.PerformTransition(Transition.DashKnockback); 

            }

            // good chance we'll be hitting the wall from air dash attack
            if (onWall)
            {
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.WallSlide);
            }

            if (isCeiling)
            { 
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Airborne);
            }
            //if we touch an invisible wall, we should not wall slide, put on airborne
            if (touchingInvisWall)
            {
                Debug.Log("Touching Invisible Wall");
                patk.didAirAttack = false;
                pc.PerformTransition(Transition.Airborne);

            }

            // checking the square magnitude of the dash distance, to circumvent a sqrt check
            if (dashDistance >= pc.dashLength * pc.dashLength)
            {
                //Debug.Log("ReachAirDashDistance");
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Airborne);
            }
            if (isGrounded)
            {
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Idle);
            }
        }

        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
