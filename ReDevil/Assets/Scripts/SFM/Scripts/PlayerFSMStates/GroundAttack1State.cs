﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BASE STATE FOR ALL ATTACK STATES FOR THE PLAYER
public class GroundAttack1State : FSMState
{
    public bool attackStarted; //bool to ensure the attack only is calculated once

    protected bool chainCancel;

    //Constructor
    public GroundAttack1State()
    {
        stateID = FSMStateID.GroundFirstStrike;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("First Chain Attack");
        
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        pc.UpdateState("Ground Attack 1");

        if (!attackStarted)
        {
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

        if (patk.groundAttack2Transition)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.GroundAttack2);
        }

        if (!patk.attacking)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Idle);
        }

        if(patk.dashTransition) //if dash cancel = true, change to dash state
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.DashAttack); // not particularly 
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            attackStarted = false;
            pc.PerformTransition(Transition.NoHealth);
        }

    }
}
