using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAttack2State : FSMState
{
    protected bool attackStarted; //bool to ensure the attack only is calculated once
    protected bool attackFinished; //if the attack is finished, use this bool value to transition to idle

    protected bool chainCancel;

    //Constructor
    public GroundAttack2State()
    {
        stateID = FSMStateID.GroundSecondStrike;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        pc.UpdateState("Ground Attack 2");

        if (!attackStarted)
        {
            Debug.Log("something being called");
            patk.GroundAttack();
            attackStarted = true;
        }

        //check for a dash cancel or an attack chain cancel
        patk.CheckDashCancel();
        patk.CheckAttackCancel();
        patk.CheckKnockbackCancel();
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        //knockback transition
        if (pc.GetInvincible() && pc.GetKbTransition())
        {
            pc.PerformTransition(Transition.Knockback);
        }

        if (patk.groundAttack3Transition)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.GroundAttack3);
        }

        if (!patk.attacking)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Idle);
        }

        if (patk.dashTransition) //if dash cancel = true, change to dash state
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Dash);
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.NoHealth);
        }

    }
}
