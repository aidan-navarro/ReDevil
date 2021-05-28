using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurikabeActiveState : FSMState
{
    public NurikabeActiveState()
    {
        stateID = FSMStateID.NurikabeActivating;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        // trigger Nurikabe specific boolean
        
    }

    public override void Act(Transform player, Transform npc)
    {
        
        Debug.Log("Checking Active");
        NurikabeFSMController nc = npc.GetComponent<NurikabeFSMController>();
        nc.SetActiveLayer();
    }

    public override void Reason(Transform player, Transform npc)
    {
        NurikabeFSMController ec = npc.GetComponent<NurikabeFSMController>();
        ec.timer = 0;
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
