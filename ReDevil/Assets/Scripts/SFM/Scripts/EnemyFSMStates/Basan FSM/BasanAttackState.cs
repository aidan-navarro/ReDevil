using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasanAttackState : FSMState
{
    private bool attackStart; //used to track if the attack has already begun
    private float attackDuration;
    //Constructor
    public BasanAttackState()
    {
        stateID = FSMStateID.BasanAttacking;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        attackStart = false;
        attackDuration = 0.0f;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        BasanFSMController ec = npc.GetComponent<BasanFSMController>();
        BasanFlamethrower bf = npc.GetComponent<BasanFlamethrower>();

        Animator anim = ec.GetComponent<Animator>();
        //for initial setup to ensure we only call basan attack once, reset attack start ONLY IF
        //the enemy is NOT attacking
        //if(attackStart && !ec.attacking)
        //{
        //    attackStart = false;
        //}
        attackDuration += Time.deltaTime;
        //if the flamethrower has not been turned on
        if (attackDuration < bf.getAttackTime()) 
        {
            //turn on flamethrower coroutine
            //ec.BasanAttack();
            anim.SetBool("Attack", true);
            bf.ActivateFlamethrower();
            attackStart = true;
        } 
        else
        {
            anim.SetBool("Attack", false);
            bf.DeactivateFlamethrower();
        }

        if (ec.GetEnemyFlinch())
        {
            // force the attack duration's time to the threshold
            attackDuration = bf.getAttackTime();
            bf.DeactivateFlamethrower();
        }

        //if (ec.GetIsHit())
        //{
        //    Debug.Log("Hit Basan");
        //    ec.StopBasanAttack();
        //    ec.SetIsHit(false);
        //}
        Debug.Log("Basan Attack State");
    }


    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        BasanFSMController ec = npc.GetComponent<BasanFSMController>();
        BasanFlamethrower bf = npc.GetComponent<BasanFlamethrower>();

        //if the flamethrower coroutine has ended
        if (attackDuration >= bf.getAttackTime())
        {
            ec.PerformTransition(Transition.BasanIdle);
        }

        else if (ec.GetEnemyFlinch())
        {
            ec.PerformTransition(Transition.BasanFlinch);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.StopBasanAttack();
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}

