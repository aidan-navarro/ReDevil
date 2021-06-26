using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingSmashState : FSMState
{
    bool jumpStarted;
    bool jumpSwitch;
    bool jumpEnded;
    bool enteredState = true;
    Vector2 jumpingTarget;

    public JumpingSmashState()
    {
        stateID = FSMStateID.OniJumpSmashing;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Oni Jumping");
        jumpStarted = false;
        jumpEnded = false;
        jumpSwitch = false;
        enteredState = true;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        oc.CheckRange(player);
        if (!jumpSwitch)
        {
            oc.TouchingFloor();
        }

        if (enteredState)
        {
            oc.OnPlayerHit += OnPlayerHit;
            enteredState = false;
        }

        if (!jumpStarted && !jumpEnded && oc.GetisGrounded())
        {
            Debug.Log("OnGround");
            // Oni is going to jump on the player
            {
                Debug.Log("Jumping on the player");
                jumpingTarget = player.position;
            }

            // Launch the Oni into the air
            oc.Jump();
            jumpStarted = true;
            oc.SetisGrounded(false);
            
            oc.StartCoroutine(WaitForJumpSwitch());
        }
        else if (jumpStarted && !oc.GetisGrounded())
        {
            // Oni is airborne 
            oc.transform.position = Vector2.MoveTowards(oc.transform.position, new Vector2(jumpingTarget.x, oc.transform.position.y), oc.JumpSpeed * Time.deltaTime);
            if (oc.rig.velocity.y > 0 && Vector2.Distance(oc.transform.position, new Vector2(jumpingTarget.x, oc.transform.position.y)) <= 2.0f)
            {
                oc.rig.velocity = new Vector2(oc.rig.velocity.x, 0);
            }
        }
        else if (jumpStarted && oc.GetisGrounded())
        {
            // Oni has landed on the ground
            Debug.Log("Oni Landed");
            oc.JumpSmashAttack();
            jumpEnded = true;
            jumpStarted = false;
        }
    }

    private void OnPlayerHit()
    {
        Debug.Log("OnPlayerHit");
        jumpEnded = true;
        jumpStarted = false;
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (oc.health <= 0)
        {
            oc.OnPlayerHit -= OnPlayerHit;
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
            return;
        }

        if (!oc.IsEnraged && oc.IsUnderHalfHealth() && jumpEnded)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.OniEnraged);
            return;
        }

        if (jumpEnded)
        {
            oc.OnPlayerHit -= OnPlayerHit;
            oc.PerformTransition(Transition.OniJumpAway);
            return;
        }

        
    }

    private IEnumerator WaitForJumpSwitch()
    {
        jumpSwitch = true;
        yield return new WaitForSeconds(0.1f);
        jumpSwitch = false;
    }
}
