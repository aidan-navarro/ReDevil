using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniWaitingState : FSMState
{
    bool switchToNextState;
    bool enteredState = true;
    public OniWaitingState()
    {
        stateID = FSMStateID.OniWaiting;
    }
    public override void EnterStateInit()
    {
        base.EnterStateInit();
        Debug.Log("Oni Waiting");
        switchToNextState = false;
        enteredState = true;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (enteredState)
        {
            oc.OnOniBossStart += OnOniBossStart;
            enteredState = false;
        }
    }

    private void OnOniBossStart(object sender, EventArgs e)
    {
        switchToNextState = true;
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (switchToNextState)
        {
            oc.PerformTransition(Transition.OniIdle);
            return;
        }
    }
}
