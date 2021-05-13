using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//THIS SCRIPT HOLDS ALL VARIABLES NEEDED FOR ALL ATTACKS,
//INCLUDING INDIVIDUAL DAMAGE VARIABLES AND HITBOXES
public class PlayerAttack : MonoBehaviour
{
    // consider making a specific attack collider for dash attacks?

    public Collider2D attackCollider;
    public SpriteRenderer sprite;
    //to compare if we hit a weakspot
    public PhysicsMaterial2D weakSpot;
    public GameObject soulShot;

    //damage amounts for each hit of the ground hit chain
    public float groundHit1;
    public float groundHit2;
    public float groundHit3;
    public float dashAttackValue; // Point values for dash attacking

    //to count what number attack chain we're on
    public int groundHitCounter;

    //damage amount for airDownStrike
    public float airDownStrike;
    //endlag once the player has landed
    public float endlag;

    //used to detect what enemies have been hit by the rising/falling attack
    public List<Collider2D> enemyHitboxes = new List<Collider2D>();

    //the variable called to deal damage to enemies.  set value using variables used to set damage amounts
    private float damage;
    public bool attacking;

    // dash attack specific, only want to have the dash attack trigger once on hit
    public bool dashAttackContact; // going to get flicked back to false once it hits
    public bool airDashAttackContact; // dash attack in the air

    //transition bools
    public bool idleTransition;
    public bool dashTransition;

    public bool groundAttack2Transition;
    public bool groundAttack3Transition;

    public bool soulShotTransition;


    public bool checkCancel; //extra safety measure checking when we are able to cancel an attack;
    public bool GetCheckCancel() { return checkCancel; }
    public void SetCheckCancel(bool inCheckCancel) { checkCancel = inCheckCancel; }

    private PlayerFSMController pc;

    // Start is called before the first frame update
    void Start()
    {
        pc = gameObject.GetComponent<PlayerFSMController>();
        attacking = false;
        TurnOffHitbox();
        checkCancel = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Weakspot detection function
    public void DetectWeakspot(Collider2D enemyMaterial)
    {
        //weakspot detection
        if (enemyMaterial.sharedMaterial == weakSpot)
        {
            Debug.Log("Weakspot Detected. Deal extra damage");
            damage = damage * 1.5f;
        }
    }


    //------------------------------------------------------------
    //Ground Attack Functions
    //------------------------------------------------------------
    #region //click the plus sign beside region to hide/unhide code

    public void GroundAttack()
    {
        //Start the coroutine
        StartCoroutine("EnableGroundHit");
    }

    public void StopGroundAttack()
    {
        Debug.Log("stopping ground Attack coroutine");
        //In case an attack is not being cancelled out of
        TurnOffHitbox();
        attacking = false;
        StopCoroutine("EnableGroundHit");
    }


    public IEnumerator EnableGroundHit()
    {
        attacking = true;
        //update the combo count
        groundHitCounter++;
        Debug.Log("GroundHitCount: " + groundHitCounter);
        //Turn the hitbox on to deal massive damage
        TurnOnHitbox();

        switch (groundHitCounter)
        {
            case 1:
                damage = groundHit1;
                break;
            case 2:
                damage = groundHit2;
                break;
            case 3:
                damage = groundHit3;
                break;
        }

        //old if statement.  replaced by above switch statement for practice reasons
        /*
        if (groundHitCounter == 1)
        {
            damage = groundHit1;
        }
        else if (groundHitCounter == 2)
        {
            damage = groundHit2;
        }
        else if (groundHitCounter == 3)
        {
            damage = groundHit3;
        }
        */

        // lock the player's position 
        pc.GetRigidbody2D().velocity = new Vector2(0, 0);

        //let a hit process
        CheckGroundHit(attackCollider, transform.forward, 10);

        //let the hit process so that we don't end up cancelling before damage is dealt
        yield return new WaitForSeconds(0.1f); 

        //allow the player state to check for a cancel (dash or attack chain)
        checkCancel = true;

        //play the animation here

        //wait for the attack to end.  During this timeframe the attack can be cancelled
        yield return new WaitForSeconds(endlag);

        //turn off the hitbox
        TurnOffHitbox();

        groundHitCounter = 0;

        //we can't check for a cancel anymore, set to false
        checkCancel = false;

        attacking = false;

        idleTransition = true;
        Debug.Log("attack coroutine end");
    }

    //collision detection for the hit utilizing collider.cast method for the ground chain attack
    private bool CheckGroundHit(Collider2D playerAttackCol, Vector2 direction, float distance)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        int numHits = playerAttackCol.Cast(direction, filter, hits, distance);
        Debug.Log("Number Ground Hits: " + numHits);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger AND is an enemy
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Enemy"))
            {
                EnemyFSMController ec = hits[i].transform.GetComponent<EnemyFSMController>();
                Collider2D eCollider = hits[i].collider.GetComponent<Collider2D>();

                DetectWeakspot(eCollider);

                Vector3 position = this.gameObject.transform.position;  // this isn't getting used

                //store the amount of hp the enemy has before the initial hit
                float pastHealth = ec.health;

                //send all relative information to the player to take damage, and apply knockback
                ec.TakeDamage(damage);

                //store the amount of hp the enemy has after the hit
                float presentHealth = ec.health;

                //if the present health goes below 0, set it to zero since you can't steal a negative soul value
                if (presentHealth < 0)
                {
                    presentHealth = 0;
                }

                //gain soul equal to the damage dealt to the enemy.
                pc.SoulCalculator(pastHealth - presentHealth);

                return true;
            }
        }
        return false;
    }
    #endregion

    //------------------------------------------------------------
    // Dash Attack Functions
    //------------------------------------------------------------
    #region click plus sign to hide/unhide code
    public void StartDashAttack()
    {
        //StartCoroutine("EnableDashAttack");
        Debug.Log("Start Dash Attack");
        attacking = true; // use the same attacking variable?
        TurnOnHitbox();
        ShrinkHitbox();
        damage = dashAttackValue;
        CheckDashAttackHit(attackCollider, transform.forward, 10);
        Debug.Log("Ground Dash Attack: " + dashAttackContact);
        Debug.Log("Air Dash Attack: " + airDashAttackContact);
    }

    public void EndDashAttack()
    {
        attacking = false;
        TurnOffHitbox();
        RevertHitbox();
        //StopCoroutine("EnableDashAttack");
    }

    //// Need an IEnumerator? don't need to delay it... 
    //// unless I need to keep the duration of the box check
    //public IEnumerator EnableDashAttack()
    //{
    //    attacking = true;
    //    yield return new WaitForSeconds(0.5f);
    //}

    private void CheckDashAttackHit(Collider2D playerAttackCol, Vector2 direction, float distance)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        int numHits = playerAttackCol.Cast(direction, filter, hits, distance);
        Debug.Log("NumHits: " + numHits);

        for (int i = 0; i < numHits; i++)
        {
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Enemy") && attacking) // this only registers frame 1
            {
                EnemyFSMController ec = hits[i].transform.GetComponent<EnemyFSMController>();
                Collider2D eCollider = hits[i].collider.GetComponent<Collider2D>();
                
                DetectWeakspot(eCollider);

                //store the amount of hp the enemy has before the initial hit
                float pastHealth = ec.health;

                //send all relative information to the player to take damage, and apply knockback
                ec.TakeDamage(damage);

                //store the amount of hp the enemy has after the hit
                float presentHealth = ec.health;

                //if the present health goes below 0, set it to zero since you can't steal a negative soul value
                if (presentHealth < 0)
                {
                    presentHealth = 0;
                }

                //gain soul equal to the damage dealt to the enemy.
                pc.SoulCalculator(pastHealth - presentHealth);

                attacking = false;
                if (pc.GetisGrounded())
                {
                    Debug.Log("Ground Hit");
                    dashAttackContact = true;
                }// now take into account of knockback
                else if (!pc.GetisGrounded())
                {
                    Debug.Log("Air Hit");
                    airDashAttackContact = true;
                }
            }
        }
    }
    #endregion

    //------------------------------------------------------------
    // Soul Shot Functions
    //------------------------------------------------------------
    public void SoulShotAttack()
    {
        StartCoroutine("EnableSoulShotAttack");
    }
    public IEnumerator EnableSoulShotAttack()
    {
        attacking = true;
        pc.GetRigidbody2D().velocity = new Vector2(0, 0);

        //fire off a soul shot in the direction the player is facing
        GameObject soulShotBullet = Instantiate(soulShot, attackCollider.gameObject.transform.position, soulShot.transform.rotation);
        if(pc.facingLeft)
        {
            soulShotBullet.GetComponent<Bullet>().direction = new Vector2(-1, 0);
        }
        else
        {
            soulShotBullet.GetComponent<Bullet>().direction = new Vector2(1, 0);
        }

        pc.SoulCalculator(-soulShotBullet.GetComponent<SoulShot>().soulCost);

        //let the hit process so that we don't end up cancelling before damage is dealt
        yield return new WaitForSeconds(0.1f);

        //allow the player state to check for a cancel (dash or attack chain)
        checkCancel = true;

        //play the animation here

        //wait for the attack to end.  During this timeframe the attack can be cancelled
        yield return new WaitForSeconds(endlag);

        //we can't check for a cancel anymore, set to false
        checkCancel = false;

        attacking = false;

        idleTransition = true;
        Debug.Log("soul shot attack coroutine end");
    }


    //------------------------------------------------------------
    //Air Attack Functions
    //------------------------------------------------------------
    #region //click the plus sign beside region to hide/unhide code

    public void AirDownStrikeAttack()
    {
        //Start the coroutine
        StartCoroutine("EnableAirDownStrike");
    }

    public void StopAirDownStrikeAttack()
    {
        Debug.Log("stopping coroutine");
        //In case an attack is not being cancelled out of
        TurnOffHitbox();
        attacking = false;
        StopCoroutine("EnableAirDownStrike");
    }

    public IEnumerator EnableAirDownStrike()
    {
        Debug.Log("Down Input");

        attacking = true;
        
        //disable all movement of the player so we can't strafe in midair.  We want to move straight down and only straight down.
        pc.GetRigidbody2D().velocity = new Vector2(0, 0);

        //set velocity to the downward motion
        pc.GetRigidbody2D().velocity = Vector3.down * pc.dashSpeed;

        //while the player is not grounded
        while (!pc.GetisGrounded())
        {
            //check if we touched the floor if we did.  break.
            pc.TouchingFloorOrWall();
            if (pc.GetisGrounded())
            {
                break;
            }

            TurnOnHitbox();
            //turn on check cancel to allow for a knockback cancel
            checkCancel = true;

            damage = airDownStrike;
            
            //check if a hitbox is hitting an enemy
            CheckAirDownStrikeHit(attackCollider, transform.forward, 10);
            
            yield return null;
        }

        //WE HAVE LANDED.  STOP ATTACKING
        //clear the list so we can hit enemies with this attack again
        enemyHitboxes.Clear();

        //do not allow cancels anymore
        checkCancel = false;
        //turn off the hitbox
        TurnOffHitbox();
        //wait for the endlag, you're not getting out of this powerful attack scot free
        yield return new WaitForSeconds(endlag);
        //tell the player were not attacking anymore
        attacking = false;
        idleTransition = true;
    }

    //collision detection for the hit utilizing collider.cast method for the ground chain attack
    private bool CheckAirDownStrikeHit(Collider2D playerAttackCol, Vector2 direction, float distance)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        int numHits = playerAttackCol.Cast(direction, filter, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            //if the hit of the collider is NOT trigger AND is an enemy
            if (!hits[i].collider.isTrigger && hits[i].collider.CompareTag("Enemy"))
            {
                EnemyFSMController ec = hits[i].transform.GetComponent<EnemyFSMController>();
                Collider2D eCollider = hits[i].collider.GetComponent<Collider2D>();

                Vector3 position = this.gameObject.transform.position;

                //check for weakspot
                DetectWeakspot(eCollider);

                //if the enemy hitbox isnt in the list, add to the list
                if (enemyHitboxes.Contains(eCollider))
                {
                    Debug.Log("already in the list");
                    damage = 0;
                }
                else if(!enemyHitboxes.Contains(eCollider))
                {
                    Debug.Log("this isnt in the list");
                    enemyHitboxes.Add(eCollider);
                }

                //store the amount of hp the enemy has before the initial hit
                float pastHealth = ec.health;

                //send all relative information to the player to take damage, and apply knockback
                ec.TakeDamage(damage);

                //store the amount of hp the enemy has after the hit
                float presentHealth = ec.health;

                //if the present health goes below 0, set it to zero since you can't steal a negative soul value
                if (presentHealth < 0)
                {
                    presentHealth = 0;
                }

                //gain soul equal to the damage dealt to the enemy.
                pc.SoulCalculator(pastHealth - presentHealth);

                return true;
            }
        }
        return false;
    }
    #endregion

    //------------------------------------------------------------
    //Hitbox Handler Functions (turn on/off hitboxes)
    //------------------------------------------------------------
    //hitbox handler
    private void TurnOnHitbox()
    {
        attackCollider.enabled = true;
        sprite.enabled = true;
    }

    //for end of the coroutine, turn off the hitbox
    private void TurnOffHitbox()
    {
        attackCollider.enabled = false;
        sprite.enabled = false;
    }

    // Dash Attack Specific, scaling down the hitbox so that the player is a bit closer to the enemy on dash
    private void ShrinkHitbox()
    {
        attackCollider.transform.localScale = new Vector2(0.5f, 0.5f);
    }

    private void RevertHitbox()
    {
        attackCollider.transform.localScale = new Vector2(1.0f, 1.0f);
    }

    //------------------------------------------------------------
    //cancels
    //------------------------------------------------------------
    #region //click the plus sign beside region to hide/unhide code

    public void CheckAttackCancel()
    {
        if (checkCancel)
        {
            //check which cancel has occured
            //ground attack cancel
            if (pc.GetAttackButtonDown() && pc.GetisGrounded())
            {
                //turn on variable to change to the correct state
                //use ground hit counter to determine which state is correct
                
                // Vincent's notes: I'm looking into the inspector and I don't see this getting triggered
                switch (groundHitCounter)
                {
                    case 1: //we are currently on first ground attack
                        StopGroundAttack();
                        groundAttack2Transition = true;
                        checkCancel = false;
                        break;
                    case 2: //we are currently on second ground attack
                        StopGroundAttack();
                        groundAttack3Transition = true;
                        checkCancel = false;
                        break;
                    default:
                        Debug.Log("something went horribly wrong trying to switch attack chain states");
                        break;
                }

            }

        }

    }

    public void CheckDashCancel()
    {
        //pc.CheckDashInput();

        // whenever the window for cancel is true, the player can act into a dash transition
        if (checkCancel)
        {
            // this should transition into the ground attack
            if ((pc.leftTriggerDown || pc.rightTriggerDown) && pc.GetisGrounded())
            {
                Debug.Log("Dash Cancel Input");
                checkCancel = false;
                groundHitCounter = 0;
                //turn on variable for dash cancel
                StopGroundAttack();
                StopAirDownStrikeAttack();
                dashTransition = true;
                Debug.Log(dashTransition);
            }
        }
    }

    public void CheckKnockbackCancel()
    {
        if(checkCancel)
        {
            if (pc.GetKbTransition())
            {
                Debug.Log(pc.GetKbTransition());
                checkCancel = false;
                groundHitCounter = 0;
                StopGroundAttack();
                StopAirDownStrikeAttack();
            }
        }
        
    }

    public void ReInitializeTransitions()
    {
        Debug.Log("reset transitions");
        idleTransition = false;
        dashTransition = false;
        dashAttackContact = false; // placing here so that the GDA state can pick up the contact first before changing the boolean back
        airDashAttackContact = false;
        groundAttack2Transition = false;
        groundAttack3Transition = false;
    }
    #endregion
}
