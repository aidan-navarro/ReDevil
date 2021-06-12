using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DEV NOTES
// to be implemented later once we get animations for this enemy

public class ChochinOkabeFlinchState : FSMState
{
    public ChochinOkabeFlinchState()
    {
        stateID = FSMStateID.ChochinOkabeFlinching;
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
