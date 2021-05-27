using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingSmashState : FSMState
{
    bool jumpStarted;
    bool jumpEnded;
    bool on2ndJump;
    bool finished2ndJump;
    Transform jumpingTarget;

    public JumpingSmashState()
    {
        stateID = FSMStateID.OniJumpSmashing;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (!finished2ndJump && oc.GetisGrounded())
        {
            // Oni is about to jump
            if (on2ndJump) // Oni is about to jump away from the player's location
            {
                foreach(Transform arenaTransform in oc.ArenaTransforms) // Find the furtherest point away from the player
                {
                    if (Vector3.Distance(player.position, jumpingTarget.position) < Vector3.Distance(player.position, arenaTransform.position))
                    {
                        jumpingTarget = arenaTransform;
                    }
                }
            }
            else // Oni is going to jump on the player
            {
                jumpingTarget = player;
            }

            // Launch the Oni into the air
            oc.Jump();

            jumpStarted = true;
        }
        else if (jumpStarted && !oc.GetisGrounded())
        {
            // Oni is airborne 
            oc.transform.position = Vector2.MoveTowards(oc.transform.position, jumpingTarget.position, oc.JumpSpeed * Time.deltaTime);
        }
        else if (jumpStarted && oc.GetisGrounded())
        {
            // Oni has landed on the ground
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


    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (jumpEnded && finished2ndJump)
        {
             oc.PerformTransition(Transition.OniIdle);
        }

        if (oc.health <= 0)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
