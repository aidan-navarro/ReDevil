using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShot : Bullet
{
    public float soulCost;
    

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy") && onCamera)
        {
            EnemyFSMController ec = collision.transform.GetComponent<EnemyFSMController>();
            ec.TakeDamage(damage); 
        }
    }

}
