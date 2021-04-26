using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeppouIdleState : EnemyIdleState
{
    //Constructor
    public NodeppouIdleState()
    {
        stateID = FSMStateID.NodeppouIdling;
        timer = 0;
        atkTransition = false;
    }


    public override void Reason(Transform player, Transform npc)
    {
        EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();

        //attack transition
        if (atkTransition)
        {
            ec.PerformTransition(Transition.NodeppouAttack);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
