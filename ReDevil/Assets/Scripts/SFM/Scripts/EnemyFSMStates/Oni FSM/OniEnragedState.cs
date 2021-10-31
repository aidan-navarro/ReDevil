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
            //oc.oniEnragedSprite.gameObject.SetActive(true);
            oc.oniEnragedCutsceneHolder.GetComponent<Animator>().Play("OniSprite");
            Debug.Log("Cutscene played");
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
            oc.DisableIFrames();
            oc.OnOniEndEnraged -= OniEndEnraged;
            oc.IsEnraged = true;
            oc.PerformTransition(Transition.OniCycloneSmash);
            return;
        }
    }

    

}
