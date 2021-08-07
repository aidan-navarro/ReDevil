using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChochinObakeIdleState : EnemyIdleState
{
    //Constructor
    public ChochinObakeIdleState()
    {
        stateID = FSMStateID.ChochinObakeIdling;
        timer = 0;
        atkTransition = false;
    }
    public override void Act(Transform player, Transform npc)
    {
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        ChochinObakeFSMController ec = npc.GetComponent<ChochinObakeFSMController>();
        Animator anim = ec.GetComponent<Animator>();

        bool positionSet = false;

        if (atkTransition)
        {
            atkTransition = false;
        }

        //ensure the enemy is properly grounded and position is correctly set on startup IF THEY ARE NOT AN AIRBORNE ENEMY
        if (!ec.airborneEnemy)
        {
            if (!ec.GetisGrounded())
            {
                //ENSURE positionSet is false 
                positionSet = false;
                //if the enemy isn't grounded, check for when the enemy touches the floor
                ec.TouchingFloor();

            }
            else if (ec.GetisGrounded())
            {
                //if the enemy is grounded, set its position to prevent weird physics collissions
                //ONLY IF WE HAVE NOT INITIALLY SET ITS POSITION
                if (!positionSet)
                {
                    ec.SetCurrentPos(ec.rig.position);
                    //set position set to true so that we're not setting the position every frame
                    positionSet = true;
                }

            }
        }

        //if the enemy is an airborne enemy, just set the initial spot to idle
        if (ec.airborneEnemy)
        {
            ec.SetCurrentPos(ec.rig.position);
        }

        //Check the players range
        ec.CheckRange(player);

        //if in range, begin timer
        // CHOCHIN SPECIFIC. Only increase the timer should the player is anywhere but below or above the player
        if (ec.GetInRange())
        {
            Vector2 bulletDirection = pc.GetRigidbody2D().position - ec.currentPos;
            Vector2 bulletDirectionNormalized = bulletDirection.normalized;

            if (bulletDirection.x < 1 && bulletDirection.x > -1)
            {
                //do nothing.  maybe anim for enemy not seeing the player
                Debug.Log("Can't see the player");
                anim.SetBool("Searching", true);
            }
            else
            {
                anim.SetBool("Searching", false);
                timer += Time.deltaTime;
            }
        }
        if (!ec.GetInRange())
        {
            //if not in range, reset timer and dont count anything
            timer = 0;
        }

        //if the timer hits the enemy's set attack interval, reset the timer
        if (timer >= ec.attackInterval)
        {
            timer = 0;
            atkTransition = true; // this holds true for all enemies.
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();

        //attack transition
        if (atkTransition)
        {
            ec.PerformTransition(Transition.ChochinObakeAttack);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.ChochinOkabeDead);
        }
    }
}
