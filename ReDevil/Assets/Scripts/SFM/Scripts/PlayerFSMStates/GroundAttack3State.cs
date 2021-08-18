using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAttack3State : FSMState
{
    protected bool attackStarted; //bool to ensure the attack only is calculated once
    protected bool attackFinished; //if the attack is finished, use this bool value to transition to idle

    protected bool chainCancel;

    //Constructor
    public GroundAttack3State()
    {
        stateID = FSMStateID.GroundThirdStrike;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("Third Chain Attack");
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();
        Animator anim = pc.GetComponent<Animator>();

        //set friction material
        pc.SetFrictionMaterial();

        //stop momentum movement
        Vector2 newMoveSpeed = Vector2.zero;
        rig.velocity = newMoveSpeed;

        pc.UpdateState("Ground Attack 3");

        if (!attackStarted)
        {
            anim.SetTrigger("Attack3");
            attackStarted = true;
        }

        //check for a dash cancel or an attack chain cancel
        patk.CheckDashCancel();
        patk.CheckAttackCancel();
        patk.CheckKnockbackCancel();
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        bool invincible = pc.GetInvincible();

        //knockback transition
        if (!invincible && pc.GetKbTransition()) //if the player isnt in iframes, do a knockback
        {
            attackStarted = false;
            pc.PerformTransition(Transition.Knockback);
        }

        if (patk.idleTransition)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Idle);
        }

        if (patk.dashTransition) //if dash cancel = true, change to dash state
        {
            attackStarted = false;
            patk.ReInitializeTransitions();
            if (pc.moveVector.y > 0.0f)
            {
                Debug.Log("DashUp");
                pc.SetDashPath(pc.moveVector);
                pc.IncrementAirDashCount();

                pc.PerformTransition(Transition.GroundToAirDashAttack);
            }
            else
            {
                pc.PerformTransition(Transition.DashAttack);
            }
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            attackStarted = false;
            patk.ReInitializeTransitions();

            pc.PerformTransition(Transition.NoHealth);
        }

    }
}
