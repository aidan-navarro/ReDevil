using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasanFSMController : EnemyFSMController
{
    public GameObject flamethrower;
    public Transform firepoint;
    public float flamethrowerTimer;
    public bool attacking; // bool value to determine if we are attacking
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


        //flameThrowerObject.enabled = false;
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
        BasanIdleState enemyIdle = new BasanIdleState();

        //add transitions
        enemyIdle.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        enemyIdle.AddTransition(Transition.BasanFlinch, FSMStateID.BasanFlinching); // transition to flinching state
        enemyIdle.AddTransition(Transition.BasanAttack, FSMStateID.BasanAttacking); //transition to attacking state

        BasanAttackState ndpAttack = new BasanAttackState();
        ndpAttack.AddTransition(Transition.BasanIdle, FSMStateID.BasanIdling);
        ndpAttack.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        BasanFlinchState bsFlinch = new BasanFlinchState();
        bsFlinch.AddTransition(Transition.BasanIdle, FSMStateID.BasanIdling);
        bsFlinch.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        //Create the Dead state
        EnemyDeadState enemyDead = new EnemyDeadState();
        //there are no transitions out of the dead state


        //Add state to the state list
        AddFSMState(enemyIdle);
        AddFSMState(ndpAttack);
        AddFSMState(bsFlinch);
        AddFSMState(enemyDead);

    }


    // NEED TO KILL THE FLAMETHROWER SOMEHOW AND 
    public IEnumerator ActivateFlamethrower()
    {
        attacking = true;
        GameObject bulletClone;
        bulletClone = Instantiate(flamethrower, firepoint.position, firepoint.rotation) as GameObject;
        //SET THE KNOCKBACK POSITION FOR THE FLAMETHROWER AS THE POSITION OF THE FIRE BREATHING CHICKEN
        bulletClone.GetComponent<Flamethrower>().kbPosition = currentPos;
        if (!isHit)
        {
            //for extra time to move after flinching so that you can't immediately get hit after flinching
            yield return new WaitForSeconds(flamethrowerTimer);
            Destroy(bulletClone);
        }
        else
        {
            Destroy(bulletClone);
        }

        attacking = false;
    }

    // ------------------- NEW FLAMETHROWER LOGIC ----------------------


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

    public void BasanAttack()
    {
        StartCoroutine("ActivateFlamethrower");
    }

    public void StopBasanAttack()
    {
        StopCoroutine("ActivateFlamethrower");
    }
}
