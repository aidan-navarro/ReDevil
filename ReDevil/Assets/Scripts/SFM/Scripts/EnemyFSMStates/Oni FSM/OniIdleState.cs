
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniIdleState : FSMState
{
    bool switchState;
    bool enteredState = true;

    public OniIdleState()
    {
        stateID = FSMStateID.OniIdling;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        Debug.Log("Oni Idle");
        switchState = false;
        enteredState = true;
    }
    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (enteredState)
        {
            Debug.Log("Oni Idle");
            enteredState = false;
            oc.StartCoroutine(IdleTimer());
        }
        oc.CheckRange(player);

    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (oc.IsWithinClubRange(player))
        {
            oc.StopCoroutine(IdleTimer());
            oc.PerformTransition(Transition.OniClubSmash);
        }

        else if (switchState)
        {
            List<Transition> possibleTransitions = new List<Transition>();
            possibleTransitions.Add(Transition.OniBoulderPut);
            possibleTransitions.Add(Transition.OniJumpSmash);
            possibleTransitions.Add(Transition.OniChase);
            if (oc.IsUnderHalfHealth())
            {
                possibleTransitions.Add(Transition.OniCycloneSmash);
            }
            oc.StopCoroutine(IdleTimer());
            oc.PerformTransition(possibleTransitions[Random.Range(0, possibleTransitions.Count)]);
        }

        if (oc.health <= 0)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
        }
    }

    public IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(0.5f);
        switchState = true;
    }
}
