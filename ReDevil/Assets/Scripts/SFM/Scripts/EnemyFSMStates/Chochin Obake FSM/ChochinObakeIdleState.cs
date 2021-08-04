using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChochinObakeIdleState : EnemyIdleState
{
    //Constructor
    public ChochinObakeIdleState()
    {
        stateID = FSMStateID.ChochinObakeIdling;
        timer = 0;
        atkTransition = false;
    }
    public override void Act(Transform player, Transform npc)
    {
        base.Act(player, npc);
        ChochinObakeFSMController ec = npc.GetComponent<ChochinObakeFSMController>();
        Animator anim = ec.GetComponent<Animator>();

    }

    public override void Reason(Transform player, Transform npc)
    {
        EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();

        //attack transition
        if (atkTransition)
        {
            ec.PerformTransition(Transition.ChochinObakeAttack);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.ChochinOkabeDead);
        }
    }
}
