using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasanDyingState : FSMState
{
    private bool enterDeath;
    public BasanDyingState()
    {
        Debug.Log("CreateDyingState");

        stateID = FSMStateID.BasanDying;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        enterDeath = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        BasanFSMController bc = npc.GetComponent<BasanFSMController>();
        Rigidbody2D rig = bc.GetComponent<Rigidbody2D>();
        Animator anim = bc.GetComponent<Animator>();
        Collider2D col = bc.GetComponent<Collider2D>();

        Debug.Log("BasanDying");
        rig.velocity = Vector2.zero;
        // disable the collider so that the enemy won't damage the player when it's dying
        col.enabled = false;

        if (!enterDeath)
        {
            anim.SetTrigger("Death");
            enterDeath = true;
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        BasanFSMController bc = npc.GetComponent<BasanFSMController>();

        // the animator will have its event triggered
        if (bc.GetDeathConfirmed())
        {
            Debug.Log("BasanDies");
            bc.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
