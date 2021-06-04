using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DEV NOTES
// to be implemented later once we get animations for this enemy
public class NurikabeFlinchState : FSMState
{
    // Start is called before the first frame update
    public NurikabeFlinchState()
    {
        stateID = FSMStateID.NurikabeFlinching;
    }
    // Update is called once per frame
    void Update()
    {
        
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
