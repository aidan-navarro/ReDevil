using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniguchiAttackState : FSMState
{
    bool hasLanded;
    bool atkStart;

    //Constructor
    public WaniguchiAttackState()
    {
        stateID = FSMStateID.WaniguchiAttacking;
        hasLanded = false;
        atkStart = false;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        WaniguchiFSMController ec = npc.GetComponent<WaniguchiFSMController>();

        //reset variables
        if (hasLanded)
        {
            hasLanded = false;
        }
        if (atkStart && !ec.attacking)
        {
            atkStart = false;
        }
        
        if(!atkStart)
        {
            //set an initial velocity for the waniguchi to jump
            ec.WaniguchiAttack();
            atkStart = true;
        }
        if(atkStart)
        {
            ec.TouchingFloor();
            //if waniguchi has landed
            if(ec.GetisGrounded())
            {
                ec.SetCurrentPos(ec.rig.position);
                ec.rig.position = ec.GetCurrentPos();
                ec.attacking = false;
                hasLanded = true;
            }
        }


    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        WaniguchiFSMController ec = npc.GetComponent<WaniguchiFSMController>();

        //the moment waniguchi is grounded again from our temp bool, transition back to idle
        if (hasLanded)
        {
            ec.PerformTransition(Transition.WaniguchiIdle);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
