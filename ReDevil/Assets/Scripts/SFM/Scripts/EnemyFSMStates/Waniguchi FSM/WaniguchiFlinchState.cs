﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer Notes:
// anytime the Waniguchi is hit, go into this state.
// Waniguchi will stay in this state until a timer ends

public class WaniguchiFlinchState : FSMState
{

    private EnemyTimer et;
    public WaniguchiFlinchState()
    {
        stateID = FSMStateID.WaniguchiFlinching;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    public override void Act(Transform player, Transform npc)
    {
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();
        Animator anim = wc.GetComponent<Animator>();


        et = wc.GetComponent<EnemyTimer>();
        Debug.Log("Waniguchi Flinch");

        wc.FlinchEnemy(wc.GetKnockbackVel());
        anim.Play("Flinch", -1, 0.0f);
        wc.SetEnemyFlinch(false);

        et.StartCoroutine("EnemyKnockbackTimer");

        wc.TouchingFloor();
        
    }

    public override void Reason(Transform player, Transform npc)
    {
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();

        bool grounded = wc.GetisGrounded();

        if (grounded)
        {
            wc.PerformTransition(Transition.WaniguchiIdle);
        }
        else
        {
            wc.PerformTransition(Transition.WaniguchiAirborne);
        }

        if (wc.health <= 0)
        {
            wc.PerformTransition(Transition.EnemyNoHealth);
        }
       

    }
}
