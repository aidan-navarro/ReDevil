using System.Collections;
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
        //pc.horizontal = Input.GetAxis("Horizontal");
        //pc.vertical = Input.GetAxis("Vertical");
        isMoving = true;
        bool grounded = pc.GetisGrounded();

        // moving and attacking
        bool attackButtonDown = pc.GetAttackButtonDown();

        pc.SetNoFrictionMaterial();
        pc.SlopeCheck();
        pc.TouchingFloorCeilingWall();
        pc.TouchingInvisibleWall();
        //pc.CheckDashInput();

        pc.UpdateState("Moving");
        pc.soundManager.PlayRun();

        if (pc.moveVector.x > 0f)
        {
            pc.direction = 1;
            pc.facingLeft = false;

            //create newMoveSpeed variable that will be used in the if statement
            Vector2 newMoveSpeed = Vector2.zero;
            

            //determine velocity based on if we are on a slope or flat ground
            if (grounded && !pc.isOnSlope)
            {
                Debug.Log("OnFlat Ground");
                newMoveSpeed = Vector2.right * pc.GetMoveSpeed();
                newMoveSpeed.y = rig.velocity.y;
            }
            else if (grounded && pc.isOnSlope)
            {
                newMoveSpeed.Set(pc.slopeNormalPerp.x * pc.GetMoveSpeed() * -1, pc.slopeNormalPerp.y * pc.GetMoveSpeed() * -1);
            }

            //set the new velocity
            rig.velocity = newMoveSpeed;

            //flip the player
            pc.FlipPlayer();
        }
        else if (pc.moveVector.x < 0f)
        {
            pc.direction = -1;
            pc.facingLeft = true;
            //create newMoveSpeed variable that will be used in the if statement
            Vector2 newMoveSpeed = Vector2.zero;

            //determine velocity based on if we are on a slope or flat ground
            if (grounded && !pc.isOnSlope)
            {
                Debug.Log("OnFlat Ground");
                newMoveSpeed = Vector2.left * pc.GetMoveSpeed();
                newMoveSpeed.y = rig.velocity.y;
            }
            else if (grounded && pc.isOnSlope)
            {
                newMoveSpeed.Set(pc.slopeNormalPerp.x * pc.GetMoveSpeed(), pc.slopeNormalPerp.y * pc.GetMoveSpeed());
            }

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

        // should the player attack, stop it
        if (attackButtonDown)
        {
            rig.velocity = Vector2.zero;
        }

        
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        //pc.horizontal = Input.GetAxis("Horizontal");
        //pc.vertical = Input.GetAxis("Vertical");

        bool grounded = pc.GetisGrounded();
        bool cD = pc.GetCanDash();
        bool dashAllowed = pc.GetDashInputAllowed();
        bool onWall = pc.GetisTouchingWall();
        bool invincible = pc.GetInvincible();
        bool attackButtonDown = pc.GetAttackButtonDown();
        bool kbTransition = pc.GetKbTransition();

        //knockback transition
        if (!invincible && kbTransition)
        {
            pc.soundManager.StopRun();
            pc.PerformTransition(Transition.Knockback);
        }

        //dash transition
        if ((pc.leftTriggerDown || pc.rightTriggerDown) && cD && dashAllowed)
        {
            // Addition: if the player is moving on the ground and dashing, Dash attack
            if (pc.GetisGrounded())
            {
                pc.soundManager.StopRun();
                pc.PerformTransition(Transition.DashAttack);
            }
            //else
            //{
            //    pc.PerformTransition(Transition.Dash);
            //}
        }

        // attacking
        if (attackButtonDown)
        {
            pc.PerformTransition(Transition.GroundAttack1);
        }

        //idle transition
        if (!isMoving)
        {
            pc.soundManager.StopRun();
            pc.PerformTransition(Transition.Idle);
        }

        //jump transition
        if (pc.GetJumpButtonDown() && onWall)
        {
            pc.soundManager.StopRun();
            pc.PerformTransition(Transition.WallJump);
        }
        else if (pc.GetJumpButtonDown())
        {
            pc.soundManager.StopRun();
            pc.PerformTransition(Transition.Jump);
        }

        //airborne transition when walking off an edge
        if (!grounded)
        {
            pc.soundManager.StopRun();
            pc.PerformTransition(Transition.Airborne);
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.soundManager.StopRun();
            pc.PerformTransition(Transition.NoHealth);
        }

    }
}
