using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniguchiIdleState : EnemyIdleState
{
    //Constructor
    public WaniguchiIdleState()
    {
        stateID = FSMStateID.WaniguchiIdling;
        timer = 0;
        atkTransition = false;
    }
    public override void EnterStateInit()
    {
        base.EnterStateInit();
        timer = 0;
    }
    public override void Act(Transform player, Transform npc)
    {
        base.Act(player, npc);
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();

        wc.TouchingFloor();

        if (wc.GetIsHit())
        {
            Debug.Log("Waniguchi is hit");
            timer = 0;
            wc.SetIsHit(false);
        }

    }
    public override void Reason(Transform player, Transform npc)
    {
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();

        //attack transition
        if (atkTransition)
        {
            wc.PerformTransition(Transition.WaniguchiAttack);
        }
        if (wc.GetEnemyFlinch())
        {
            Debug.Log("Transition");
            wc.PerformTransition(Transition.WaniguchiFlinch);
        }

        //dead transition
        if (wc.health <= 0)
        {
            wc.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
