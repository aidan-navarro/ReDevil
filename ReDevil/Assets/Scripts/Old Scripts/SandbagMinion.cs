using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandbagMinion : BaseMinion
{
    //this is just a sandbag.  It takes damage.  And dies

    private void Update()
    {
        //check if the enemy is dead
        Dead();

        ResetAirDownStrikeHit();
        
        foreach (Collider2D eColliders in enemyCollider)
        {
            //run the check collisions function and determine if a collision has occured
            CheckPlayerCollisions(eColliders, transform.forward, 10);
        }
        
        //check the range since we know were still alive
        CheckRange(player.transform, ref canAttack);
    }

    protected override void Attack(bool attackRight, bool attack)
    {
       //were a sandbag do nothing
    }
}
