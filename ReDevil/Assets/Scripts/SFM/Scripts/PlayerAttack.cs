using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//THIS SCRIPT HOLDS ALL VARIABLES NEEDED FOR ALL ATTACKS,
//INCLUDING INDIVIDUAL DAMAGE VARIABLES AND HITBOXES
public class PlayerAttack : MonoBehaviour
{
    public Collider2D attackCollider;
    public SpriteRenderer sprite;
    //to compare if we hit a weakspot
    public PhysicsMaterial2D weakSpot;

    //damage amounts for each hit of the ground hit chain
    public float groundHit1;
    public float groundHit2;
    public float groundHit3;

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

    //transition bools
    public bool idleTransition;
    public bool dashTransition;
    public bool groundAttack2Transition;
    public bool groundAttack3Transition;


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
        Debug.Log("stopping coroutine");
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

        pc.rig.velocity = new Vector2(0, 0);

        //let a hit process
        CheckGroundHit(attackCollider, transform.forward, 10);

        //let the hit process so that we don't end up cancelling before damage is dealt
        yield return new WaitForSeconds(0.1f); // may be too small of a window for ground lag

        //allow the player state to check for a cancel (dash or attack chain)
        checkCancel = true;

        //play the animation here

        //wait for the attack to end.  During this timeframe the attack can be cancelled
        yield return new WaitForSeconds(endlag);

        //turn off the hitbox
        TurnOffHitbox();

        //we can't check for a cancel anymore, set to false
        checkCancel = false;

        attacking = false;

        groundHitCounter = 0;
    }

    //collision detection for the hit utilizing collider.cast method for the ground chain attack
    private bool CheckGroundHit(Collider2D playerAttackCol, Vector2 direction, float distance)
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

                DetectWeakspot(eCollider);

                Vector3 position = this.gameObject.transform.position;

                //store the amount of hp the enemy has before the initial hit
                float pastHealth = ec.health;

                //send all relative information to the player to take damage, and apply knockback
                ec.TakeDamage(damage);

                //store the amount of hp the enemy has after the hit
                float presentHealth = ec.health;

                //if the present health goes below 0, set it to zero since you can't steal a negative soul value
                if(presentHealth < 0)
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
        pc.rig.velocity = new Vector2(0, 0);

        //set velocity to the downward motion
        pc.rig.velocity = Vector3.down * pc.dashSpeed;

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
            if (Input.GetButtonDown("Attack") && pc.GetisGrounded())
            {
                //turn on variable to change to the correct state
                //use ground hit counter to determine which state is correct
                
                // Vincent's notes: I'm looking into the inspector and I don't see this getting triggered
                switch (groundHitCounter)
                {
                    case 1: //we are currently on first ground attack
                        groundAttack2Transition = true;
                        checkCancel = false;
                        break;
                    case 2: //we are currently on second ground attack
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
        pc.CheckDashInput();

        // whenever the window for cancel is true, the player can act into a dash transition
        if (checkCancel)
        {
            if ((pc.leftTriggerDown || pc.rightTriggerDown) && pc.GetisGrounded())
            {
                Debug.Log("Dash Cancel Input");
                checkCancel = false;
                groundHitCounter = 0;
                //turn on variable for dash cancel
                StopGroundAttack();
                StopAirDownStrikeAttack();
                dashTransition = true;
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
        idleTransition = false;
        dashTransition = false;
        groundAttack2Transition = false;
        groundAttack3Transition = false;
    }
    #endregion
}
