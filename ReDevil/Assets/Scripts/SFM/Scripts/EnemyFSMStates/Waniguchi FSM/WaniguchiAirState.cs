using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniguchiAirState : FSMState
{
    // Start is called before the first frame update
    private bool isGrounded;
    public WaniguchiAirState()
    {
        stateID = FSMStateID.WaniguchiMidair;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        isGrounded = false;

    }

    public override void Act(Transform player, Transform npc)
    {
        WaniguchiFSMController ec = npc.GetComponent<WaniguchiFSMController>();
        EnemyTimer et = npc.GetComponent<EnemyTimer>();

        // if we've commenced attacking from the Waniguchi Attack State and the Waniguchi has left the ground
        // it's still checking if we touched the ground
        ec.TouchingFloor();
        isGrounded = ec.GetisGrounded();
        
        //if (!isGrounded)
        //{
        //    Debug.Log("We Are Attacking");

        //}
        //else 
        
        // THIS IS GETTING CALLED TOO FAST
        if (isGrounded)
        {
            Debug.Log("Grounding");
            ec.SetCurrentPos(ec.rig.position);
            ec.rig.position = ec.GetCurrentPos();
            ec.WaniguchiStop();
            ec.attacking = false; 

        }


    }

    public override void Reason(Transform player, Transform npc)
    {
        WaniguchiFSMController ec = npc.GetComponent<WaniguchiFSMController>();
        Animator anim = ec.GetComponent<Animator>();

        if (!ec.attacking && ec.GetisGrounded())
        {
            Debug.Log("Landed");
            ec.ResumeAnim();

            ec.PerformTransition(Transition.WaniguchiIdle);
        }

        if (ec.GetEnemyFlinch() && ec.health > 0)
        {
            anim.SetBool("Flinch", ec.GetEnemyFlinch());
            anim.Play("Flinch", -1, 0.0f);
            ec.PerformTransition(Transition.WaniguchiFlinch);
        }


        else if (ec.health <= 0)
        {

            ec.PerformTransition(Transition.WaniguchiDead);
        }

    }
}
