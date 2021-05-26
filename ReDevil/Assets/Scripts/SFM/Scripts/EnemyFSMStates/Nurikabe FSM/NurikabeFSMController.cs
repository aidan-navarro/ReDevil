using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurikabeFSMController : EnemyFSMController
{
    [SerializeField] private Transform idlePoint;
    [SerializeField] private Transform activePoint;
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

        NurikabeRisingState nurikabeRising = new NurikabeRisingState();

        NurikabeActiveState nurikabeActive = new NurikabeActiveState();
    }
}
