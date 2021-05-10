using System.Collections;
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
    private float dashDistance;
    private bool endDash;
    private bool dashAttackStarted; // bool to check if the attack hitbox has started



    // calling the constructor in the Player FSMController class
    public GroundDashAttack()
    {
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
        bool enterDashKnockback = pc.GetDKBTransition();

        Debug.Log("Dash Knockback State: " + enterDashKnockback);
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
            // if we're still in dash and the player hasn't contacted anyone yet
            if (dashDistance < pc.dashLength && !patk.dashAttackContact)
            {
                patk.StartDashAttack();
            }
            //dashed max distance or we hit someone, end the dash.
            else if (dashDistance >= pc.dashLength)
            {
                Debug.Log("Dash Distance Reached");
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true; 
                patk.EndDashAttack();

            }
            else if (patk.dashAttackContact) // change this into a knockback state?
            {
                Debug.Log("Contact with Dash");
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();
            }

            // during the middle of the dash check if the 
            // pseudo code
            // else if (dash hitbox contact)
            // patk.EndDashAttack()

            //tweak this maybe, because I may implement a different knockback
            // this still enters the knockback state interestingly enough
            if (enterKnockback)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                dashAttackStarted = false;
                endDash = true;
                patk.EndDashAttack();

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
            if (!isGrounded)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = prevGravityScale;
                patk.EndDashAttack();
                dashAttackStarted = false;
                endDash = true;
            }
        }
    }

    // basic reasoning logic for the transitions exiting ground dash
    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>(); // to reset the transitions out of dash attack
        //pc.horizontal = Input.GetAxis("Horizontal");
        //pc.vertical = Input.GetAxis("Vertical");

        isGrounded = pc.GetisGrounded();
        //Debug.Log("IsGroundedCheck" + isGrounded);
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();
        bool dkbTransition = pc.GetDKBTransition();

        if (endDash)
        {
            if (!invincible && kbTransition)
            {
                Debug.Log("Enter Knockback");
                pc.PerformTransition(Transition.Knockback);
            }
            if (patk.dashAttackContact && isGrounded)
            {
                Debug.Log("Transition to Dash Knockback");
                // potential issue... is this going back to idle?
                // patk.ReInitializeTransitions(); // dash attack contact was never getting flicked earlier, so now right on transition the contact boolean will get flicked to false
                pc.PerformTransition(Transition.DashKnockback); // change this into custom knockback
            }

            // just in case
            if (onWall)
            {
                pc.PerformTransition(Transition.WallSlide);
            }

            //// moving transition
            //if ((pc.moveVector.x > 0 || pc.moveVector.x < 0) && !patk.dashAttackContact)
            //{
            //    patk.ReInitializeTransitions();
            //    pc.PerformTransition(Transition.Move);
            //}

          
            if (isGrounded && (dashDistance >= pc.dashLength))
            {
                Debug.Log("Transition into Idle");
                pc.PerformTransition(Transition.Idle);
            }
            else if (isGrounded && onWall)
            {
                Debug.Log("End Ground Dash Hit Wall");
                pc.PerformTransition(Transition.Idle);
            }
            // if we ground dash off of the ledge
            else if (!isGrounded)
            {
                Debug.Log("Dashed off ledge");
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Airborne);
            }
        }

        // dead transition at any point
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
