
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
            oc.StartCoroutine(IdleTimer(oc));
        }
        oc.CheckRange(player);

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

        if (!oc.IsEnraged && oc.IsUnderHalfHealth())
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.OniEnraged);
            return;
        }
        if (oc.IsWithinClubRange(player))
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.OniClubSmash);
            return;
        }

        else if (switchState)
        {
            List<Transition> possibleTransitions = new List<Transition>();
            //possibleTransitions.Add(Transition.OniBoulderPut);
            if (Vector3.Distance(oc.transform.position, player.position) >= oc.JumpDistanceRequirement)
            {
                possibleTransitions.Add(Transition.OniJumpSmash);
            } 
            if (!oc.IsWithinClubRange(player))
            {
                possibleTransitions.Add(Transition.OniBoulderPut);
            }
            possibleTransitions.Add(Transition.OniChase);
            oc.StopCoroutine(IdleTimer(oc));
            oc.PerformTransition(possibleTransitions[Random.Range(0, possibleTransitions.Count)]);
            return;
        }
    }

    public IEnumerator IdleTimer(OniFSMController oc)
    {
        yield return new WaitForSeconds(oc.IdleWaitTime);
        switchState = true;
    }
}
