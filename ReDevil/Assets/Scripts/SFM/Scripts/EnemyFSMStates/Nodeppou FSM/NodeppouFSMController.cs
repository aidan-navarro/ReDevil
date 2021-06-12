using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeppouFSMController : EnemyFSMController
{
    public GameObject bullet;
    public Transform firepoint;

    [SerializeField]
    private Vector2 knockbackVel;
    public Vector2 GetKnockbackVel() { return knockbackVel; }

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
        NodeppouIdleState enemyIdle = new NodeppouIdleState();

        //add transitions
        enemyIdle.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        enemyIdle.AddTransition(Transition.NodeppouFlinch, FSMStateID.NodeppouFlinching);
        enemyIdle.AddTransition(Transition.NodeppouAttack, FSMStateID.NodeppouAttacking); //transition to attacking state

        NodeppouAttackState ndpAttack = new NodeppouAttackState();
        ndpAttack.AddTransition(Transition.NodeppouIdle, FSMStateID.NodeppouIdling);
        ndpAttack.AddTransition(Transition.NodeppouFlinch, FSMStateID.NodeppouFlinching);
        ndpAttack.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        NodeppouFlinchState ndpFlinch = new NodeppouFlinchState();
        ndpFlinch.AddTransition(Transition.NodeppouIdle, FSMStateID.NodeppouIdling);
        ndpFlinch.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        //Create the Dead state
        EnemyDeadState enemyDead = new EnemyDeadState();
        //there are no transitions out of the dead state


        //Add state to the state list
        AddFSMState(enemyIdle);
        AddFSMState(ndpAttack);
        AddFSMState(ndpFlinch);
        AddFSMState(enemyDead);

    }

    public override void FlinchEnemy(Vector2 flinchKB)
    {
        //base.FlinchEnemy(flinchKB);
        if (facingRight)
        {
            rig.velocity = flinchKB * new Vector2(-1, 1);
        }
        else if (!facingRight)
        {
            rig.velocity = flinchKB;
        }
    }
}
