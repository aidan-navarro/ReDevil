using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniJumpAwayState : FSMState
{
    bool jumpStarted;
    bool jumpSwitch;
    bool jumpEnded;
    Vector2 jumpingTarget;

    public OniJumpAwayState()
    {
        stateID = FSMStateID.OniJumpAway;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Oni Jumping");
        jumpStarted = false;
        jumpEnded = false;
        jumpSwitch = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        oc.CheckRange(player);
        if (!jumpSwitch)
        {
            oc.TouchingFloor();
        }

        if (!jumpStarted && !jumpEnded && oc.GetisGrounded())
        {
            jumpingTarget = player.position;
            foreach (Transform arenaTransform in oc.ArenaTransforms) // Find the furtherest point away from the player
            {
                if (Vector3.Distance(player.position, jumpingTarget) < Vector3.Distance(player.position, arenaTransform.position))
                {
                    Debug.Log(arenaTransform.position);
                    jumpingTarget = arenaTransform.position;
                }
            }

            // Launch the Oni into the air
            oc.Jump(jumpingTarget);
            jumpStarted = true;
            oc.SetisGrounded(false);

            oc.StartCoroutine(WaitForJumpSwitch());
        }

        //else if (jumpStarted && !oc.GetisGrounded())
        //{
        //    // Oni is airborne 
        //    oc.transform.position = Vector2.MoveTowards(oc.transform.position, new Vector2(jumpingTarget.x, oc.transform.position.y), oc.JumpHeight * Time.deltaTime);
        //    if (oc.rig.velocity.y > 0 && Vector2.Distance(oc.transform.position, new Vector2(jumpingTarget.x, oc.transform.position.y)) <= 2.0f)
        //    {
        //        oc.rig.velocity = new Vector2(oc.rig.velocity.x, 0);
        //    }
        //}
        else if (jumpStarted && oc.GetisGrounded())
        {
            // Oni has landed on the ground
            Debug.Log("Oni Landed");
            oc.JumpSmashAttack();
            oc.rig.velocity = new Vector2();
            jumpEnded = true;
            jumpStarted = false;
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (oc.health <= 0)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
            return;
        }

        if (!oc.IsEnraged && oc.IsUnderHalfHealth() && jumpEnded)
        {
            oc.StopAllCoroutines();
            oc.DisableIFrames();
            oc.PerformTransition(Transition.OniEnraged);
            return;
        }

        if (jumpEnded)
        {
            oc.DisableIFrames();
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
