using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniBreakState : FSMState
{
    bool switchToNextState;
    bool enteredState = true;

    public OniBreakState()
    {
        stateID = FSMStateID.OniBreak;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (enteredState)
        {
            enteredState = false;
            oc.OnOniBeginBreak?.Invoke();
            oc.OnOniEndBreak += OniEndBreak;
        }
    }

    private void OniEndBreak()
    {
        switchToNextState = true;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        switchToNextState = false;
        enteredState = true;
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

        if (switchToNextState)
        {
            oc.DisableIFrames();
            oc.OnOniEndBreak -= OniEndBreak;
            oc.IsEnraged = true;
            oc.PerformTransition(Transition.OniCycloneSmash);
            return;
        }
    }
}
