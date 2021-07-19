using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSMController : AdvancedFSM
{
    //*******************************************************************
    //Variables for the Enemy.  THIS IS THE BASE CLASS ALL ENEMIES WILL 
    //WORK OFF OF AND WILL ONLY CONTAIN UNIVERSAL VARIABLES
    //AND FUNCTIONS
    //*******************************************************************

    protected BoxCollider2D col; //the enemy collider
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    public Rigidbody2D rig;
    protected float gravityScale;

    public float health;
    public float maxHealth;
    //get and set functions for health

    public float range;//The enemy will only attack in this range
    //following 3 lines may not be needed
    private bool inRange;
    public bool GetInRange() { return inRange; }
    public void SetInRange(bool inInRange) { inRange = inInRange; }

    public float damage; //this is the damage that will be dealt to the player

    public float attackInterval; //the amount of time in seconds the enemy must wait in idle before attacking
    public float knockbackPower; //used to determine strength of the knockback.

    public Vector2 atkDirectionRight; //the direction we want a projectile to move in the RIGHT direction.  Used to calculate projectile direction
    public float projectileSpeed;

    // do we need this to be public if we already ahve an accessor
    public Vector2 currentPos; //this variable is to reset the initial position of the enemy when colliding with the player
    public Vector2 GetCurrentPos() { return currentPos; }
    public void SetCurrentPos(Vector2 inCurrentPos) { currentPos = inCurrentPos; }

    [SerializeField]
    private bool isGrounded; //required for enemies that jump
    public bool GetisGrounded() { return isGrounded; }
    public void SetisGrounded(bool inIsGrounded) { isGrounded = inIsGrounded; }
    private Vector2 m_vDebugGround; // checking the grounded feet position vector

    protected bool isTouchingWall; //may be needed.  leave here for now
    public bool GetisTouchingWall() { return isTouchingWall; }
    public void SetisTouchingWall(bool inIsTouchingWall) { isTouchingWall = inIsTouchingWall; }

    protected bool facingRight; //bool used to flip the enemy

    protected bool atkTransition;
    public bool GetAtkTransition() { return atkTransition; }
    public void SetAtkTransition(bool inAtkTransition) { atkTransition = inAtkTransition; }

    public bool airborneEnemy; //Check this if the enemy is meant to stay in the air

    // ----------------- Hit Logic (boolean trigger for all enemies)... -----------------------
    [SerializeField] protected bool isHit;
    public bool GetIsHit() { return isHit; }
    public void SetIsHit(bool inIsHit) { isHit = inIsHit; }

    [SerializeField] protected bool enemyFlinch;
    public bool GetEnemyFlinch() { return enemyFlinch; }
    public void SetEnemyFlinch(bool inEnemyFlinch) { enemyFlinch = inEnemyFlinch; }
    
    // Particle System... part of the enemy fsm? or perhaps something i can toy with in the animator
    [SerializeField] private ParticleSystem m_particles;
    [SerializeField] private Vector3 deathParticlesSpawnPos;
    public ParticleSystem GetParticles() { return m_particles; }


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
        m_vDebugGround = rig.position;
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

        //Create the Dead state
        DeadState dead = new DeadState();
        //there are no transitions out of the dead state


        //Add state to the state list

    }

    public void SpriteFlip()
    {
        facingRight = !facingRight;
        transform.localScale = Vector2.Scale(transform.localScale, new Vector2(-1f, 1f));
    }

    //this function is virtual to adjust for enemies that this will cause glitches for
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(EnemyIFrames());
            rig.velocity = Vector2.zero;
            rig.position = currentPos; //THIS SPECIFIC LINE OF CODE WILL CAUSE ISSUES WITH ENEMIES THAT MOVE TO ATTACK

            Vector2 position = gameObject.transform.position;

            //send all relative information to the player to take damage, and apply knockback
            PlayerFSMController pc = collision.transform.GetComponent<PlayerFSMController>();
            pc.KnockbackTransition(damage, knockbackPower, position);

            StartCoroutine(EnemyIFrames());
        }
    }

    //----------- TEST: Flinching overrideable function ------------
    public virtual void FlinchEnemy(Vector2 flinchKB)
    {
        Debug.Log("base Flinch");
    }

    public IEnumerator EnemyIFrames()
    {
        //make enemy invincible
        gameObject.layer = LayerMask.NameToLayer("IFrames");

        //for extra time to move after flinching so that you can't immediately get hit after flinching
        yield return new WaitForSeconds(1);

        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void TouchingFloor()
    {
        //equation values to determine if the player is on the ground
        Vector2 feetPos = col.bounds.center;
        feetPos.y -= col.bounds.extents.y;
        m_vDebugGround = feetPos;
        Vector2 resizeCol = Vector2.Scale(col.size, transform.localScale);
        isGrounded = Physics2D.OverlapBox(feetPos, new Vector2(resizeCol.x - 0.2f, 0.1f), 0f, groundLayer.value);
    }

    public virtual void CheckRange(Transform player)
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);

        //if the player is in range of the enemy
        if (distance <= range)
        {
            //determine the direction the enemy faces
            if (player.transform.position.x > transform.position.x)
            {
                if (!facingRight)
                {
                    SpriteFlip();
                }
                facingRight = true;
                inRange = true;
            }
            if (player.transform.position.x < transform.position.x)
            {
                if (facingRight)
                {
                    SpriteFlip();
                }
                facingRight = false;
                inRange = true;
            }
        }
        else
        {
            inRange = false;
        }


    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
    }


    //This function will only fire a projectile in a straight line.  Adjust this function as needed per enemy
    public virtual void InstantiateProjectile(GameObject bullet, Vector3 pos, Quaternion rot, Vector2 rightDirection, float inSpeed)
    {
        GameObject bulletClone;
        bulletClone = Instantiate(bullet, pos, rot) as GameObject;
        bulletClone.GetComponent<Bullet>().speed = inSpeed;
        if (facingRight)
        {
            bulletClone.GetComponent<Bullet>().direction = rightDirection;
        }
        else if(!facingRight)
        {
            bulletClone.GetComponent<Bullet>().direction = new Vector2(-1, 1) * rightDirection;
        }
    }

    public void ActivateDeathParticles()
    {
        int direction = 0;
        if (facingRight)
        { direction = -1; }
        else
        {
            direction = 1;
        }
        Instantiate(m_particles, (deathParticlesSpawnPos * direction) + transform.position, Quaternion.Euler(-90f, 0.0f, 0.0f));

    }
    public void Killed()
    {
        Destroy(this.gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_vDebugGround, 0.1f);
        //Gizmos.color = Color.green;
        int direction = 0;
        if (facingRight)
        { direction = -1; }
        else
        {
            direction = 1;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawSphere((deathParticlesSpawnPos * direction) + transform.position, 0.05f);

    }
}
