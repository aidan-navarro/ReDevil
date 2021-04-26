using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniguchiFSMController : EnemyFSMController
{
    public bool attacking;

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
        WaniguchiIdleState enemyIdle = new WaniguchiIdleState();

        //add transitions
        enemyIdle.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        enemyIdle.AddTransition(Transition.WaniguchiAttack, FSMStateID.WaniguchiAttacking); //transition to attacking state

        WaniguchiAttackState ndpAttack = new WaniguchiAttackState();
        ndpAttack.AddTransition(Transition.WaniguchiIdle, FSMStateID.WaniguchiIdling);
        ndpAttack.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);


        //Create the Dead state
        EnemyDeadState enemyDead = new EnemyDeadState();
        //there are no transitions out of the dead state


        //Add state to the state list
        AddFSMState(enemyIdle);
        AddFSMState(ndpAttack);
        AddFSMState(enemyDead);

    }

    //this function is virtual to adjust for enemies that this will cause glitches for
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(EnemyIFrames());
            if(!attacking)
            {
                rig.velocity = Vector2.zero;
                rig.position = currentPos; //THIS SPECIFIC LINE OF CODE WILL CAUSE ISSUES WITH ENEMIES THAT MOVE TO ATTACK
            }
            
            Vector2 position = this.gameObject.transform.position;

            //send all relative information to the player to take damage, and apply knockback
            PlayerFSMController pc = collision.transform.GetComponent<PlayerFSMController>();
            pc.KnockbackTransition(damage, knockbackPower, position);

            StartCoroutine(EnemyIFrames());
        }
    }

    public void WaniguchiAttack()
    {
        if(!facingRight)
        {
            rig.velocity = atkDirectionRight * new Vector2(-1, 1);
        }
        else if(facingRight)
        {
            rig.velocity = atkDirectionRight;
        }
        
    }
}
