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

        //do nothing

    }
	//Act
	public override void Act( Transform player, Transform npc)
	{
        //do nothing
	}

}
