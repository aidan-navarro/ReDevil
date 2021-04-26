using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutenDojiFSMController : EnemyFSMController
{
    public GameObject bullet;
    public Transform firepoint;

    //initialize FSM
    protected override void Initialize()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        rig = GetComponent<Rigidbody2D>();
        //set value for gravity based on rigs gravity scaling
        if (airborneEnemy)
        {
            rig.gravityScale = 0;
        }
        gravityScale = rig.gravityScale;

        //box collider
        col = GetComponent<BoxCollider2D>();

        //set currentPos
        currentPos = rig.position;

        ConstructFSM();
    }

    protected override void FSMUpdate()
    {
        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);
    }

    private void ConstructFSM()
    {
        //
        //Create States in each enemies inheriting FSM Controller
        //
        ShutenDojiIdleState enemyIdle = new ShutenDojiIdleState();

        //add transitions
        enemyIdle.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        enemyIdle.AddTransition(Transition.ShutenDojiAttack, FSMStateID.ShutenDojiAttacking); //transition to attacking state

        ShutenDojiAttackState ndpAttack = new ShutenDojiAttackState();
        ndpAttack.AddTransition(Transition.ShutenDojiIdle, FSMStateID.ShutenDojiIdling);
        ndpAttack.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);


        //Create the Dead state
        EnemyDeadState enemyDead = new EnemyDeadState();
        //there are no transitions out of the dead state


        //Add state to the state list
        AddFSMState(enemyIdle);
        AddFSMState(ndpAttack);
        AddFSMState(enemyDead);

    }
}
