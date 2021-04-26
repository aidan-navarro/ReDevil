using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutenDojiIdleState : EnemyIdleState
{
    //Constructor
    public ShutenDojiIdleState()
    {
        stateID = FSMStateID.ShutenDojiIdling;
        timer = 0;
        atkTransition = false;
    }


    public override void Reason(Transform player, Transform npc)
    {
        EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();

        //attack transition
        if (atkTransition)
        {
            ec.PerformTransition(Transition.ShutenDojiAttack);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
