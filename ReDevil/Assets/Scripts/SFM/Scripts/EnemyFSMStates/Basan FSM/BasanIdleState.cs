using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasanIdleState : EnemyIdleState
{
    //Constructor
    public BasanIdleState()
    {
        stateID = FSMStateID.BasanIdling;
        timer = 0;
        atkTransition = false;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        BasanFSMController ec = npc.GetComponent<BasanFSMController>();
        Animator anim = ec.GetComponent<Animator>();
        anim.SetBool("Attack", false);
        anim.SetBool("Flinch", false);
        //ec.flameThrowerObject.GetComponent<Flamethrower>().enabled = false;
        bool positionSet = false;
        //just a failsafe in case my code is weird and idk where to fix it cause if not constructor where else
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
        if (ec.GetInRange())
        {
            timer += Time.deltaTime;
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
            atkTransition = true;
        }

        if (ec.GetIsHit())
        {
            Debug.Log("Hit Basan");
            timer = 0;
            ec.SetIsHit(false);
        }

    }


    public override void Reason(Transform player, Transform npc)
    {
        BasanFSMController ec = npc.GetComponent<BasanFSMController>();
        Animator anim = ec.GetComponent<Animator>();

        //attack transition
        if (atkTransition)
        {
            anim.SetTrigger("Initial Attack Trigger");
            ec.PerformTransition(Transition.BasanAttack);
        }

        if (ec.GetEnemyFlinch())
        {
            ec.PerformTransition(Transition.BasanFlinch);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
