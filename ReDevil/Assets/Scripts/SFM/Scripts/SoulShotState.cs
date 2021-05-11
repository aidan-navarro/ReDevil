﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShotState : FSMState
{
    private bool soulShotStarted;

    public SoulShotState()
    {
        stateID = FSMStateID.SoulShot;
    }
    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("Soul Shot State");

        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        pc.UpdateState("Soul Shot");

        if (!soulShotStarted)
        {
            Debug.Log("something being called");
            patk.SoulShotAttack();
            soulShotStarted = true;
        }

        patk.CheckKnockbackCancel();
    }

    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        bool invincible = pc.GetInvincible();
        bool grounded = pc.GetisGrounded();

        //knockback transition
        if (!invincible && pc.GetKbTransition()) //if the player isnt in iframes, do a knockback
        {
            soulShotStarted = false;
            pc.PerformTransition(Transition.Knockback);
        }

        if (patk.idleTransition)
        {
            soulShotStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Idle);
        }

        if (!grounded)
        {
            soulShotStarted = false;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Airborne);
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
