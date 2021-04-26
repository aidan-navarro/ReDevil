using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//base class that all minion enemies inherit from.
public class BaseMinion : MonoBehaviour
{
    //variables
    public float health;
    
    private float distance;
    public bool lookRight = false;
    public bool canAttack;
    public bool attacking;

    //currentPos is used to hard set an enemy when player initiates knockback to prevent nudging the enemy
    public Vector2 currentPos;
    //boolean variable to determine if knockback is allowed to be dealt to the player
    public bool canKnockback;

    //used to detemine if the enemy is grounded
    public BoxCollider2D col; //***MAKE SURE THIS IS SET TO THE PHYSICAL COLLIDER THAT THE ENEMY USES TO STAND ON
    public LayerMask groundLayer;

    //bool to determine if the enemy is grounded (not applicable to all enemies)
    public bool isGrounded = false;

    private bool airDownStrikeWasHit = false;

    public float damage;
    public float knockbackPower;
    public Collider2D[] enemyCollider;

    public float range;

    public bool facingRight;

    //timer for attacking
    public float attackTimer;

    public SpriteRenderer sprite;

    public Player player;

    public Rigidbody2D rig;

    protected virtual void IsGrounded()
    {
        //equation values to determine if the player is on the ground
        Vector2 feetPos = col.bounds.center;
        feetPos.y -= col.bounds.extents.y;
        isGrounded = Physics2D.OverlapBox(feetPos, new Vector2(col.size.x - 0.2f, 0.1f), 0f, groundLayer.value);

        //if the enemy is grounded, set current Pos
        if (isGrounded)
        {
            currentPos = transform.position;
        }
    }

    //when the enemy is hit, it takes damage
    public void Damage(float dmg)
    {
        if(airDownStrikeWasHit)
        {
            dmg = 0;
        }
        health -= dmg;
    }

    //this function is virtual to adjust for enemies that this will cause glitches for
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            StartCoroutine(EnemyIFrames());
            rig.velocity = new Vector2(0, 0);
            rig.position = currentPos;
            
            Vector2 position = this.gameObject.transform.position;

            //send all relative information to the player to take damage, and apply knockback
            PlayerFSMController pc = collision.transform.GetComponent<PlayerFSMController>();
            pc.KnockbackTransition(damage, knockbackPower, position);

            StartCoroutine(EnemyIFrames());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            //we are in range to apply a knockback.  lets make sure we disable our rigs velocity to 0
            canKnockback = true;
        }
    }

    //***IRRELEVANT FUNCTION KEEPING FOR BACKTRACKING PURPOSES
    //check if the enemy itself is touched by the player ***MAKE SURE THIS FUNCTION IS CALLED IN INHERITING CLASS***
    protected virtual bool CheckPlayerCollisions(Collider2D enemyCollider, Vector2 direction, float distance)
    {

        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        

        int numHits = enemyCollider.Cast(direction, filter, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger and the hit is tagged for a player object
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Player"))
            {
                Vector2 position = this.gameObject.transform.position;
                //send all relative information to the player to take damage, and apply knockback

                hits[i].collider.SendMessageUpwards("Damage", damage);
                hits[i].collider.SendMessageUpwards("GetKnockbackPower", knockbackPower);
                hits[i].collider.SendMessageUpwards("KnockBack", position);
                               
                StartCoroutine(EnemyIFrames());
                return true;
            }
            
        }
        return false;
    }

    //check range
    protected virtual void CheckRange(Transform player, ref bool canAttack)
    {
        distance = Vector2.Distance(transform.position, player.transform.position);

        //if the player is in range of the enemy
        if (distance <= range)
        {
            //determine the direction the enemy faces
            if (player.transform.position.x > transform.position.x)
            {
                if(!facingRight)
                {
                    SpriteFlip();
                }
                lookRight = true;

            }
            if (player.transform.position.x < transform.position.x)
            {
                if (facingRight)
                {
                    SpriteFlip();
                }
                lookRight = false;
            }
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }

    }

    public IEnumerator EnemyIFrames()
    {
        //make enemy invincible
        gameObject.layer = LayerMask.NameToLayer("IFrames");

        //for extra time to move after flinching so that you can't immediately get hit after flinching
        yield return new WaitForSeconds(1);

        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    //do an attack
    protected virtual void Attack(bool direction, bool doWeAttack)
    {
        //so cool holy shittttttttttttttt.  OVerride this function for specialized enemies
    }

    public void AirDownStrikeHit()
    {
        airDownStrikeWasHit = true;
    }

    public void ResetAirDownStrikeHit()
    {
        if(player.isGrounded)
        {
            airDownStrikeWasHit = false;
        }        
    }

    //rip in pieces, the minion is dead
    public void Dead()
    {
        if (health <= 0)
        {
            Debug.Log("dead");
            Destroy(this.gameObject);
        }
    }

    public void SpriteFlip()
    {
        facingRight = !facingRight;
        this.transform.localScale = Vector2.Scale(this.transform.localScale, new Vector2(-1f, 1f));
        
    }

}
