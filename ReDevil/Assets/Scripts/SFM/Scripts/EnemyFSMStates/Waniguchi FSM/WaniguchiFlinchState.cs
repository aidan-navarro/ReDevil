using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer Notes:
// anytime the Waniguchi is hit, go into this state.
// Waniguchi will stay in this state until a timer ends

public class WaniguchiFlinchState : FSMState
{

    private EnemyTimer et;
    public WaniguchiFlinchState()
    {
        stateID = FSMStateID.WaniguchiFlinching;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    public override void Act(Transform player, Transform npc)
    {
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();
        Animator anim = wc.GetComponent<Animator>();

        wc.ResumeAnim();
        et = wc.GetComponent<EnemyTimer>();
        Debug.Log("Waniguchi Flinch"); // only happening once???


        wc.FlinchEnemy(wc.GetKnockbackVel());
        //anim.Play("Flinch", -1, 0.0f);

        et.StartCoroutine("EnemyKnockbackTimer");

        
    }

    public override void Reason(Transform player, Transform npc)
    {
        WaniguchiFSMController wc = npc.GetComponent<WaniguchiFSMController>();
        Animator anim = wc.GetComponent<Animator>();
        bool grounded = wc.GetisGrounded();
        if (!wc.GetEnemyFlinch() && wc.health > 0)
        {
            if (grounded)
            {
                Debug.Log("TouchGroound");
                wc.ResumeAnim();
                wc.PerformTransition(Transition.WaniguchiIdle);
            }
            else
            {
                Debug.Log("InAir");
                wc.PerformTransition(Transition.WaniguchiAirborne);
            }

            
        }
          else if (wc.health <= 0)
            {
            
                wc.PerformTransition(Transition.WaniguchiDead);
            }
    }
}
