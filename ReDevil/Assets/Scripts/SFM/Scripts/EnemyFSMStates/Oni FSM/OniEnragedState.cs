using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniEnragedState : FSMState
{
    bool switchToNextState;
    bool enteredState = true;

    public OniEnragedState()
    {
        base.EnterStateInit();
        Debug.Log("Oni Mad");
        switchToNextState = false;
        enteredState = true;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (enteredState)
        {
            enteredState = false;
            oc.StartCoroutine(Enraged(oc));
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (switchToNextState)
        {
            oc.PerformTransition(Transition.OniIdle);
        }
    }

    private IEnumerator Enraged(OniFSMController oniFSMController)
    {
        yield return new WaitForSeconds(0.2f);
        oniFSMController.GetComponent<SpriteRenderer>().color = Color.red;
        switchToNextState = true;
    }

}
