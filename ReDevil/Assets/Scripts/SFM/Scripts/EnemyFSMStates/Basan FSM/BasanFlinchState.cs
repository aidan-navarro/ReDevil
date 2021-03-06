using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasanFlinchState : FSMState
{
    // Start is called before the first frame update
    private EnemyTimer et;
    public BasanFlinchState()
    {
        stateID = FSMStateID.BasanFlinching;
    }

    public override void Act(Transform player, Transform npc)
    {
        BasanFSMController bc = npc.GetComponent<BasanFSMController>();
        Animator anim = bc.GetComponent<Animator>();
        et = bc.GetComponent<EnemyTimer>();
        anim.SetBool("Flinch", true);
        anim.Play("Flinch", -1, 0.0f);
        bc.FlinchEnemy(bc.GetKnockbackVel());
        bc.SetEnemyFlinch(false);
        et.StartCoroutine("EnemyKnockbackTimer");

        bc.TouchingFloor();
    }

    public override void Reason(Transform player, Transform npc)
    {
        BasanFSMController bc = npc.GetComponent<BasanFSMController>();
        Animator anim = bc.GetComponent<Animator>();

        bool grounded = bc.GetisGrounded();

        if (grounded)
        {

            bc.PerformTransition(Transition.BasanIdle);
        }
    
        if (bc.health <= 0)
        {
            bc.PerformTransition(Transition.BasanDead);
        }
    }
}
