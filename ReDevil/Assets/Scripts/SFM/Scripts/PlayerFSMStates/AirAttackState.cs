using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAttackState : FSMState
{
    // Start is called before the first frame update
    public bool attackStarted;

    public AirAttackState()
    {
        stateID = FSMStateID.AirStrike;
    }
   
    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        float attackGravity = Physics2D.gravity.y / 2;
        rig.gravityScale = attackGravity;

        pc.UpdateState("Air Attack");

        if (!attackStarted)
        {
            attackStarted = true;
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
    }
}
