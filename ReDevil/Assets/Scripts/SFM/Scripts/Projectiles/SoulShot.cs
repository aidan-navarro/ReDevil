using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShot : Bullet
{
    public float soulCost;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            EnemyFSMController ec = collision.transform.GetComponent<EnemyFSMController>();
            ec.TakeDamage(damage);
            Destroy(gameObject);
        }
            //if we hit a wall destroy the gameobject
            if (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Bullet"))
        {
            //destroy the bullet
            Destroy(gameObject);
        }
    }
}
