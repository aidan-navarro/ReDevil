using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderPuttState : FSMState
{
    bool attackStarted;
    bool attackFinished;

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
        yield return new WaitForSeconds(2.0f);
    }

}
