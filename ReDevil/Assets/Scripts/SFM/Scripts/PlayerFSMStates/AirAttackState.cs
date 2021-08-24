using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAttackState : FSMState
{
    // Start is called before the first frame update
    public bool attackStarted;
    private float prevGravScale;
    private float airTime;

    public AirAttackState()
    {
        stateID = FSMStateID.AirStrike;
    }
    public override void EnterStateInit()
    {
        base.EnterStateInit();
        airTime = 0.0f;
        attackStarted = false;
    }
    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();
        Animator anim = pc.GetComponent<Animator>();
        //prevGravScale = rig.gravityScale;
        //float attackGravity = prevGravScale/2;
        //rig.gravityScale = attackGravity;

        pc.UpdateState("Air Attack");
        airTime += Time.deltaTime;

        if (!attackStarted)
        {
            anim.Play("Air Attack", 0, 0.0f);
            patk.attacking = true;
            patk.didAirAttack = true;
            attackStarted = true;
        }


        if (!patk.airAttackContact) // this is keeping it locked
        {
            
            // must test this out

            if (pc.moveVector.x > 0f)
            {
               
                Vector2 newMoveSpeed = Vector2.right * pc.GetMoveSpeed();
                newMoveSpeed.y = rig.velocity.y;

                rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * pc.airControl);

            }
            else if (pc.moveVector.x < 0f)
            {
              
                Vector2 newMoveSpeed = Vector2.left * pc.GetMoveSpeed();
                newMoveSpeed.y = rig.velocity.y;

                rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * pc.airControl);

            }

        } 
        else if (patk.airAttackContact)
        {
            pc.UpdateState("Air Attack Hit");
            patk.StopAirAttack();
        }

        pc.TouchingFloorCeilingWall();
        patk.CheckDashCancel();
        patk.CheckKnockbackCancel();
        pc.CheckAirDash();

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

    }

    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        bool invincible = pc.GetInvincible();
        bool isGrounded = pc.GetisGrounded();
        bool onWall = pc.GetisTouchingWall();
        bool isCeiling = pc.GetisTouchingCeiling();
        bool touchingInvisWall = pc.GetisTouchingInvisibleWall();

        patk.CheckDashCancel();
        pc.CheckAirDash();
        if (!invincible && pc.GetKbTransition())
        {
            //patk.StopAirAttack();
            attackStarted = false;
            //rig.gravityScale = prevGravScale;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Knockback);
        }
        if (patk.dashTransition && pc.GetCanDash()) //if dash cancel = true, change to dash state
        {
            //patk.StopAirAttack();
            attackStarted = false;
            //rig.gravityScale = prevGravScale;
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
            pc.IncrementAirDashCount();
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.AirDashAttack);
        }
        if (onWall)
        {
            patk.StopAirAttack();
            attackStarted = false;
            //patk.StopAirAttack();
            patk.didAirAttack = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.WallSlide);
        }

        if (touchingInvisWall)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Airborne);
        }

        if (isGrounded)
        {
            patk.StopAirAttack();
            attackStarted = false;
            patk.idleTransition = true;
            patk.didAirAttack = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Idle);
        }
        // consider switching this logic to be at the end of the animation time
        if (airTime >= patk.GetAirAttackTime())
        {
            //patk.StopAirAttack();

            if (!isGrounded || isCeiling)
            {
                attackStarted = false;
                //rig.gravityScale = prevGravScale;
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Airborne);
            }

            else if (isGrounded)
            {
                //patk.StopAirAttack();

                attackStarted = false;
                patk.idleTransition = true;
                patk.didAirAttack = false;
                //rig.gravityScale = prevGravScale;
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Idle);
            }
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
