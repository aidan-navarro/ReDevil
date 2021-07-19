using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniguchiDyingState : FSMState
{
    private bool enterDeath;
    // Start is called before the first frame update
    public WaniguchiDyingState()
    {
        stateID = FSMStateID.WaniguchiDying;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        enterDeath = false;
    }
    public override void Act(Transform player, Transform npc)
    {
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();
        Rigidbody2D rig = wc.GetComponent<Rigidbody2D>();
        Animator anim = wc.GetComponent<Animator>();
        Collider2D col = wc.GetComponent<Collider2D>();

        Debug.Log("WaniDying");
        wc.ResumeAnim();

        rig.velocity = Vector2.zero;
        // disable the collider so that the enemy won't damage the player when it's dying
        col.enabled = false;

        if (!enterDeath)
        {
            anim.SetTrigger("Death");
            wc.ActivateDeathParticles();
            enterDeath = true;
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();
        
        // the animator will have its event triggered
        if (wc.GetDeathConfirmed())
        {
            Debug.Log("WANIDIES");
            wc.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
