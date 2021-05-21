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

    public override void Act(Transform player, Transform npc)
    {
        base.Act(player, npc);
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();

        wc.TouchingFloor();
        Debug.Log("Entering Idle State");
        
    }
    public override void Reason(Transform player, Transform npc)
    {
        EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();

        //attack transition
        if (atkTransition)
        {
            ec.PerformTransition(Transition.WaniguchiAttack);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
