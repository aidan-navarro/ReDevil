using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    public bool didFlameHit;
    public float damage;
    public float knockbackPower;
    public Vector2 kbPosition;
    private Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        CheckCollisions(col, transform.forward, 10);
    }

    private bool CheckCollisions(Collider2D bulletCollider, Vector2 direction, float distance)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        int numHits = bulletCollider.Cast(direction, filter, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger AND is the player
            // in here, check if the player has been hit once
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Player"))
            {
                Vector2 position = this.gameObject.transform.position;

                // cast to the player controller
                PlayerFSMController pc = hits[i].collider.transform.GetComponent<PlayerFSMController>();
                if (!pc.GetFlameKB())
                {
                    pc.KnockbackTransition(damage, knockbackPower, position);
                    pc.SetFlameKB(true);
                } 
                return true;
            }
            //if the hit of the collider is NOT trigger AND is an enemy, still deal damage (friendly fire)
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Enemy"))
            {
                Vector3 position = this.gameObject.transform.position;
                //send all relative information to the player to take damage, and apply knockback
                //hits[i].collider.SendMessageUpwards("Damage", damage);

                return true;
            }
            //else if we hit a wall or another bullet
            else if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Wall") || !hits[i].collider.isTrigger && hits[i].collider.CompareTag("Bullet"))
            {

            }
        }
        return false;
    }

    /* OnCollisionEnter2D version of collision check
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the collision is a player
        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine("BulletIFrames");
            //get the position of the bullet
            Vector2 position = kbPosition;

            //make a clone of the player object
            PlayerFSMController pc = collision.transform.GetComponent<PlayerFSMController>();
            pc.KnockbackTransition(damage, knockbackPower, position);
        }

        if (collision.transform.CompareTag("Enemy"))
        {
            EnemyFSMController ec = collision.transform.GetComponent<EnemyFSMController>();
            //friendly fire. Call the function that has the enemy take damage.


            //make sure physics don't affect the enemy hit
            ec.rig.position = ec.currentPos;

        }

    }
    */

    public IEnumerator BulletIFrames()
    {
        //make enemy invincible
        gameObject.layer = LayerMask.NameToLayer("IFrames");

        //for extra time to move after flinching so that you can't immediately get hit after flinching
        yield return new WaitForSeconds(1);

        gameObject.layer = LayerMask.NameToLayer("Bullet");
    }
}
