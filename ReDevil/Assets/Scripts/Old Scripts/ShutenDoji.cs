using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//throws an exploding projectile in an arc.  projectile still needs to have lingering hitbox after explosion if hitting the ground
public class ShutenDoji : BaseMinion
{
    //variables to fire the projectile
    public GameObject molotov;

    public float shootInterval;
    public float bulletSpeed;
    //public float fixNormalize;
    public Transform shootPoint; //spot where bullet instantiates on left
    //public Transform shootPointRight; //spot where bullet instantiates on right
    public Vector2 throwingArc;

    private void Update()
    {
        //check if the enemy is dead
        Dead();

        ResetAirDownStrikeHit();

        IsGrounded();

        //check if the player has touched the collider
        //for each collider within the enemy collider array
        /*
        foreach (Collider2D eColliders in enemyCollider)
        {
            //run the check collisions function and determine if a collision has occured
            CheckPlayerCollisions(eColliders, transform.forward, 10);
        }
        */
        //check the range since we know were still alive
        CheckRange(player.transform, ref canAttack);
        //if in range, use the attack command
        Attack(lookRight, canAttack);

    }

    protected override void Attack(bool attackRight, bool attack)
    {
        if (attack == true)
        {
            //to set rate of fire
            attackTimer += Time.deltaTime;

            if (attackTimer >= shootInterval)
            {
                //turret is facing left
                if (!attackRight)
                {
                    //instantiate the bullet and set velocity
                    GameObject bulletClone;
                    bulletClone = Instantiate(molotov, shootPoint.transform.position, shootPoint.transform.rotation) as GameObject;                    
                    bulletClone.GetComponent<Rigidbody2D>().velocity = Vector2.Scale(throwingArc, new Vector2(-1, 1));

                    attackTimer = 0;
                }
                //turret facing right
                if (attackRight)
                {
                    //instantiate the bullet and set velocity
                    GameObject bulletClone;
                    bulletClone = Instantiate(molotov, shootPoint.transform.position, shootPoint.transform.rotation) as GameObject;
                    bulletClone.GetComponent<Rigidbody2D>().velocity = throwingArc;

                    attackTimer = 0;
                }
            }
        }
    }
}
