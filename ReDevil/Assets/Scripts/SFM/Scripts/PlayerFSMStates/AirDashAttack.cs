using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashAttack : FSMState
{

    private bool isGrounded; // just in case we account for any slanted platforms in the future
    private bool onWall;
    private float prevGravityScale;
    private float dashDistance;
    private bool endDash;
    private bool dashAttackStarted;

    public AirDashAttack()
    {
        Debug.Log("Create AirDash");
        stateID = FSMStateID.AirDashAttacking;

        // initialize variables
        isGrounded = false;
        dashAttackStarted = false;
        endDash = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        bool enterKnockback = pc.GetKbTransition(); // in the event we get hit by a projectile
        bool enterDashKnock = pc.GetDKBTransition();

        pc.UpdateState("Air Dash Attack");
        pc.TouchingFloorOrWall();

        if (!dashAttackStarted)
        {
            pc.SetCanDash(false);
            pc.SetDashInputAllowed(false);
            dashAttackStarted = true;
            endDash = false;

            prevGravityScale = pc.GetRigidbody2D().gravityScale;
            pc.GetRigidbody2D().gravityScale = 0;

            pc.SetDashStartPos(pc.transform.position);

            if (pc.moveVector.x > 0f)
            {
                pc.direction = 1;
                pc.facingLeft = false;

                //pc.FlipPlayer();
            } 
            else if (pc.moveVector.x < 0f)
            {
                pc.direction = -1;
                pc.facingLeft = false;

               // pc.FlipPlayer();
            }
            else
            {

            }
        }
        // total distance of dash... make a different length?
        Vector2 dashSP = pc.GetDashStartPos();

        dashDistance = Mathf.Abs(dashSP.x - pc.transform.position.x);
        pc.GetRigidbody2D().velocity = Vector2.right * pc.direction * pc.dashSpeed;
        pc.TouchingFloorOrWall();
        isGrounded = pc.GetisGrounded();
        onWall = pc.GetisTouchingWall();
        //Debug.Log("Dash Distance: " + dashDistance);
        //Debug.Log("Grounded??? -> " + isGrounded);
        //Debug.Log("On Wall??? -> " + onWall);

        // during dash
        if (!endDash)
        {
            if (dashDistance < pc.dashLength && !patk.airDashAttackContact)
            {
                patk.StartDashAttack();
            }
            else if (dashDistance >= pc.dashLength)
            {
                Debug.Log("Reached Air Dash Attack distance");
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }
            else if (patk.airDashAttackContact)
            {
                Debug.Log("Air Dash Attack Hit");
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }
            // TO ADD: break out function if we get stuck on one of the corners

            if (enterKnockback)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }
            if (onWall)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }
            // check for if we land on the ground mid dash? 
            // account for slanted platforms down the line?

            if (isGrounded)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }

            // we must create a condition where if the player is stuck on something, break out of the condition

        }

    }

    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        isGrounded = pc.GetisGrounded();
        onWall = pc.GetisTouchingWall();

        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();
        bool dkbTransition = pc.GetDKBTransition();

        // end of dash
        if (endDash)
        {
            Debug.Log("FinishDash");
            if (!invincible && kbTransition)
            {
                pc.PerformTransition(Transition.Knockback);
            }
            if (patk.airDashAttackContact)
            {
                pc.AirDashKnockback();
                pc.SetDKBTransition(true);
                pc.PerformTransition(Transition.DashKnockback); // still need to test this

            }

            // good chance we'll be hitting the wall from air dash attack
            if (onWall)
            {
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.WallSlide);
            }

            if (dashDistance >= pc.dashLength)
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
