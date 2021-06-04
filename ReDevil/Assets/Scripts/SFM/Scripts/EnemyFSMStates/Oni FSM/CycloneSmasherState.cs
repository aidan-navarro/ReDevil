using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycloneSmasherState : FSMState
{
    bool enteredState = true;
    bool cycloneCharged;
    GameObject pillar;

    public CycloneSmasherState()
    {
        stateID = FSMStateID.OniCycloneSmashing;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (enteredState)
        {
            oc.StartCoroutine(ChargeUpCyclone(npc));
            pillar = oc.SpawnPillar(false);
            oc.OnWallHit += OnWallHit;
            enteredState = false;
        }

        else if (cycloneCharged)
        {
            oc.ChargeTowardsPlayer();
        }
    }

    private void OnWallHit(object sender, EventArgs e)
    {
        UnityEngine.Object.Destroy(pillar);
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (!enteredState && pillar == null)
        {
            oc.PerformTransition(Transition.OniIdle);
            npc.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (oc.health <= 0)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
            npc.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    IEnumerator ChargeUpCyclone(Transform npc)
    {
        yield return new WaitForSeconds(2.0f);
        npc.GetComponent<SpriteRenderer>().color = Color.red;
        cycloneCharged = true;
    }

}
