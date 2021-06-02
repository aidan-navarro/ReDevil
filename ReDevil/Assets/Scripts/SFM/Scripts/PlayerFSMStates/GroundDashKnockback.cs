using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDashKnockback : FSMState
{
    // simple implementation?
    private PlayerFSMController pc;

    public GroundDashKnockback()
    {
        stateID = FSMStateID.DashKnockingBack;
    }
    public override void Act(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        pc = player.GetComponent<PlayerFSMController>();

        // utilizing the timer class to activate a timer
        InvincibleTimer timer = pc.GetComponent<InvincibleTimer>();

        // set the dkb transition to false... stay within the DKB state until landing on the ground;
        //pc.SetDKBTransition(true);
        timer.StartCoroutine("DashKnockbackTimer");

        pc.UpdateState("Hit Enemy");

        if (pc.GetCanDash())
        {
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


        pc.TouchingFloorCeilingWall();

    }

    public override void Reason(Transform player, Transform npc)
    {
        pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();
        InvincibleTimer timer = pc.GetComponent<InvincibleTimer>();

        bool grounded = pc.GetisGrounded();
        bool onWall = pc.GetisTouchingWall();
        bool cD = pc.GetCanDash();
        bool dashAllowed = pc.GetDashInputAllowed();
        //       patk.ReInitializeTransitions();
        //       pc.PerformTransition(Transition.Airborne);

        //// this grounded check is being called first... boolean wrapper value needed 
        if (grounded && !pc.GetDKBTransition())
        {
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Idle);
        }

        else if (!grounded && !pc.GetDKBTransition())
        {
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Airborne);
        }

        //dash transition
        if ((pc.leftTriggerDown || pc.rightTriggerDown) && cD && dashAllowed)
        {
            timer.StopCoroutine("DashKnockbackTimer");
            pc.SetDKBTransition(false);
            patk.ReInitializeTransitions();
            if (grounded)
            {
                pc.PerformTransition(Transition.Dash);
            }
            else
            {
                pc.IncrementAirDashCount();
                pc.PerformTransition(Transition.AirDashAttack);
            }
        }


        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }

}
