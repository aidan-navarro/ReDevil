using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This state is for midair movement
public class MidairState : FSMState
{
    
    // state variables
    // use these to acquire input axes for air dash ability
    
    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    //Constructor
    public MidairState()
    {
        stateID = FSMStateID.Midair;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        //Debug.Log("State ID: Midair");
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();

        pc.UpdateState("In Midair");
        pc.CheckAirDash();
        // Setting the Dash Vector, we only want to change this whenever the player hasn't gone
        // into dash. That way we avoid mid-air path change
        // We're using the move vector from the Player FSM Controller to dictate the dash path
        if (pc.GetCanDash())
        {
            Debug.Log("Changing dash path");
            if (pc.moveVector != Vector2.zero)
            {
                pc.SetDashPath(pc.moveVector);
            }
            else
            { // should the analog stick not be pointed, the player should still dash horizontally
                if (pc.facingLeft)
                {
                    pc.SetDashPath(Vector2.left);
                }
                else if (!pc.facingLeft)
                {
                    pc.SetDashPath(Vector2.right);
                }
            }
        }
        //Debug.Log(pc.GetDashPath());
        //first check if we are touching a wall or floor
        pc.TouchingFloorOrWall();

        //check if a dash is input
        //pc.CheckDashInput();
        if (!pc.GetDKBTransition()) // if we aren't knocked back from a dash attack
        {
            //Debug.Log("Normal Midair Behaviour");
            if (pc.moveVector.x > 0f)
            {
                pc.direction = 1;
                pc.facingLeft = false;
                Vector2 newMoveSpeed = Vector2.right * pc.GetMoveSpeed();
                newMoveSpeed.y = rig.velocity.y;

                rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * pc.airControl);

                pc.FlipPlayer();
            }
            else if (pc.moveVector.x < 0f)
            {
                pc.direction = -1;
                pc.facingLeft = true;
                Vector2 newMoveSpeed = Vector2.left * pc.GetMoveSpeed();
                newMoveSpeed.y = rig.velocity.y;

                rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * pc.airControl);

                pc.FlipPlayer();

            }

            // FIXED -> just needed to get the boolean for whether or not it was touching the ground
            else //no input detected.  stop speed and set bool to not moving to transition to idle
            {
                //stop momentum movement
                Vector2 newMoveSpeed = Vector2.zero;
                newMoveSpeed.y = rig.velocity.y;
                rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * pc.airControl);
            }
        }
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();

        bool grounded = pc.GetisGrounded();
        bool onWall = pc.GetisTouchingWall();
        bool cD = pc.GetCanDash();
        bool dashAllowed = pc.GetDashInputAllowed();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();
        bool attackButtonDown = pc.GetAttackButtonDown();
        bool soulAttackButtonDown = pc.GetSoulAttackButtonDown();

        pc.CheckAirDash();

        //knockback transition
        if (!invincible && kbTransition)
        {
            pc.PerformTransition(Transition.Knockback);
        }

        //attack transition
        if (attackButtonDown && pc.moveVector.y <= -0.45f )
        {
            pc.PerformTransition(Transition.AirDownStrike);
        }

        if (soulAttackButtonDown && pc.GetSoul() >= player.GetComponent<PlayerAttack>().soulShot.GetComponent<SoulShot>().soulCost)
        {
            pc.PerformTransition(Transition.SoulShot);
        }

        //dash transition
        if ((pc.leftTriggerDown || pc.rightTriggerDown) && cD && dashAllowed )
        {
            // this does work
            //Debug.Log("Commence Air Dash Attack");

            pc.IncrementAirDashCount();
            // check using a boolean if we've hit an enemy already            
            pc.PerformTransition(Transition.AirDashAttack);
            // switch to dash attack state
        }


        if (onWall)
        {
            pc.PerformTransition(Transition.WallSlide);
        }

        //have to track if we are not touching a wall as well because of some weird ass bug where if you latch onto a wall
        //sometimes it also determines your touching the ground
        if (grounded)
        {
            //pc.SetDashPath(Vector2.zero); // reset the dash path
            pc.PerformTransition(Transition.Idle);
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
