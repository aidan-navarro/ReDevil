using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeadState : FSMState
{

	//Constructor
    public DeadState()
	{
		stateID = FSMStateID.Dead;
	}

	//Reason
	public override void Reason( Transform player, Transform npc)
	{
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        //do nothing
        pc.UpdateState("Dead");
    }
	//Act
	public override void Act( Transform player, Transform npc)
	{
        //do nothing
	}

}
