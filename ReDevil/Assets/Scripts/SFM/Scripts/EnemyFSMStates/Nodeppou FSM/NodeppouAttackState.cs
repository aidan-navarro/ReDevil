using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeppouAttackState : FSMState
{
    bool bulletFired;

    //Constructor
    public NodeppouAttackState()
    {
        stateID = FSMStateID.NodeppouAttacking;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        NodeppouFSMController ec = npc.GetComponent<NodeppouFSMController>();
        if(bulletFired)
        {
            bulletFired = false;
        }

        if(!bulletFired)
        {
            ec.InstantiateProjectile(ec.bullet, ec.firepoint.position, ec.firepoint.rotation, ec.atkDirectionRight, ec.projectileSpeed);
        }
        
        bulletFired = true;

    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        NodeppouFSMController ec = npc.GetComponent<NodeppouFSMController>();

        if(bulletFired)
        {
            ec.PerformTransition(Transition.NodeppouIdle); 
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
