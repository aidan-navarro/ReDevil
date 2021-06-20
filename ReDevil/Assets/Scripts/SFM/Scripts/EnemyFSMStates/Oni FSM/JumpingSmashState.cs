using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingSmashState : FSMState
{
    bool jumpStarted;
    bool jumpSwitch;
    bool jumpEnded;
    bool on2ndJump;
    bool finished2ndJump;
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
        on2ndJump = false;
        finished2ndJump = false;
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

        Debug.Log("Act");
        if (enteredState)
        {
            oc.OnPlayerHit += OnPlayerHit;
            enteredState = false;
        }

        if (!jumpStarted && !finished2ndJump && oc.GetisGrounded())
        {
            Debug.Log("OnGround");
            // Oni is about to jump
            if (on2ndJump) // Oni is about to jump away from the player's location
            {
                foreach(Transform arenaTransform in oc.ArenaTransforms) // Find the furtherest point away from the player
                {
                    if (Vector3.Distance(player.position, jumpingTarget) < Vector3.Distance(player.position, arenaTransform.position))
                    {
                        Debug.Log("Jumping away");
                        jumpingTarget = arenaTransform.position;
                    }
                }
            }
            else // Oni is going to jump on the player
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
        }
        else if (jumpStarted && oc.GetisGrounded())
        {
            // Oni has landed on the ground
            Debug.Log("Oni Landed");
            oc.JumpSmashAttack();
            jumpEnded = true;
            jumpStarted = false;
            if (!on2ndJump)
            {
                on2ndJump = true;
            }
            else
            {
                finished2ndJump = true;
            }
        }
    }

    private void OnPlayerHit(object sender, EventArgs e)
    {
        Debug.Log("OnPlayerHit");
        jumpEnded = true;
        jumpStarted = false;
        if (!on2ndJump)
        {
            on2ndJump = true;
        }
        else
        {
            finished2ndJump = true;
        }
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

        if (!oc.IsEnraged && oc.IsUnderHalfHealth() && jumpEnded && finished2ndJump)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.OniEnraged);
            return;
        }

        if (jumpEnded && finished2ndJump)
        {
            oc.OnPlayerHit -= OnPlayerHit;
            oc.PerformTransition(Transition.OniIdle);
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
