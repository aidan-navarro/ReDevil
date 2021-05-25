﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubSmashState : FSMState
{
    bool attackStarted;
    bool attackEnded;

    public ClubSmashState()
    {
        stateID = FSMStateID.OniClubSmashing;
    }
    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }


    public override void Act(Transform player, Transform npc)
    {
        if (!attackStarted)
        {
            attackStarted = true;
            npc.GetComponent<OniFSMController>().StartCoroutine(PerformClubSmash(npc));
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (oc.health <= 0)
        {
            npc.GetComponent<OniFSMController>().StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
        }

        if (attackEnded)
        {
            oc.PerformTransition(Transition.OniIdle);
        }
    }

    IEnumerator PerformClubSmash(Transform npc)
    {
        // Make the oni flash yellow or some color to tell the player the boss is about to attack
        npc.GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(3.0f);
        npc.GetComponent<SpriteRenderer>().color = Color.white;
        npc.GetComponent<OniFSMController>().ClubSmashAttack();
        yield return new WaitForSeconds(3.0f);
        attackEnded = true;
    }
}
