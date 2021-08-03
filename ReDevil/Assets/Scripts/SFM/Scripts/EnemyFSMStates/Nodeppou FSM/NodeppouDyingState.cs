using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeppouDyingState : FSMState
{
    private bool enterDeath;
    // Start is called before the first frame update
    public NodeppouDyingState()
    {
        stateID = FSMStateID.NodeppouDying;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        enterDeath = false;
    }
    public override void Act(Transform player, Transform npc)
    {
        NodeppouFSMController nc = npc.GetComponent<NodeppouFSMController>();
        Rigidbody2D rig = nc.GetComponent<Rigidbody2D>();
        Animator anim = nc.GetComponent<Animator>();
        Collider2D col = nc.GetComponent<Collider2D>();

        rig.velocity = Vector2.zero;
        // disable the collider so that the enemy won't damage the player when it's dying
        col.enabled = false;

        if (!enterDeath)
        {
            anim.SetTrigger("Death");
            nc.ActivateDeathParticles();
            enterDeath = true;
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        NodeppouFSMController nc = npc.GetComponent<NodeppouFSMController>();

        // the animator will have its event triggered
        if (nc.GetDeathConfirmed())
        {
            nc.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
