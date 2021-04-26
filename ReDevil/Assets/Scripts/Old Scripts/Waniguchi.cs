using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waniguchi : BaseMinion
{
    //enemy jumps forward to attack

    public float jumpInterval;
    public Collider2D groundCollider;
    public bool isAttacking = false;

    public Vector2 attackVel;

    private void Start()
    {
        groundCollider.enabled = false;
    }

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
        if(isAttacking)
        {
            CheckGroundCollisions(groundCollider, transform.forward, 2);
        }
        
        //check the range since we know were still alive
        CheckRange(player.transform, ref canAttack);
        //if in range, use the attack command
        Attack(lookRight, canAttack);

    }

    /*
    protected override bool CheckPlayerCollisions(Collider2D enemyCollider, Vector2 direction, float distance)
    {

        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();


        int numHits = enemyCollider.Cast(direction, filter, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger and the hit is tagged for a player object
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Player"))
            {
                Vector3 position = this.gameObject.transform.position;
                //send all relative information to the player to take damage, and apply knockback
                hits[i].collider.SendMessageUpwards("Damage", damage);
                hits[i].collider.SendMessageUpwards("GetKnockbackPower", knockbackPower);
                hits[i].collider.SendMessageUpwards("KnockBack", position);

                rig.velocity = new Vector2(0, -3);
                StartCoroutine(EnemyIFrames());
                return true;
            }

        }
        return false;
    }
    */

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            //stop velocity upon touching the player.
            rig.velocity = new Vector2(0, 0);
            //only if we are not attacking.  prevent nudging if the player runs into the enemy
            if (!isAttacking)
            {
                rig.position = currentPos;                
            }
            Vector2 position = this.gameObject.transform.position;
            //send all relative information to the player to take damage, and apply knockback

            collision.transform.SendMessageUpwards("Damage", damage);
            collision.transform.SendMessageUpwards("GetKnockbackPower", knockbackPower);
            collision.transform.SendMessageUpwards("KnockBack", position);

            StartCoroutine(EnemyIFrames());
        }
    }

    protected override void Attack(bool attackRight, bool attack)
    {
        if (attack == true)
        {
            //to set rate of fire
            attackTimer += Time.deltaTime;

            if (attackTimer >= jumpInterval)
            {
                isAttacking = true;
                //turret is facing left
                if (!attackRight)
                {
                    rig.velocity = Vector2.Scale(attackVel, new Vector2(-1, 1));
                }
                //turret facing right
                if (attackRight)
                {
                    rig.velocity = attackVel;  
                }
                StartCoroutine(TurnOnGroundCollider());
                attackTimer = 0;
            }
        }
    }


    //this function is only being utilized WHILE we are attacking.  Once we touch the ground using collision.cast, function should be disabled
    public bool CheckGroundCollisions(Collider2D groundCollider, Vector2 direction, float distance)
    {
        Debug.Log("Ground where are youuuuu");
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        int numHits = groundCollider.Cast(direction, filter, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger and the hit is tagged for a player object
            //this means we are touching the floor.  the attack has ended
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Wall"))
            {
                currentPos = transform.position;
                rig.velocity = new Vector2(0, 0);
                isAttacking = false;
                groundCollider.enabled = false;
                Debug.Log("Found the ground yataaaa");
                return true;
            }

        }
        return false;
    }

    public IEnumerator TurnOnGroundCollider()
    {
        //for extra time to move after flinching so that you can't immediately get hit after flinching
        yield return new WaitForSeconds(0.5f);

        groundCollider.enabled = true;

        yield return null;
    }
}
