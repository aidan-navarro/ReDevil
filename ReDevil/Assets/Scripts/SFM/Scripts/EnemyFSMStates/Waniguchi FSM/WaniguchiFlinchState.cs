using System.Collections;
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
        et = wc.GetComponent<EnemyTimer>();
        
        wc.FlinchEnemy(wc.GetKnockbackVel());
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
