using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDownStrikeState : FSMState
{
    protected bool attackStarted; //bool to ensure the attack only is calculated once
    protected bool attackFinished; //if the attack is finished, use this bool value to transition to idle

    protected bool chainCancel;

    //Constructor
    public AirDownStrikeState()
    {
        stateID = FSMStateID.AirDownStrike;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("Air Down Strike");
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        pc.UpdateState("Falling Strike");

        if (!attackStarted)
        {
            patk.AirDownStrikeAttack();
            attackStarted = true;
        }

        //check for a knockback cancel
        patk.CheckKnockbackCancel();
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        bool invincible = pc.GetInvincible();

        //knockback transition
        if (!invincible && pc.GetKbTransition()) //if the player isnt in iframes, do a knockback
        {
            attackStarted = false;
            pc.PerformTransition(Transition.Knockback);
        }

        //idle transition
        if (patk.idleTransition)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Idle);
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
