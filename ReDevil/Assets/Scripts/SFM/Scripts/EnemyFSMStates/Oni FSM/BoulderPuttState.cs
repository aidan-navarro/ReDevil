using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderPuttState : FSMState
{
    bool attackStarted;
    bool attackFinished;
    GameObject pillar;
    public BoulderPuttState()
    {
        stateID = FSMStateID.OniBoulderPutting;
    }
    public override void EnterStateInit()
    {
        Debug.Log("Oni Putt");
        attackStarted = false;
        attackFinished = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (!attackStarted)
        {
            attackStarted = true;
            oc.StartCoroutine(PerformBoulderPutt(npc));
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (oc.health <= 0)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
            return;
        }

        if (!oc.IsEnraged & oc.IsUnderHalfHealth())
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.OniEnraged);
            return;
        }

        if (attackFinished)
        {
            oc.PerformTransition(Transition.OniIdle);
        }

    }

    IEnumerator PerformBoulderPutt(Transform npc)
    {
        // Spawm in the pillar
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        pillar = oc.SpawnPillar(true);
        // Wait a moment before destorying it
        yield return new WaitForSeconds(0.5f);
        if (pillar == null)
        {
            oc.PerformTransition(Transition.OniClubSmash);
        }
        Object.Destroy(pillar);

        oc.BoulderPut();

        yield return new WaitForSeconds(1.0f);

        attackFinished = true;
    }

}
