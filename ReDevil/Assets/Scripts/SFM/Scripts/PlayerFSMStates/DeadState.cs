﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class DeadState : FSMState
{ 

	//Constructor
    public DeadState()
	{
		stateID = FSMStateID.Dead;
	}


	//Act
	public override void Act( Transform player, Transform npc)
	{
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        pc.UpdateState("Dead");
        pc.CurrentArament.DeActivateArament();
        SceneManager.LoadScene(1);
        //do nothing
    }

    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        
        //do nothing.  Player is dead.  There is no transition to a new state
        
    }

}
