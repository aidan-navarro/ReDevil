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

    // Start is called before the first frame update
    protected override void Initialize()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        rig = GetComponent<Rigidbody2D>();
        if (airborneEnemy)
        {
            rig.gravityScale = 0;
        }
        gravityScale = rig.gravityScale;

        idlePoint = new Vector2(transform.position.x, transform.position.y);
        activePoint = new Vector2(transform.position.x, transform.position.y + riseValue);
        timer = 0;
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
        NurikabeIdleState nurikabeIdle = new NurikabeIdleState();

        nurikabeIdle.AddTransition(Transition.NurikabeRise, FSMStateID.NurikabeRising); // from the rest state, if player is in range, then rise
        nurikabeIdle.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        NurikabeRisingState nurikabeRising = new NurikabeRisingState();

        nurikabeRising.AddTransition(Transition.NurikabeActive, FSMStateID.NurikabeActivating);
        nurikabeRising.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        NurikabeActiveState nurikabeActive = new NurikabeActiveState();
        nurikabeActive.AddTransition(Transition.EnemyNoHealth, FSMStateID.EnemyDead);

        EnemyDeadState enemyDead = new EnemyDeadState();

        AddFSMState(nurikabeIdle);
        AddFSMState(nurikabeRising);
        AddFSMState(nurikabeActive);
        AddFSMState(enemyDead);

    }

    public void ActivateNurikabe(Vector2 startPos, Vector2 endPos, float timer)
    {
        Rigidbody2D rig = GetComponent<Rigidbody2D>();

        rig.position = Vector2.Lerp(startPos, endPos, timer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(idlePoint, 0.1f);
        Gizmos.DrawWireSphere(activePoint, 0.1f);
    }
}
