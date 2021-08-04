using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChochinOkabeDyingState : FSMState
{
    // Start is called before the first frame update
    bool enterDeath;

    public ChochinOkabeDyingState()
    {
        stateID = FSMStateID.ChochinOkabeDying;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        enterDeath = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        ChochinObakeFSMController coc = npc.GetComponent<ChochinObakeFSMController>();
        Rigidbody2D rig = coc.GetComponent<Rigidbody2D>();
        Animator anim = coc.GetComponent<Animator>();
        Collider2D col = coc.GetComponent<Collider2D>();

        rig.velocity = Vector2.zero;
        rig.gravityScale = 0;
        // disable the collider so that the enemy won't damage the player when it's dying
        col.enabled = false;

        if (!enterDeath)
        {
            anim.SetTrigger("Death");
            coc.ActivateDeathParticles();
            enterDeath = true;
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        ChochinObakeFSMController coc = npc.GetComponent<ChochinObakeFSMController>();

        // the animator will have its event triggered
        if (coc.GetDeathConfirmed())
        {
            coc.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
