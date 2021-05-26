using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurikabeRisingState : FSMState
{
    // Start is called before the first frame update
    public NurikabeRisingState()
    {
        stateID = FSMStateID.NurikabeRising;
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
