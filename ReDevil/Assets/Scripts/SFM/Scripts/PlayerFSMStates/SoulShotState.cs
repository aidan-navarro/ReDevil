using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShotState : FSMState
{
    private bool soulShotStarted;

    public SoulShotState()
    {
        stateID = FSMStateID.SoulShot;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        soulShotStarted = false;

    }
    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("Soul Shot State");

        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();
        Animator anim = pc.GetComponent<Animator>();
        pc.UpdateState("Soul Shot");

        pc.TouchingFloorCeilingWall();

        if (!soulShotStarted)
        {
            // calling soul shot attack in the animation instead
            //patk.SoulShotAttack();
            Debug.Log("Play soul shot");
            if(pc.GetisGrounded())
            {
                anim.Play("Soul Shot Ground");
            } 
            else
            {
                anim.Play("Soul Shot Air");
            }

            soulShotStarted = true;
            pc.SetDidSoulAttack(soulShotStarted);
            pc.SetSoulAttackActive(soulShotStarted);
        }

        pc.TouchingFloorCeilingWall();
        patk.CheckKnockbackCancel();
    }

    public override void Reason(Transform player, Transform npc)
    {
        //Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        bool invincible = pc.GetInvincible();
        bool grounded = pc.GetisGrounded();

        //knockback transition
        if (!invincible && pc.GetKbTransition()) //if the player isnt in iframes, do a knockback
        {
            //soulShotStarted = false;
            pc.PerformTransition(Transition.Knockback);
        }

        if (!pc.GetSoulAttackActive())
        {
            if (patk.idleTransition)
            {
                //soulShotStarted = false;
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Idle);
            }

            if (!grounded)
            {
                //soulShotStarted = false;
                Debug.Log("Soul Shot End");

                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Airborne);
            }

            // check wall too
        }
        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
