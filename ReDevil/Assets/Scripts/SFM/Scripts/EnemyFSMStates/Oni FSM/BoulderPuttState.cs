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
        base.EnterStateInit();
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

        if (attackFinished)
        {
            oc.PerformTransition(Transition.OniIdle);
        }

        if (oc.health <= 0)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
        }
    }

    IEnumerator PerformBoulderPutt(Transform npc)
    {
        // Spawm in the pillar
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        pillar = oc.SpawnPillar(true);
        // Wait a moment before destorying it
        yield return new WaitForSeconds(3.0f);
        Object.Destroy(pillar);

        oc.BoulderPut();

        yield return new WaitForSeconds(2.0f);

        attackFinished = true;
    }

}
