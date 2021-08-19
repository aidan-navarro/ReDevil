using UnityEngine;
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
        Animator anim = pc.GetComponent<Animator>();

        anim.Play("Death");
        pc.UpdateState("Dead");
        pc.TouchingFloorCeilingWall();

        if (pc.GetisGrounded() || pc.GetisTouchingWall())
        {
            pc.ResumeAnim();
        }

        // PLAY THE DEATH ANIMATION AND CALL PLAY PLAYER DEAD IN THAT ANIMATION
        // slow down the frame speed in the inspector
        //pc.PlayPlayerDead();

        //do nothing
    }

    //Reason
    public override void Reason(Transform player, Transform npc)
    {

        //do nothing.  Player is dead.  There is no transition to a new state

    }

}
