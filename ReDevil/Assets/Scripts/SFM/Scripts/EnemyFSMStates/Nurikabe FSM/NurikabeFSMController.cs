using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurikabeFSMController : EnemyFSMController
{
    // use this value to adjust how high
    [Header("Nurikabe Settings")]
    [SerializeField] private float riseValue;
    [SerializeField] private float riseSpeed;
    public float GetRiseSpeed() { return riseSpeed; }
    [SerializeField] private Vector2 idlePoint;
    public Vector2 GetIdlePoint() { return idlePoint; }
    
    [SerializeField] private Vector2 activePoint;
    public Vector2 GetActivePoint() { return activePoint; }

    public float timer;
    public bool isActive;

    // Death Specific... place in EnemyFSM???
    [SerializeField] private bool deathConfirmed;
    public bool GetDeathConfirmed() { return deathConfirmed; }
    public void SetDeathConfirmed(bool inDeathConfirmed)
    {
        deathConfirmed = inDeathConfirmed;
    }

    [SerializeField] private Vector2 flinchVector;

    // Start is called before the first frame update
    protected override void Initialize()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        deathConfirmed = false;

        rig = GetComponent<Rigidbody2D>();
        if (airborneEnemy)
        {
            rig.gravityScale = 0;
        }
        gravityScale = rig.gravityScale;

        idlePoint = new Vector2(transform.position.x, transform.position.y - riseValue);
        activePoint = new Vector2(transform.position.x, transform.position.y);
        transform.position = idlePoint;
        timer = 0;
        //box collider
        col = GetComponent<BoxCollider2D>();
        isActive = false;
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
        NurikabeIdleState nurikabeIdle = new NurikabeIdleState();

        nurikabeIdle.AddTransition(Transition.NurikabeRise, FSMStateID.NurikabeRising); // from the rest state, if player is in range, then rise
        nurikabeIdle.AddTransition(Transition.NurikabeDead, FSMStateID.NurikabeDying);

        NurikabeRisingState nurikabeRising = new NurikabeRisingState();

        nurikabeRising.AddTransition(Transition.NurikabeActive, FSMStateID.NurikabeActivating);
        nurikabeRising.AddTransition(Transition.NurikabeDead, FSMStateID.NurikabeDying);

        NurikabeActiveState nurikabeActive = new NurikabeActiveState();
        nurikabeActive.AddTransition(Transition.NurikabeDead, FSMStateID.NurikabeDying);

        NurikabeDyingState nurikabeDying = new NurikabeDyingState();
        nurikabeDying.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        EnemyDeadState enemyDead = new EnemyDeadState();

        AddFSMState(nurikabeIdle);
        AddFSMState(nurikabeRising);
        AddFSMState(nurikabeActive);
        AddFSMState(nurikabeDying);
        AddFSMState(enemyDead);

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit Nurikabe");
        // TO DO: make a check here specifically for nurikabe
        if (collision.transform.CompareTag("Player") && !isActive)
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

    public void ActivateNurikabe(Vector2 startPos, Vector2 endPos, float timer)
    {
        Rigidbody2D rig = GetComponent<Rigidbody2D>();

        rig.position = Vector2.Lerp(startPos, endPos, timer);
    }

    public void SetActiveLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Ground");
    }

    public void NurikabeFlinch()
    {
        if (facingRight)
        {
            rig.AddForce(flinchVector, ForceMode2D.Impulse);
            FMODUnity.RuntimeManager.PlayOneShot("event:/ENEMIES/Enemy_Impact");
    } 
        else
        {
            flinchVector.x *= -1;
            rig.AddForce(flinchVector, ForceMode2D.Impulse);
            FMODUnity.RuntimeManager.PlayOneShot("event:/ENEMIES/Enemy_Impact");

        }
    }

    public void ConfirmDeath()
    {
        deathConfirmed = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(idlePoint, 0.1f);
        Gizmos.DrawWireSphere(activePoint, 0.1f);
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void NurikabeRiseSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/ENEMIES/NURIKABE/Nurikabe_Rise");
    }

    public void NurikabeDeathSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/ENEMIES/Yokai_Death");
        FMODUnity.RuntimeManager.PlayOneShot("event:/ENEMIES/NURIKABE/Nurikabe_Death");
    }
}
