using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniguchiFSMController : EnemyFSMController
{
    public bool attacking;
    [SerializeField]
    private Vector2 knockbackVel;
    public Vector2 GetKnockbackVel() { return knockbackVel; }

    // animator properties
    private Animator waniAnim;
    private float prevAnimSpeed;
    [SerializeField] private bool deathConfirmed;
    public bool GetDeathConfirmed() { return deathConfirmed; }
    public void SetDeathConfirmed(bool inDeathConfirmed)
    {
        deathConfirmed = inDeathConfirmed;
    }

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

        // animator
        waniAnim = GetComponent<Animator>();
        prevAnimSpeed = waniAnim.speed;

        // don't die just yet
        deathConfirmed = false;

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
        enemyIdle.AddTransition(Transition.WaniguchiFlinch, FSMStateID.WaniguchiFlinching);
        enemyIdle.AddTransition(Transition.WaniguchiDead, FSMStateID.WaniguchiDying);

        WaniguchiAttackState waniguchiAttack = new WaniguchiAttackState();
        waniguchiAttack.AddTransition(Transition.WaniguchiIdle, FSMStateID.WaniguchiIdling);
        waniguchiAttack.AddTransition(Transition.WaniguchiFlinch, FSMStateID.WaniguchiFlinching);
        waniguchiAttack.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        waniguchiAttack.AddTransition(Transition.WaniguchiAirborne, FSMStateID.WaniguchiMidair); // the attack state will transition right into the midair state
        waniguchiAttack.AddTransition(Transition.WaniguchiDead, FSMStateID.WaniguchiDying);

        WaniguchiAirState waniguchiAirState = new WaniguchiAirState();
        waniguchiAirState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        waniguchiAirState.AddTransition(Transition.WaniguchiIdle, FSMStateID.WaniguchiIdling); // The ideal result is that from midair, we transition to the idle state and stop moving when we land
        waniguchiAirState.AddTransition(Transition.WaniguchiFlinch, FSMStateID.WaniguchiFlinching);
        waniguchiAirState.AddTransition(Transition.WaniguchiDead, FSMStateID.WaniguchiDying);

        WaniguchiFlinchState waniguchiFlinchState = new WaniguchiFlinchState();
        waniguchiFlinchState.AddTransition(Transition.WaniguchiIdle, FSMStateID.WaniguchiIdling);
        waniguchiFlinchState.AddTransition(Transition.WaniguchiAirborne, FSMStateID.WaniguchiMidair);
        waniguchiFlinchState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        waniguchiFlinchState.AddTransition(Transition.WaniguchiDead, FSMStateID.WaniguchiDying);

        WaniguchiDyingState waniguchiDyingState = new WaniguchiDyingState();
        waniguchiDyingState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        //Create the Dead state
        EnemyDeadState enemyDead = new EnemyDeadState();
        //there are no transitions out of the dead state

        //Add state to the state list
        AddFSMState(enemyIdle);
        AddFSMState(waniguchiAttack);
        AddFSMState(waniguchiAirState);
        AddFSMState(waniguchiFlinchState);
        AddFSMState(waniguchiDyingState);
        AddFSMState(enemyDead);

    }

    //this function is virtual to adjust for enemies that this will cause glitches for
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(EnemyIFrames());
            // is this getting called accidentally?
            if(!attacking)
            {
                rig.velocity = Vector2.zero;
                rig.position = currentPos; //THIS SPECIFIC LINE OF CODE WILL CAUSE ISSUES WITH ENEMIES THAT MOVE TO ATTACK
            }
            Vector2 position = this.gameObject.transform.position;
            ResumeAnim();
            //send all relative information to the player to take damage, and apply knockback
            PlayerFSMController pc = collision.transform.GetComponent<PlayerFSMController>();
            pc.KnockbackTransition(damage, knockbackPower, position);

            StartCoroutine(EnemyIFrames());
        }
    }

    // -------------------- TEST: Waniguchi Flinch Behaviour --------------------
    public override void FlinchEnemy(Vector2 flinchKB)
    {
        //base.FlinchEnemy(flinchKB);
        //rig.AddForce(flinchKB, ForceMode2D.Impulse);
        Debug.Log("Waniflinch");
        if (facingRight)
        {
            rig.velocity = flinchKB * new Vector2(-1, 1);
        }
        else if (!facingRight)
        {
            rig.velocity = flinchKB;
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

    public void WaniguchiStop()
    {
        rig.velocity = Vector2.zero;
    }

    private void PauseAnim()
    {
        waniAnim.speed = 0;
    }

    public void ResumeAnim()
    {
        waniAnim.speed = prevAnimSpeed;
    }

    // reference this in the animator
    public void ConfirmDeath()
    {
        deathConfirmed = true;
    }
}
