using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeppouFlinchState : FSMState
{
    // Start is called before the first frame update
    private EnemyTimer et;
    public NodeppouFlinchState()
    {
        stateID = FSMStateID.NodeppouFlinching;
    }

    public override void Act(Transform player, Transform npc)
    {
        NodeppouFSMController nc = npc.GetComponent<NodeppouFSMController>();
        Animator anim = nc.GetComponent<Animator>();
        et = nc.GetComponent<EnemyTimer>();

        nc.FlinchEnemy(nc.GetKnockbackVel());

        if (nc.GetEnemyFlinch())
        {
            anim.SetTrigger("FlinchTrigger");
            nc.SetEnemyFlinch(false);
        }

        et.StartCoroutine("EnemyKnockbackTimer");

        nc.TouchingFloor();
    }

    public override void Reason(Transform player, Transform npc)
    {
        NodeppouFSMController nc = npc.GetComponent<NodeppouFSMController>();

        bool grounded = nc.GetisGrounded();

        if (grounded)
        {
            nc.PerformTransition(Transition.NodeppouIdle);
        }

        if (nc.health <= 0)
        {
            nc.PerformTransition(Transition.NodeppouDead);
        }
    }
}
