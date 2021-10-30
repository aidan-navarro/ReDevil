using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShot : Bullet
{
    public float soulCost;
    public PhysicsMaterial2D bulletWeakSpot;
    public PhysicsMaterial2D standardWeakSpot;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy") && onCamera)
        {
            EnemyFSMController ec = collision.transform.GetComponent<EnemyFSMController>();
            ec.TakeDamage(damage);
            if (DetectWeakspot(collision))
            {
                ec.OnWeakPointHit();
                ec.TakeDamage(damage);
            }
        }
    }

    private bool DetectWeakspot(Collider2D enemyMaterial)
    {
        //weakspot detection
        if (enemyMaterial.sharedMaterial == bulletWeakSpot || enemyMaterial.sharedMaterial == standardWeakSpot)
        {
            Debug.Log("Weakspot Detected. Deal extra damage");
            damage = damage * 1.5f;
            return true;
        }
        return false;
    }

}
