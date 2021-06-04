using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class OniFSMController : EnemyFSMController
{
    [SerializeField]
    private GameObject pillarPrehab;
    [SerializeField]
    private GameObject boulderPillarPrehab;
    [SerializeField]
    private GameObject boulderPrehab;
    [SerializeField]
    private Transform firepoint;

    [SerializeField]
    private Transform pillarSpawnPoint;
    [SerializeField]
    private Transform clubAttackPoint;
    [SerializeField]
    private float clubDamage;
    [SerializeField]
    private float clubKnockback;
    [SerializeField]
    private float clubRange;
    [SerializeField]
    private float jumpSmashDamage;
    [SerializeField]
    private float jumpSmashKnockback;
    [SerializeField]
    private float jumpSmashRange;
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
    private float cycloneSpeed;
    public float CycloneSpeed => cycloneSpeed;

    [SerializeField]
    private List<Transform> arenaPoints;
    public List<Transform> ArenaTransforms => arenaPoints;
    [SerializeField]
    private float playerPointLineRange = 0.5f;

    public event EventHandler OnPlayerHit;
    public event EventHandler OnWallHit;

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

        OniIdleState oniIdleState = new OniIdleState();

        oniIdleState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        oniIdleState.AddTransition(Transition.OniBoulderPut, FSMStateID.OniBoulderPutting);
        oniIdleState.AddTransition(Transition.OniJumpSmash, FSMStateID.OniJumpSmashing);
        oniIdleState.AddTransition(Transition.OniChase, FSMStateID.OniChasing);
        oniIdleState.AddTransition(Transition.OniCycloneSmash, FSMStateID.OniCycloneSmashing);

        OniChaseState oniChaseState = new OniChaseState();
        oniChaseState.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);
        oniChaseState.AddTransition(Transition.OniBoulderPut, FSMStateID.OniBoulderPutting);
        oniChaseState.AddTransition(Transition.OniJumpSmash, FSMStateID.OniJumpSmashing);
        oniChaseState.AddTransition(Transition.OniClubSmash, FSMStateID.OniClubSmashing);
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

        AddFSMState(oniIdleState);
        AddFSMState(oniChaseState);
        AddFSMState(boulderPuttState);
        AddFSMState(clubSmashState);
        AddFSMState(jumpingSmashState);
        AddFSMState(cycloneSmasherState);

        AddFSMState(enemyDead);
    }

    public void ClubSmashAttack()
    {
        Collider2D collider = Physics2D.OverlapCircle(clubAttackPoint.position, clubRange, playerLayer);
        if (collider != null)
        {
            collider.GetComponent<PlayerFSMController>().KnockbackTransition(clubDamage, clubKnockback, transform.position);
        }

    }

    public bool IsWithinClubRange(Transform gameObject)
    {
        return Vector2.Distance(transform.position, gameObject.position) <= clubRange;
    }

    public void JumpSmashAttack()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, jumpSmashRange, playerLayer);
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

    public GameObject SpawnPillar(bool inFront)
    {
        Vector3 PillarSpawn = new Vector3();
        GameObject pillarToSpawn;

        // Find the position to spawn in the pillar

        if (inFront)
        {
            PillarSpawn = pillarSpawnPoint.position;
            pillarToSpawn = boulderPillarPrehab;
        }
        else
        {
            bool playerInBetween = false;
            pillarToSpawn = pillarPrehab;

            foreach (Transform arenaTransform in ArenaTransforms) // Find the furtherest point away from the oni that has the player in between them
            {
                // To determine if the arenaPoint is in between the oni and player I'll use DistancePointLine
                playerInBetween = HandleUtility.DistancePointLine(playerTransform.position, transform.position, arenaTransform.position) < playerPointLineRange ? true : false;
                
                if (playerInBetween && Vector3.Distance(transform.position, PillarSpawn) < Vector3.Distance(transform.position, arenaTransform.position))
                {
                    PillarSpawn = arenaTransform.position;
                }
            }
        }

        return Instantiate(pillarToSpawn, PillarSpawn, pillarPrehab.transform.rotation);
    }

    public void BoulderPut()
    {
        Vector2 bulletDirection = (playerTransform.position - firepoint.transform.position);
        bulletDirection.y = 0;
        bulletDirection = bulletDirection.normalized;
        Debug.Log(bulletDirection);
        GameObject bulletClone;
        bulletClone = Instantiate(boulderPrehab, firepoint.position, boulderPrehab.transform.rotation);
        bulletClone.GetComponent<Bullet>().direction = bulletDirection;
        }

    public void MoveTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y), chaseSpeed * Time.deltaTime);
    }

    public void ChargeTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y), cycloneSpeed * Time.deltaTime);
    }

    public bool IsUnderHalfHealth()
    {
        return (health < maxHealth / 2);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(clubAttackPoint.position, clubRange);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerHit?.Invoke(this, EventArgs.Empty);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            OnWallHit?.Invoke(this, EventArgs.Empty);
        }
    }
}
