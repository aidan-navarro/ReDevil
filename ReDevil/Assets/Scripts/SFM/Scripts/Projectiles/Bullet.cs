using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//bullet object that is put on projectiles to deal damage.
//THIS IS THE OLD BULLET SCRIPT
public class Bullet : MonoBehaviour
{
    public float damage;
    public float knockbackPower;
    public float speed;

    public Rigidbody2D rig;
    public Vector2 direction;

    protected virtual void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        //update the speed of the bullet
        rig.velocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the collision is a player
        if(collision.transform.CompareTag("Player"))
        {
            //get the position of the bullet
            Vector2 position = this.gameObject.transform.position;

            //make a clone of the player object
            PlayerFSMController pc = collision.transform.GetComponent<PlayerFSMController>();
            pc.KnockbackTransition(damage, knockbackPower, position);

            Destroy(this.gameObject);
        }

        if(collision.transform.CompareTag("Enemy"))
        {
            EnemyFSMController ec = collision.transform.GetComponent<EnemyFSMController>();
            //friendly fire. Call the function that has the enemy take damage.


            //make sure physics don't affect the enemy hit
            ec.rig.position = ec.currentPos;

            Destroy(this.gameObject);
        }

        //if we hit a wall destroy the gameobject
        if (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Bullet"))
        {
            //destroy the bullet
            Destroy(this.gameObject);
        }

    }

    /* Old Version of the collision check
    private bool CheckCollisions(Collider2D bulletCollider, Vector2 direction, float distance)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        int numHits = bulletCollider.Cast(direction, filter, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger AND is the player
            if(!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Player"))
            {
                Vector2 position = this.gameObject.transform.position;

                PlayerFSMController pc = hits[i].collider.transform.GetComponent<PlayerFSMController>();
                pc.KnockbackTransition(damage, knockbackPower, position);
                //send all relative information to the player to take damage, and apply knockback
                //hits[i].collider.SendMessageUpwards("Damage", dmg);
                //hits[i].collider.SendMessageUpwards("GetKnockbackPower", knockbackPower);
                //hits[i].collider.SendMessageUpwards("KnockBack", position);
                //destroy the bullet
                Destroy(this.gameObject);
                return true;
            }
            //if the hit of the collider is NOT trigger AND is an enemy, still deal damage (friendly fire)
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Enemy"))
            {
                Vector3 position = this.gameObject.transform.position;
                //send all relative information to the player to take damage, and apply knockback
                hits[i].collider.SendMessageUpwards("Damage", damage);
                
                //destroy the bullet
                Destroy(this.gameObject);
                return true;
            }
            //else if we hit a wall or another bullet
            else if(!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Wall") || !hits[i].collider.isTrigger && hits[i].collider.CompareTag("Bullet"))
            {
                Destroy(this.gameObject);
            }
        }
        return false;
    }
    */



}
