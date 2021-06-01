using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeppouIdleState : EnemyIdleState
{
    //Constructor
    bool isHit;

    public NodeppouIdleState()
    {
        stateID = FSMStateID.NodeppouIdling;
        timer = 0;
        isHit = false;
        atkTransition = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        base.Act(player, npc);

        NodeppouFSMController nc = npc.GetComponent<NodeppouFSMController>();

        if (nc.GetIsHit())
        {
            Debug.Log("Nodeppou is hit");
            timer = 0;
            nc.SetIsHit(false);
        }
        
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
