using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasanAttackState : FSMState
{
    private bool attackStart; //used to track if the attack has already begun
    //Constructor
    public BasanAttackState()
    {
        stateID = FSMStateID.BasanAttacking;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        BasanFSMController ec = npc.GetComponent<BasanFSMController>();

        //for initial setup to ensure we only call basan attack once, reset attack start ONLY IF
        //the enemy is NOT attacking
        if(attackStart && !ec.attacking)
        {
            attackStart = false;
        }

        //if the flamethrower has not been turned on
        if (!attackStart)
        {
            //turn on flamethrower coroutine
            ec.BasanAttack();
            attackStart = true;
        }
        Debug.Log("Basan Attack State");
    }


    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        BasanFSMController ec = npc.GetComponent<BasanFSMController>();

        //if the flamethrower coroutine has ended
        if (!ec.attacking)
        {
            ec.PerformTransition(Transition.BasanIdle);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}

