using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : FSMState
{
    //Constructor
    public EnemyDeadState()
    {
        stateID = FSMStateID.EnemyDead;
    }

    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        EnemyFSMController enemy = npc.GetComponent<EnemyFSMController>();
        //destroy the enemy
        Debug.Log("goodbye cruel world");
        enemy.Killed();

    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        //do nothing
    }
}
