using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAttackState : FSMState
{
    // Start is called before the first frame update
    public bool attackStarted;
    private float prevGravScale;
    private float airTime;

    public AirAttackState()
    {
        stateID = FSMStateID.AirStrike;
    }
    public override void EnterStateInit()
    {
        base.EnterStateInit();
        airTime = 0.0f;
    }
    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        //prevGravScale = rig.gravityScale;
        //float attackGravity = prevGravScale/2;
        //rig.gravityScale = attackGravity;

      
        pc.UpdateState("Air Attack");

        if (airTime < patk.GetAirAttackTime())
        {
            Debug.Log("Airstrike");
            patk.AirAttack();
            airTime += Time.deltaTime;
        }
        pc.TouchingFloorCeilingWall();
        patk.CheckDashCancel();
        patk.CheckKnockbackCancel();

    }

    public override void Reason(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerAttack patk = player.GetComponent<PlayerAttack>();

        bool invincible = pc.GetInvincible();
        bool isGrounded = pc.GetisGrounded();
        bool onWall = pc.GetisTouchingWall();
        bool isCeiling = pc.GetisTouchingCeiling();

        if (patk.dashTransition) //if dash cancel = true, change to dash state
        {
            patk.StopAirAttack();
            attackStarted = false;
            //rig.gravityScale = prevGravScale;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.DashAttack);
        }
        if (onWall)
        {
            patk.StopAirAttack();

            attackStarted = false;
            patk.StopAirAttack();
            //rig.gravityScale = prevGravScale;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.WallSlide);
        }

        if (isGrounded)
        {
            patk.StopAirAttack();

            attackStarted = false;
            patk.idleTransition = true;
            patk.didAirAttack = false;
            //rig.gravityScale = prevGravScale;
            patk.ReInitializeTransitions();
            pc.PerformTransition(Transition.Idle);
        }
        if (airTime >= patk.GetAirAttackTime())
        {
            patk.StopAirAttack();
            

            if (!isGrounded || isCeiling)
            {
                attackStarted = false;
                //rig.gravityScale = prevGravScale;
                patk.ReInitializeTransitions();
                pc.PerformTransition(Transition.Airborne);
            }

            
        }

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }
    }
}
