using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniEnragedState : FSMState
{
    bool switchToNextState;
    bool enteredState = true;

    public OniEnragedState()
    {
        stateID = FSMStateID.OniEnraged;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        switchToNextState = false;
        enteredState = true;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (enteredState)
        {
            enteredState = false;
            oc.OnOniBeginEnraged?.Invoke();
            Debug.Log("OnOniBeginEnraged");
            oc.OnOniEndEnraged += OniEndEnraged;
        }
    }

    private void OniEndEnraged()
    {
        switchToNextState = true;
        
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
            oc.OnOniEndEnraged -= OniEndEnraged;
            oc.IsEnraged = true;
            oc.PerformTransition(Transition.OniCycloneSmash);
            return;
        }
    }


}
