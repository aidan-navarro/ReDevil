using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniFSMController : EnemyFSMController
{
    public GameObject pillarPrehab;
    public GameObject boulderPrehab;
    [SerializeField]
    private Transform firepoint;
    [SerializeField]
    private Transform clubAttackPoint;
    [SerializeField]
    private float clubDamage;
    [SerializeField]
    private float clubKnockback;
    [SerializeField]
    private float jumpSmashDamage;
    [SerializeField]
    private float jumpSmashKnockback;
    [SerializeField]
    private float cycloneSmashDamage;
    [SerializeField]
    private float CycloneSmashKnockback;
    [SerializeField]
    private float jumpSpeed;
    public float JumpSpeed => jumpSpeed;
    [SerializeField]
    private float airSpeed;
    public float AirSpeed => airSpeed;
    [SerializeField]
    private float chaseSpeed;
    public float ChaseSpeed => chaseSpeed;

    [SerializeField]
    private List<Transform> arenaPoints;
    public List<Transform> ArenaTransforms => arenaPoints;

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

        OniIdleState idleState = new OniIdleState();

        idleState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        idleState.AddTransition(Transition.OniBoulderPut, FSMStateID.OniBoulderPutting);
        idleState.AddTransition(Transition.OniJumpSmash, FSMStateID.OniJumpSmashing);
        idleState.AddTransition(Transition.OniChase, FSMStateID.OniChasing);
        idleState.AddTransition(Transition.OniCycloneSmash, FSMStateID.OniCycloneSmashing);

        OniChaseState oniChaseState = new OniChaseState();
        oniChaseState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        oniChaseState.AddTransition(Transition.OniBoulderPut, FSMStateID.OniBoulderPutting);
        oniChaseState.AddTransition(Transition.OniJumpSmash, FSMStateID.OniJumpSmashing);
        oniChaseState.AddTransition(Transition.OniCycloneSmash, FSMStateID.OniCycloneSmashing);

        BoulderPuttState boulderPuttState = new BoulderPuttState();
        boulderPuttState.AddTransition(Transition.OniIdle, FSMStateID.OniIdling);
        boulderPuttState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        ClubSmashState clubSmashState = new ClubSmashState();
        clubSmashState.AddTransition(Transition.OniIdle, FSMStateID.OniIdling);
        clubSmashState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        JumpingSmashState jumpingSmashState = new JumpingSmashState();
        jumpingSmashState.AddTransition(Transition.OniIdle, FSMStateID.OniIdling);
        jumpingSmashState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        CycloneSmasherState cycloneSmasherState = new CycloneSmasherState();
        cycloneSmasherState.AddTransition(Transition.OniIdle, FSMStateID.OniIdling);
        cycloneSmasherState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        //Create the Dead state
        EnemyDeadState enemyDead = new EnemyDeadState();
        //there are no transitions out of the dead state
    }

    public void ClubSmashAttack()
    {
        Collider2D collider = Physics2D.OverlapCircle(clubAttackPoint.position, range / 2, playerLayer);
        if (collider != null)
        {
            collider.GetComponent<PlayerFSMController>().KnockbackTransition(clubDamage, clubKnockback, clubAttackPoint.position);
        }
    }

    public void JumpSmashAttack()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, range, playerLayer);
        if (collider != null)
        {
            collider.GetComponent<PlayerFSMController>().KnockbackTransition(jumpSmashDamage, jumpSmashKnockback, transform.position);
        }
    }

    public void Jump()
    {
        Vector2 newVel = rig.velocity;
        newVel.y = jumpSpeed;
        rig.velocity = newVel;
    }

}
