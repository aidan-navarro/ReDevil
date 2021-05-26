using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurikabeIdleState : EnemyIdleState
{
    // Start is called before the first frame update
    private bool rangeTrigger;

    public NurikabeIdleState()
    {
        stateID = FSMStateID.NurikabeIdling;
        rangeTrigger = false;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }
    
    //public override void Act(Transform player, Transform npc)
    //{
    //    EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();
    //    //ec.CheckRange(player);
    //    rangeTrigger = ec.GetInRange();

    //    if (rangeTrigger)
    //    {
    //        Debug.Log("Within Nurikabe Range");
    //        rangeTrigger = true;
    //    } else
    //    {
    //        Debug.Log("Exit Nurikabe Range");
    //        rangeTrigger = false;
    //    }
    //}

    public override void Reason(Transform player, Transform npc)
    {
        EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();

        if (atkTransition)
        {
            Debug.Log("Rising");
            // transition into attacking
            ec.PerformTransition(Transition.NurikabeRise);
        }

        // just in case, might not need it in idle state
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
