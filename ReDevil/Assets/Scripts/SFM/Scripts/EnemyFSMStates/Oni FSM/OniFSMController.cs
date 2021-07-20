using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.UI;
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
    private float firepointSpawnBufferX = 0.25f;
    [SerializeField]
    private float firepointSpawnBufferY = 0.50f;
    [SerializeField]
    private Transform boulderPillarSpawnPoint;
    [SerializeField]
    private float pillarSpawnBufferX = 0.25f;
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
    private float jumpHeight;
    public float JumpHeight => jumpHeight;
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
    private TMPro.TextMeshProUGUI stateText;

    [SerializeField]
    private List<Transform> arenaPoints;
    public List<Transform> ArenaTransforms => arenaPoints;
    [SerializeField]
    private float playerPointLineRange = 0.5f;
    [SerializeField]
    private float idleWaitTime = 1.0f;
    public float IdleWaitTime => idleWaitTime;
    [SerializeField]
    private float chaseTime = 2.0f;
    public float ChaseTime => chaseTime;
    [SerializeField]
    private float jumpDistanceRequirement = 6.0f;
    public float JumpDistanceRequirement => jumpDistanceRequirement;
    [SerializeField]
    private Collider2D clubCollider;
    [SerializeField]
    private Collider2D jumpAttackCollider;
    private ContactFilter2D contactFilter2D  = new ContactFilter2D();

    public UnityAction OnPlayerHit;
    public UnityAction OnWallHit;
    public UnityAction OnOniBossStart;
    public UnityAction OnOniBeginEnraged;
    public UnityAction OnOniEndEnraged;
    public UnityAction OnOniBeginDeath;
    public UnityAction OnOniEndDeath;

 
    [SerializeField]
    private GameObject healthBar;

    public bool IsEnraged;

    public GameObject cutsceneHolder;

    public GameObject oniSprite;

    public float GetHealth() { return health; }
    public void SetHealth(float inHealth) { health = inHealth; UpdateHealth(); }

    public float GetMaxHealth() { return maxHealth; }

    public void UpdateHealth()
    {
        healthBar.transform.localScale = new Vector3(health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
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

        contactFilter2D.layerMask = playerLayer;

        //SetSpawnPoints();
        ConstructFSM();
    }

    public void OnValidate()
    {
        SetSpawnPoints();
    }
    protected override void FSMUpdate()
    {
        stateText.text = CurrentStateID.ToString();
        CurrentState.Act(playerTransform, transform);
        CurrentState.Reason(playerTransform, transform);
        
    }

    private void ConstructFSM()
    {
        //
        //Create States in each enemies inheriting FSM Controller
        //

        OniWaitingState oniWaitingState = new OniWaitingState();

        oniWaitingState.AddTransition(Transition.OniIdle, FSMStateID.OniIdling);

        OniEnragedState oniEnragedState = new OniEnragedState();

        oniEnragedState.AddTransition(Transition.OniCycloneSmash, FSMStateID.OniCycloneSmashing);

        OniIdleState oniIdleState = new OniIdleState();

        oniIdleState.AddTransition(Transition.EnemyNoHealth, FSMStateID.OniDeath);
        oniIdleState.AddTransition(Transition.OniClubSmash, FSMStateID.OniClubSmashing);
        oniIdleState.AddTransition(Transition.OniBoulderPut, FSMStateID.OniBoulderPutting);
        oniIdleState.AddTransition(Transition.OniJumpSmash, FSMStateID.OniJumpSmashing);
        oniIdleState.AddTransition(Transition.OniChase, FSMStateID.OniChasing);
        oniIdleState.AddTransition(Transition.OniCycloneSmash, FSMStateID.OniCycloneSmashing);
        oniIdleState.AddTransition(Transition.OniEnraged, FSMStateID.OniEnraged);

        OniChaseState oniChaseState = new OniChaseState();
        oniChaseState.AddTransition(Transition.EnemyNoHealth, FSMStateID.OniDeath);
        oniChaseState.AddTransition(Transition.OniBoulderPut, FSMStateID.OniBoulderPutting);
        oniChaseState.AddTransition(Transition.OniJumpSmash, FSMStateID.OniJumpSmashing);
        oniChaseState.AddTransition(Transition.OniClubSmash, FSMStateID.OniClubSmashing);
        oniChaseState.AddTransition(Transition.OniCycloneSmash, FSMStateID.OniCycloneSmashing);
        oniChaseState.AddTransition(Transition.OniEnraged, FSMStateID.OniEnraged);

        BoulderPuttState boulderPuttState = new BoulderPuttState();
        boulderPuttState.AddTransition(Transition.OniIdle, FSMStateID.OniIdling);
        boulderPuttState.AddTransition(Transition.EnemyNoHealth, FSMStateID.OniDeath);
        boulderPuttState.AddTransition(Transition.OniEnraged, FSMStateID.OniEnraged);

        ClubSmashState clubSmashState = new ClubSmashState();
        clubSmashState.AddTransition(Transition.OniJumpAway, FSMStateID.OniJumpAway);
        clubSmashState.AddTransition(Transition.EnemyNoHealth, FSMStateID.OniDeath);
        clubSmashState.AddTransition(Transition.OniEnraged, FSMStateID.OniEnraged);

        JumpingSmashState jumpingSmashState = new JumpingSmashState();
        jumpingSmashState.AddTransition(Transition.OniJumpAway, FSMStateID.OniJumpAway);
        jumpingSmashState.AddTransition(Transition.EnemyNoHealth, FSMStateID.OniDeath);
        jumpingSmashState.AddTransition(Transition.OniEnraged, FSMStateID.OniEnraged);

        OniJumpAwayState jumpAwayState = new OniJumpAwayState();
        jumpAwayState.AddTransition(Transition.OniIdle, FSMStateID.OniIdling);
        jumpAwayState.AddTransition(Transition.EnemyNoHealth, FSMStateID.OniDeath);
        jumpAwayState.AddTransition(Transition.OniEnraged, FSMStateID.OniEnraged);

        CycloneSmasherState cycloneSmasherState = new CycloneSmasherState();
        cycloneSmasherState.AddTransition(Transition.OniIdle, FSMStateID.OniIdling);
        cycloneSmasherState.AddTransition(Transition.EnemyNoHealth, FSMStateID.OniDeath);
        cycloneSmasherState.AddTransition(Transition.OniEnraged, FSMStateID.OniEnraged);

        //Create the Dead state
        OniDeathState oniDeath = new OniDeathState();
        //there are no transitions out of the dead state

        AddFSMState(oniWaitingState);
        AddFSMState(oniEnragedState);
        AddFSMState(oniIdleState);
        AddFSMState(oniChaseState);
        AddFSMState(boulderPuttState);
        AddFSMState(clubSmashState);
        AddFSMState(jumpingSmashState);
        AddFSMState(jumpAwayState);
        AddFSMState(cycloneSmasherState);

        AddFSMState(oniDeath);
    }

    public void ClubSmashAttack()
    {
        //Collider2D collider = Physics2D.OverlapCircle(clubAttackPoint.position, clubRange, playerLayer);
        //if (collider != null)
        //{
        //    collider.GetComponent<PlayerFSMController>().KnockbackTransition(clubDamage, clubKnockback, transform.position);
        //}

        Collider2D[] collider2Ds = new Collider2D[10];

        int hits = clubCollider.OverlapCollider(contactFilter2D, collider2Ds);

        for(int i = 0; i < hits; i++)
        {
            PlayerFSMController player = collider2Ds[i].GetComponent<PlayerFSMController>();
            if (player != null)
            {
                player.KnockbackTransition(clubDamage, clubKnockback, transform.position);
                return;
            }
        }

    }

    public bool IsWithinClubRange(Transform gameObject)
    {
        return Vector2.Distance(transform.position, gameObject.position) <= clubRange;
    }

    public void JumpSmashAttack()
    {
        //Collider2D collider = Physics2D.OverlapCircle(transform.position, jumpSmashRange, playerLayer);
        //if (collider != null)
        //{
        //    collider.GetComponent<PlayerFSMController>().KnockbackTransition(jumpSmashDamage, jumpSmashKnockback, transform.position);
        //}

        Collider2D[] collider2Ds = new Collider2D[3];

        int hits = jumpAttackCollider.OverlapCollider(contactFilter2D, collider2Ds);

        for (int i = 0; i < hits; i++)
        {
            PlayerFSMController player = collider2Ds[i].GetComponent<PlayerFSMController>();
            if (player != null)
            {
                player.KnockbackTransition(jumpSmashDamage, jumpSmashKnockback, transform.position);
                return;
            }
        }

    }

    public void Jump(Vector2 jumpingTarget)
    {
        if (GetisGrounded())
        {
            float distanceFromTarget = jumpingTarget.x - transform.position.x;
            rig.AddForce(new Vector2(distanceFromTarget, jumpHeight), ForceMode2D.Impulse);
        }
    }

    public GameObject SpawnPillar(bool inFront)
    {
        Vector3 PillarSpawn = new Vector3();
        GameObject pillarToSpawn;

        // Find the position to spawn in the pillar

        PillarSpawn = boulderPillarSpawnPoint.position;

        if (inFront)
        {      
            pillarToSpawn = boulderPillarPrehab;
        }
        else
        {
            bool playerInBetween = false;
            pillarToSpawn = pillarPrehab;

            foreach (Transform arenaTransform in ArenaTransforms) // Find the furtherest point away from the oni that has the player in between them
            {
                // To determine if the arenaPoint is in between the oni and player I'll use DistancePointLine
                //playerInBetween = HandleUtility.DistancePointLine(playerTransform.position, transform.position, arenaTransform.position) < playerPointLineRange ? true : false;
                playerInBetween = Mathf.Sign((transform.position - arenaTransform.position).x) == Mathf.Sign((playerTransform.position - arenaTransform.position).x);


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
        //Vector2 MoveTowardsVector = new Vector2(playerTransform.position.x, transform.position.y) - new Vector2(playerTransform.position.x, transform.position.y);
        //rig.velocity = MoveTowardsVector.normalized * Time.deltaTime * chaseSpeed;
    }

    public void ChargeTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y), cycloneSpeed * Time.deltaTime);
        //Vector2 MoveTowardsVector = new Vector2(playerTransform.position.x, transform.position.y) - new Vector2(playerTransform.position.x, transform.position.y);
        //rig.velocity = MoveTowardsVector.normalized * Time.deltaTime * cycloneSpeed;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(EnemyIFrames());
            rig.velocity = Vector2.zero; 
            
            Vector2 position = gameObject.transform.position;

            //send all relative information to the player to take damage, and apply knockback
            PlayerFSMController pc = collision.transform.GetComponent<PlayerFSMController>();
            pc.KnockbackTransition(damage, knockbackPower, position);

            StartCoroutine(EnemyIFrames());
            OnPlayerHit?.Invoke();
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            OnWallHit?.Invoke();
        }
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;
        UpdateHealth();
    }

    public void OniBossStart()
    {
        OnOniBossStart?.Invoke();
    }

    private void SetSpawnPoints()
    {
        
        SpriteRenderer oniRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer pillarRenderer = boulderPillarPrehab.GetComponent<SpriteRenderer>();
        SpriteRenderer boulderRenderer = boulderPrehab.GetComponent<SpriteRenderer>();

        boulderPillarSpawnPoint.position = transform.position;
        Vector3 newSpawnPoint = new Vector3();
        newSpawnPoint.x -= (pillarSpawnBufferX + oniRenderer.bounds.size.x / 2 + pillarRenderer.bounds.size.x / 2);
        newSpawnPoint.y -= oniRenderer.bounds.size.y / 2;
        newSpawnPoint.z = transform.position.z;
        boulderPillarSpawnPoint.localPosition = newSpawnPoint;


        firepoint.position = transform.position;
        Vector3 newFirePoint = new Vector3();
        newFirePoint.x -= (firepointSpawnBufferX + boulderRenderer.bounds.size.x / 2 + oniRenderer.bounds.size.x / 2);
        newFirePoint.y += -oniRenderer.bounds.size.y / 2 + firepointSpawnBufferY;
        newFirePoint.z = transform.position.z;
        firepoint.localPosition = newFirePoint;
    }
}
