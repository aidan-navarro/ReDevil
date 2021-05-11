using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDashKnockback : FSMState
{
    // simple implementation?
    public GroundDashKnockback()
    {
        stateID = FSMStateID.DashKnockingBack;
    }
    public override void Act(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        pc.SetDKBTransition(false);
      
        Debug.Log("Rebound");
        pc.DashKnockback(); // using the custom dash Knockback
        
        pc.UpdateState("Hit Enemy");

    }

    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        bool grounded = pc.GetisGrounded();
        bool onWall = pc.GetisTouchingWall();
        bool cD = pc.GetCanDash();
        bool dashAllowed = pc.GetDashInputAllowed();

        //if (grounded)
        //{
        //    patk.ReInitializeTransitions();
        //    pc.PerformTransition(Transition.Airborne);
        //}

        //else if (!grounded)
        //{
        //    patk.ReInitializeTransitions();
        //    pc.PerformTransition(Transition.Airborne);
        //}

        ////dash transition
        //if ((pc.leftTriggerDown || pc.rightTriggerDown) && cD && dashAllowed)
        //{
        //    patk.ReInitializeTransitions();
        //    pc.PerformTransition(Transition.Dash);
        //}
        patk.ReInitializeTransitions();
        pc.PerformTransition(Transition.Airborne);

        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
