using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : FSMState
    //THIS IS A BASE IDLE STATE FOR ALL MINION ENEMIES.
    //ENSURE THAT YOU CREATE NEW CONSTRUCTOR AND OVERRIDE REASON FUNCTION TO CHANGE TO APPROPORIATE STATES
{
    protected float timer;
    protected bool atkTransition;

    //Constructor
    public EnemyIdleState()
    {
        stateID = FSMStateID.None;
        timer = 0;
        atkTransition = false;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();
        bool positionSet = false;
        //just a failsafe in case my code is weird and idk where to fix it cause if not constructor where else
        if(atkTransition)
        {
            atkTransition = false;
        }

        //ensure the enemy is properly grounded and position is correctly set on startup IF THEY ARE NOT AN AIRBORNE ENEMY
        if(!ec.airborneEnemy)
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
        if(ec.airborneEnemy)
        {
            ec.SetCurrentPos(ec.rig.position);
        }

        //Check the players range
        ec.CheckRange(player);

        //if in range, begin timer
        if(ec.GetInRange())
        {
            timer += Time.deltaTime;
        }
        if(!ec.GetInRange())
        {
            //if not in range, reset timer and dont count anything
            timer = 0;
        }

        //if the timer hits the enemy's set attack interval, reset the timer
        if(timer >= ec.attackInterval)
        {
            timer = 0;
            atkTransition = true; // this holds true for all enemies.
        }

    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        EnemyFSMController ec = npc.GetComponent<EnemyFSMController>();

        //attack transition
        if (atkTransition)
        {
            ec.PerformTransition(Transition.NodeppouAttack);
        }

        //dead transition
        if (ec.health <= 0)
        {
            ec.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
