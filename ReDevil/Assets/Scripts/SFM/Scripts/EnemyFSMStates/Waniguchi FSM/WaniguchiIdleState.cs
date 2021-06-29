using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniguchiIdleState : EnemyIdleState
{
    //Constructor
    public WaniguchiIdleState()
    {
        stateID = FSMStateID.WaniguchiIdling;
        timer = 0;
        atkTransition = false;
    }
    public override void EnterStateInit()
    {
        base.EnterStateInit();
        timer = 0;
    }
    public override void Act(Transform player, Transform npc)
    {
        base.Act(player, npc);
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();
        Animator anim = wc.GetComponent<Animator>();
        anim.SetBool("Attack", false);
        anim.SetBool("Flinch", false);
        wc.TouchingFloor();

        if (wc.GetIsHit())
        {
            timer = 0;
            wc.SetIsHit(false);
        }

    }
    public override void Reason(Transform player, Transform npc)
    {
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();
        Animator anim = wc.GetComponent<Animator>();

        //attack transition
        if (atkTransition)
        {
            //anim.SetBool("Attack", atkTransition);
            anim.SetTrigger("AttackTrigger");
            wc.PerformTransition(Transition.WaniguchiAttack);
        }
        if (wc.GetEnemyFlinch())
        {
            Debug.Log("Transition"); 
            anim.SetBool("Flinch", wc.GetEnemyFlinch());
            wc.PerformTransition(Transition.WaniguchiFlinch);
        }

        //dead transition
        if (wc.health <= 0)
        {
            wc.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
