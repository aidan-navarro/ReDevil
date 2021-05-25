using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChochinObakeFSMController : EnemyFSMController
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
        ChochinObakeIdleState enemyIdle = new ChochinObakeIdleState();

        //add transitions
        enemyIdle.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        enemyIdle.AddTransition(Transition.ChochinObakeAttack, FSMStateID.ChochinObakeAttacking); //transition to attacking state

        ChochinObakeAttackState ndpAttack = new ChochinObakeAttackState();
        ndpAttack.AddTransition(Transition.ChochinObakeIdle, FSMStateID.ChochinObakeIdling);
        ndpAttack.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);


        //Create the Dead state
        EnemyDeadState enemyDead = new EnemyDeadState();
        //there are no transitions out of the dead state


        //Add state to the state list
        AddFSMState(enemyIdle);
        AddFSMState(ndpAttack);
        AddFSMState(enemyDead);

    }

    public override void InstantiateProjectile(GameObject bullet, Vector3 pos, Quaternion rot, Vector2 rightDirection, float inSpeed)
    {
        Debug.Log("Bullet speed: " + inSpeed);
        GameObject bulletClone;
        bulletClone = Instantiate(bullet, pos, rot) as GameObject;
        bulletClone.GetComponent<Bullet>().speed = inSpeed;
        bulletClone.GetComponent<Bullet>().direction = rightDirection;        
    }
}
