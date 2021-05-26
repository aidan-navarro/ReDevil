using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurikabeIdleState : FSMState
{
    // Start is called before the first frame update
    public NurikabeIdleState()
    {
        stateID = FSMStateID.NurikabeIdling;
    }
    public override void Act(Transform player, Transform npc)
    {
        throw new System.NotImplementedException();
    }

    public override void Reason(Transform player, Transform npc)
    {
        throw new System.NotImplementedException();
    }
}
