using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChochinObakeFSMController : EnemyFSMController
{
    public GameObject bullet;
    public Transform firepoint;

    private Vector2 bulletDirectionNormalized;

    // Death Specific... place in EnemyFSM???
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
        enemyIdle.AddTransition(Transition.ChochinOkabeDead, FSMStateID.ChochinOkabeDying); //transition to attacking state
        enemyIdle.AddTransition(Transition.ChochinObakeAttack, FSMStateID.ChochinObakeAttacking); //transition to attacking state

        ChochinObakeAttackState chochinAttack = new ChochinObakeAttackState();
        chochinAttack.AddTransition(Transition.ChochinOkabeDead, FSMStateID.ChochinOkabeDying);
        chochinAttack.AddTransition(Transition.ChochinObakeIdle, FSMStateID.ChochinObakeIdling);

        ChochinOkabeDyingState chochinDying = new ChochinOkabeDyingState();
        chochinDying.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        //Create the Dead state
        EnemyDeadState enemyDead = new EnemyDeadState();
        //there are no transitions out of the dead state


        //Add state to the state list
        AddFSMState(enemyIdle);
        AddFSMState(chochinAttack);
        AddFSMState(chochinDying);
        AddFSMState(enemyDead);

    }

    public void UpdatePlayerPos(Vector2 inRightDirection)
    {
        bulletDirectionNormalized = inRightDirection;
    }

    public override void InstantiateProjectile(GameObject bullet, Vector3 pos, Quaternion rot, Vector2 rightDirection, float inSpeed)
    {
        Debug.Log("Bullet speed: " + inSpeed);
        GameObject bulletClone;
        bulletClone = Instantiate(bullet, pos, rot) as GameObject;
        bulletClone.GetComponent<Bullet>().speed = inSpeed;
        bulletClone.GetComponent<Bullet>().direction = rightDirection;        
    }
    public void ChochinProjectile()
    {
        InstantiateProjectile(bullet, firepoint.position, firepoint.rotation, bulletDirectionNormalized, projectileSpeed);
    }
    public void ConfirmDeath()
    {
        deathConfirmed = true;
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, range);
    //}

    private void ChochinAttackSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/ENEMIES/CHOCIN/Chochin_Attack");
    } 
    
    private void ChochinDeathSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/ENEMIES/Yokai_Death");
    }
}
