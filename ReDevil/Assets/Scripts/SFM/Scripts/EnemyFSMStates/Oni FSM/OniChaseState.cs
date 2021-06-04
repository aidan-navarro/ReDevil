using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniChaseState : FSMState
{
    bool switchToAttack;
    bool enteredState = true;
    public OniChaseState()
    {
        stateID = FSMStateID.OniChasing;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Oni Chase");
        switchToAttack = false;
        enteredState = true;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (enteredState)
        {
            enteredState = false;
            oc.StartCoroutine(ChaseTimer());
        }

        oc.MoveTowardsPlayer();
        oc.CheckRange(player);
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (oc.IsWithinClubRange(player))
        {
            oc.StopCoroutine(ChaseTimer());
            oc.PerformTransition(Transition.OniClubSmash);
        }

        else if (switchToAttack)
        {
            List<Transition> possibleTransitions = new List<Transition>();
            possibleTransitions.Add(Transition.OniBoulderPut);
            possibleTransitions.Add(Transition.OniJumpSmash);
            if (oc.IsUnderHalfHealth())
            {
                possibleTransitions.Add(Transition.OniCycloneSmash);
            }
            oc.StopCoroutine(ChaseTimer());
            oc.PerformTransition(possibleTransitions[Random.Range(0, possibleTransitions.Count)]);
        }

        if (oc.health <= 0)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
        }


    }

    public IEnumerator ChaseTimer()
    {
        yield return new WaitForSeconds(3.0f);
        switchToAttack = true;
    }

}
