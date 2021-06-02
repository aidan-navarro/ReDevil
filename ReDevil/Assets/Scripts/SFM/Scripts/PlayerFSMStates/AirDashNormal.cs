using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashNormal : FSMState
{
    //state variables
    private bool isGrounded;
    private bool onWall;
    private bool dashStarted;
    private float prevGravityScale;
    private float dashDistance;
    private bool dashEnded;
    private bool floorOrWall;

    public AirDashNormal()
    {
        stateID = FSMStateID.AirDashing;
        dashStarted = false;
        dashEnded = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();

        bool enterKnockback = pc.GetKbTransition();
        pc.UpdateState("Air Dash Cancel");

        // check to touch a wall or ground to properly transition out of state
        pc.TouchingFloorCeilingWall();
        onWall = pc.GetisTouchingWall();
        isGrounded = pc.GetisGrounded();

        if(!dashStarted)
        {
            pc.SetCanDash(false);
            pc.SetDashInputAllowed(false);

            dashStarted = true;
            dashEnded = false;

            prevGravityScale = pc.GetRigidbody2D().gravityScale;

            pc.GetRigidbody2D().gravityScale = 0f;

            pc.SetDashStartPos(pc.transform.position);

            //determine direction of the dash.  Only change direction and facing left variables
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
        }

        Vector2 dashSP = pc.GetDashStartPos();
        Vector2 playerPos = new Vector2(pc.transform.position.x, pc.transform.position.y);
        Vector2 dashDiff = dashSP - playerPos;

        dashDistance = UsefullFunctions.Vec2Magnitude(dashDiff);

        pc.GetRigidbody2D().velocity = pc.GetDashPath() * pc.dashSpeed;

        if (!dashEnded)
        {
            if (dashDistance >= pc.dashLength * pc.dashLength)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashStarted = false;
                dashEnded = true;
            }
            if (enterKnockback)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashStarted = false;
                dashEnded = true;
            }
            if (onWall)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashStarted = false;
                dashEnded = true;
            }
            if (isGrounded)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashStarted = false;
                dashEnded = true;
            }
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();

        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();

        if (dashEnded)
        {
            if (!invincible && kbTransition)
            {
                pc.PerformTransition(Transition.Knockback);
            }
            if (onWall)
            {
                pc.PerformTransition(Transition.WallSlide);
            }

            // specific case
            if (isGrounded && onWall)
            {
                pc.PerformTransition(Transition.Idle);
            }

            if (isGrounded)
            {
                pc.PerformTransition(Transition.Idle);
            }
            else if (onWall)
            {
                pc.PerformTransition(Transition.WallSlide);

            } else if (!isGrounded)
            {
                pc.PerformTransition(Transition.Airborne);
            }
        }

        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
