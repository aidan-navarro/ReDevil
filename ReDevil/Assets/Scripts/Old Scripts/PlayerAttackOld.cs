using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//for everything related to player attacking.  the different attacks, damage dealing, and soul charge is handled here.
public class PlayerAttackOld : MonoBehaviour
{
    //hitbox damage, temp sprite to show it works, and damage variable
    public Collider2D hitBox;
    public SpriteRenderer sprite;
    private float dmg;
    private float soulValue;

    //values to deal damage based on the hit
    public float groundHit1;
    public float groundHit2;
    public float groundHit3;
    public float airFallStrike;

    //bool to determine what air attack we are using
    private bool airDownStrike = false;
    private int airStrikeSoulCount;

    //to count what combo number we're on
    private int comboCount;

    //player variable so we can determine the direction
    private Player player;

    //variable to determine the enemy
    private BaseMinion enemy;

    IEnumerator enableHit;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        TurnOffHitbox();
        enableHit = EnableHit();
        airStrikeSoulCount = 0;
    }

    //this might not be needed
    private void Update()
    {
        //see if the player has input the air finish attack
        if(player.vertical <= -0.45f && Input.GetButtonDown("Attack"))
        {
            AirFinishAttack();
        }

        //see if the player has input the ground attack
        if(Input.GetButtonDown("Attack") && player.isGrounded)
        {
            GroundAttack();
        }

        
        //Allow for a Dash Cancel
        DashCancel();
    }

    private void AirFinishAttack()
    {
            StartCoroutine("AirDownStrike");
    }

    private void DashCancel()
    {
        //this function is kind of a cheat and not really a new attack.  its basically stopping coroutines while the dash function from player.cs is being called
        if(!player.disableMove)
        {
            if (Input.GetAxisRaw("DashLeft") != 0 || Input.GetAxisRaw("DashRight") != 0)
            {
                TurnOffHitbox();
                player.disableMove = false;
                StopCoroutine("EnableHit");
                comboCount = 0;
            }
        }
        
    }

    public void GroundAttack()
    {
            if (comboCount < 3)
            {
                TurnOffHitbox();
                StopCoroutine("EnableHit");
                StartCoroutine("EnableHit");
            }             
    }

    private bool CheckGroundAttack(Collider2D hitboxCollider, Vector2 direction, float distance)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        int numHits = hitboxCollider.Cast(direction, filter, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger AND is an enemy
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Enemy"))
            {
                Vector3 position = this.gameObject.transform.position;
                //send all relative information to the player to take damage, and apply knockback
                hits[i].collider.SendMessageUpwards("Damage", dmg);
                player.Soul(soulValue);

                //if we are doing an air strike, do special air strike things like disabling multiple hits to damage
                //and multiple sould charges                

                return true;
            }
        }
        return false;
    }

    private bool CheckAirDownStrike(Collider2D hitboxCollider, Vector2 direction, float distance)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        int numHits = hitboxCollider.Cast(direction, filter, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger AND is an enemy
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Enemy"))
            {
                Vector3 position = this.gameObject.transform.position;
                //send all relative information to the player to take damage, and apply knockback
                hits[i].collider.SendMessageUpwards("Damage", dmg);

                //if we already dealt soul damage, set to 0 so we do not deal it a second time
                if (airStrikeSoulCount > 0)
                {
                    soulValue = 0;
                }
                player.Soul(soulValue);

                //if we are doing an air strike, do special air strike things like disabling multiple hits to damage
                //and multiple sould charges
                if (airDownStrike)
                {
                    hits[i].collider.SendMessageUpwards("AirDownStrikeHit");
                    airStrikeSoulCount++;
                }
                return true;
            }
        }
        return false;
    }

    public IEnumerator EnableHit()
    {
        //update the combo count
        comboCount++;

        //Turn the hitbox on to deal massive damage
        TurnOnHitbox();
        if (comboCount == 1)
        {
            dmg = groundHit1;
            soulValue = groundHit1 / 2;
        }
        else if(comboCount == 2)
        {
            dmg = groundHit2;
            soulValue = groundHit2 / 2;
        }
        else if(comboCount == 3)
        {
            dmg = groundHit3;
            soulValue = groundHit3 / 2;
        }

        player.disableMove = true;
        player.rig.velocity = new Vector2(0, 0);

        //check if a hitbox is hitting an enemy
        CheckGroundAttack(hitBox, transform.forward, 10);

        Debug.Log("Damage Value for Attack" + comboCount +": " + dmg);
        
        
        //play the animation here

        //wait a bit, let the hit sink in
        yield return new WaitForSeconds(0.5f);

        //turn off the hitbox

        player.disableMove = false;
        TurnOffHitbox();
        
        comboCount = 0;
    }

    public IEnumerator AirDownStrike()
    {
        Debug.Log("Down Input");
        airDownStrike = true;
        //disable all movement of the player so we can't strafe in midair.  We want to move straight down and only straight down.
        player.disableMove = true;
        player.rig.velocity = new Vector2(0, 0);

        player.rig.velocity = Vector3.down * player.dashSpeed;

        while (!player.isGrounded)
        {
            TurnOnHitbox();
            dmg = airFallStrike;
            soulValue = airFallStrike / 2;
            //check if a hitbox is hitting an enemy
            CheckAirDownStrike(hitBox, transform.forward, 10);
            yield return null;
        }

        airStrikeSoulCount = 0;
        yield return null;
        player.disableMove = false;
        airDownStrike = false;
        TurnOffHitbox();

        
    }
    //----------------------------------------------
    //old attack function.  did not work due to nature of spawning a hitbox within an enemy.  oncollisionstay did not work
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.isTrigger == true && collision.CompareTag("Enemy"))
        {
            //deal massive damage big kek
            collision.GetComponent<BaseMinion>().Damage(dmg);
            //charge up that super meter for all the powers and charge shot
            player.Soul(soulValue);
        }
    }
    */

    //determine which way the opponent is facing, and turn on the hitbox
    private void TurnOnHitbox()
    {
        hitBox.enabled = true;
        sprite.enabled = true;
    }

    //for end of the coroutine, turn off the hitbox
    private void TurnOffHitbox()
    {
        hitBox.enabled = false;
        sprite.enabled = false;
    }
}
