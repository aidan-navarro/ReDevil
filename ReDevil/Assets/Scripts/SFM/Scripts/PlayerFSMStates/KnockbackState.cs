using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackState : FSMState
{
    private InvincibleTimer timer;

    //Constructor
    public KnockbackState()
    {
        stateID = FSMStateID.KnockedBack;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        //Debug.Log("Knockback State");
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        timer = pc.GetComponent<InvincibleTimer>();
        pc.SetKbTransition(false);

        //if we are not invincible, do the following
        if(!pc.GetInvincible())
        {
            timer.StartCoroutine("Timer");
            pc.TakeDamage();
            pc.KnockBack();
        }

        pc.UpdateState("Knockback");
        pc.TouchingFloorOrWall();

    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        bool checkImmobile = pc.GetImmobile();
        bool grounded = pc.GetisGrounded();
        bool onWall = pc.GetisTouchingWall();

        if (!checkImmobile && grounded)
        {
            pc.PerformTransition(Transition.Idle);
        }

        if (!checkImmobile && !grounded && onWall)
        {
            pc.PerformTransition(Transition.WallSlide);
        }

        if (!checkImmobile && !grounded)
        {
            pc.PerformTransition(Transition.Airborne);
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
