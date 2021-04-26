using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public LayerMask groundLayer;

    public float moveSpeed = 5f;
    public float jumpPower = 5f;
    public float airControl = 10f;
    public float dashLength = 5f;
    public float dashSpeed = 10f;
    public float downSpeed = 20f;
    public float slideSpeed = 4f;
    public float knockbackY = 5f;

    private float horizontal;
    public float vertical;
    private bool getLeftTriggerDown = false;
    private bool getRightTriggerDown = false;

    public Rigidbody2D rig;
    private BoxCollider2D col;
    private SpriteRenderer sprite;
    private bool playerFlipped = false;

    public bool isGrounded = false;
    private int direction = 1;
    private bool isTouchingWall = false;
    private Vector3 dashStartPos;
    private float dashDistance;
    private bool canDash = false;
    private float gravityScale;

    //health variables
    private float health;
    public float maxHealth = 100f;
    public Transform healthBar;
    public Text deathText;
    private bool damageDealt = false;

    //soul variables
    public float soul;
    public float startSoul = 50f;
    public float maxSoul = 300f;
    public Transform soulBarLvl1;
    public Transform soulBarLvl2;
    public Transform soulBarLvl3;

    public bool disableMove = false;
    public bool invincible = false;
    public float iFramesTimer;

    //knockback variables
    private Vector2 kbDirection;
    private float kbPower;

    //variables for attack function
    public bool facingLeft = false;

    //to reload current scene once dead
    public Scene scene;

    public enum PlayerMode { Move, Dash, Slide };
    private PlayerMode playerMode;

    void Start()
    {
        //player rigidbody
        rig = GetComponent<Rigidbody2D>();

        //box collider
        col = GetComponent<BoxCollider2D>();

        //sprite
        sprite = GetComponent<SpriteRenderer>();

        //set value for gravity based on rigs gravity scaling
        gravityScale = rig.gravityScale;

        Time.timeScale = 1f;
        scene = SceneManager.GetActiveScene();

        health = maxHealth;

        soul = startSoul;

        deathText.enabled = false;

    }

    void Update()
    {
        TouchingFloorOrWall();
        //check for movement
        if (disableMove == false)
        {
            Movement();
        }

        UpdateHealth(health, maxHealth);
        UpdateSoul(soul, maxHealth);
        //check if were dead
        Ripperoni(health);
    }

    public void TouchingFloorOrWall()
    {
        //equation values to determine if the player is on the ground
        Vector2 feetPos = col.bounds.center;
        feetPos.y -= col.bounds.extents.y;
        isGrounded = Physics2D.OverlapBox(feetPos, new Vector2(col.size.x - 0.2f, 0.1f), 0f, groundLayer.value);

        //equation values to determine if the player is on a wall
        Vector2 sidePos = col.bounds.center;
        sidePos.x += col.bounds.extents.x * direction;
        isTouchingWall = Physics2D.OverlapBox(sidePos, new Vector2(0.1f, col.size.y - 0.6f), 0f, groundLayer.value);
    }

    public void Movement()
    {
        if (playerMode == PlayerMode.Move)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (horizontal > 0f)
            {
                direction = 1;
                facingLeft = false;
                Vector2 newMoveSpeed = Vector2.right * moveSpeed;
                newMoveSpeed.y = rig.velocity.y;
                if (isGrounded)
                {
                    rig.velocity = newMoveSpeed;
                }
                else
                {
                    rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * airControl);
                }
                //sprite.flipX = false;
                FlipPlayer();
            }
            else if (horizontal < 0f)
            {
                direction = -1;
                facingLeft = true;
                Vector2 newMoveSpeed = Vector2.left * moveSpeed;
                newMoveSpeed.y = rig.velocity.y;
                if (isGrounded)
                {
                    rig.velocity = newMoveSpeed;
                }
                else
                {
                    rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * airControl);
                }
                //sprite.flipX = true;
                FlipPlayer();

            }
            else
            {
                Vector2 newMoveSpeed = Vector2.zero;
                newMoveSpeed.y = rig.velocity.y;
                if (isGrounded)
                {
                    rig.velocity = newMoveSpeed;
                }
                else
                {
                    rig.velocity = Vector2.Lerp(rig.velocity, newMoveSpeed, Time.deltaTime * airControl);
                }
            }

            if (isGrounded)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    Vector2 newVel = rig.velocity;
                    newVel.y = jumpPower;
                    rig.velocity = newVel;
                }

                canDash = true;
            }

            if (!isGrounded && !isTouchingWall)
            {
                if (Input.GetButtonUp("Jump"))
                {
                    if (rig.velocity.y > 0f)
                    {
                        Vector2 newVel = rig.velocity;
                        newVel.y = 0f;
                        rig.velocity = newVel;
                    }
                }
            }

            //wall jump
            if (isTouchingWall && Input.GetButtonDown("Jump"))
            {
                Vector2 newVel = rig.velocity;
                newVel.y = jumpPower;
                newVel.x = jumpPower * -direction / 2;
                rig.velocity = newVel;
            }
            else if (isTouchingWall && rig.velocity.y < 0f)
            {
                Vector2 newVel = rig.velocity;
                newVel.y = -moveSpeed / slideSpeed;
                rig.velocity = newVel;
            }
        }
        //dashing
        else if (playerMode == PlayerMode.Dash)
        {
            dashDistance = Mathf.Abs(dashStartPos.x - transform.position.x);
            rig.velocity = Vector3.right * direction * dashSpeed;

            //dashed max distance, end the dash.
            if (dashDistance >= dashLength)
            {
                rig.gravityScale = gravityScale;
                playerMode = PlayerMode.Move;
            }

            //hit a wall.  end the dash
            if (isTouchingWall)
            {
                rig.gravityScale = gravityScale;
                playerMode = PlayerMode.Move;
            }
        }
        //sliding
        else if (playerMode == PlayerMode.Slide)
        {
            //who knows if this will be a feature
        }

        //dashing input
        if (Input.GetAxisRaw("DashLeft") != 0)
        {
            if (!getLeftTriggerDown)
            {
                if (playerMode == PlayerMode.Move && canDash)
                {
                    rig.gravityScale = 0f;
                    dashStartPos = transform.position;
                    canDash = false;
                    playerMode = PlayerMode.Dash;
                }
                else if (playerMode == PlayerMode.Dash)
                {
                    rig.gravityScale = gravityScale;
                    playerMode = PlayerMode.Move;
                }
                getLeftTriggerDown = true;
            }
        }
        if (Input.GetAxisRaw("DashLeft") == 0)
        {
            getLeftTriggerDown = false;
        }

        if (Input.GetAxisRaw("DashRight") != 0)
        {
            if (!getRightTriggerDown)
            {
                if (playerMode == PlayerMode.Move && canDash)
                {
                    rig.gravityScale = 0f;
                    dashStartPos = transform.position;
                    canDash = false;
                    playerMode = PlayerMode.Dash;
                }
                else if (playerMode == PlayerMode.Dash)
                {
                    rig.gravityScale = gravityScale;
                    playerMode = PlayerMode.Move;
                }
                getRightTriggerDown = true;
            }
        }
        if (Input.GetAxisRaw("DashRight") == 0)
        {
            getRightTriggerDown = false;
        }
    }

    //old take damage functions-----------------------
    /*
    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        BaseMinion enemy = collision.GetComponent<BaseMinion>();
        if (invincible != true)
        {
            //if the player makes contact, deal knockback and damage
            if (collision.isTrigger != true && collision.CompareTag("Enemy"))
            {
                    //deal damage to the player
                    Damage(enemy.damage);

                    //call the player knockback function
                    StartCoroutine(InvincibleTimer(iFramesTimer));

                    //apply the knockback
                  //  KnockBack(enemy.gameObject, enemy.knockbackPower);

            }
        }
        else if (invincible == true)
        {
            //Fuck if I care the player is invincible
        }

    }

    //if we end up within the collison, apply the knockback
    public void OnTriggerStay2D(Collider2D collision)
    {
        BaseMinion enemy = collision.GetComponent<BaseMinion>();

        if (invincible != true) //the null reference exception occurs here
        {
            //if the player makes contact, deal knockback and damage
            if (collision.isTrigger != true && collision.CompareTag("Enemy"))
            {
                    //deal damage to the player
                    Damage(enemy.damage);

                    //call the player knockback function
                    StartCoroutine(InvincibleTimer(iFramesTimer));

                    //apply the knockback
                   // KnockBack(enemy.gameObject, enemy.knockbackPower);

            }
        }
        else if (invincible == true)
        {
            //Fuck if I care the player is invincible
        }

    }
    //------------------------------------------------
    */

    public IEnumerator InvincibleTimer(float iFrameCount)
    {
            invincible = true;
        gameObject.layer = LayerMask.NameToLayer("IFrames");


        //we disable movement so that your knockback animation plays and you can't act while flinching
        StartCoroutine(DisableMovement(iFrameCount / 4));

        //for extra time to move after flinching so that you can't immediately get hit after flinching
        yield return new WaitForSeconds(iFrameCount);

        
            invincible = false;
        gameObject.layer = LayerMask.NameToLayer("Default");

        damageDealt = false;
    }

    public IEnumerator DisableMovement(float iFrameCount)
    {
        if (!disableMove)
        {
            disableMove = true;
        }

        yield return new WaitForSeconds(iFrameCount);

        if (disableMove)
        {
            disableMove = false;
        }
    }

    //Knockback functions

    public void GetKnockbackPower(float knockbackPower)
    {
        kbPower = knockbackPower;

    }

    public void KnockBack(Vector2 pos)
    {
        if (!invincible)
        {
            //determine what direction we are hit
            Vector2 enemyPos = pos; //enemy position        
            Vector2 currentPos = this.gameObject.transform.position; //player position

            //set to playerMode.move so that we don't continue the dash after being hit
            playerMode = PlayerMode.Move;
            rig.gravityScale = gravityScale;

            //call the player knockback function
            StartCoroutine(InvincibleTimer(iFramesTimer));

            kbDirection = (currentPos - enemyPos).normalized;
            kbDirection = new Vector2(Mathf.Abs(kbDirection.x)/kbDirection.x, 1); //hard set y value to 1 to ensure enemy bounces up on knockback every time

            //Debug.Log(kbDirection);

            /*
            if (currentPos.x < enemyPos.x) //if player is to the left upon collision
            {
                kbDirection = new Vector2(-1, 1);
            }
            else if (currentPos.x > enemyPos.x) //if player is to the right of collision
            {
                kbDirection = new Vector2(1, 1);
            }
            */
            //under the EXTREMELY RARE CHANCE we land right on top of the enemy, set the knockback direction to be the opposite direction we are currently facing
            if (kbDirection.x == 0 && facingLeft) 
            {
                kbDirection = new Vector2(1, 1);
            }
            else if (kbDirection.x == 0 && !facingLeft)
            {
                kbDirection = new Vector2(-1, 1);
            }
            
            //ensure we change direction so we face away from the direction we are hit
            if ((kbDirection.x < 0 && facingLeft) || (kbDirection.x > 0 && !facingLeft))
            {
                facingLeft = !facingLeft;
                FlipPlayer();
            }

            //make sure velocity is doing some dumb shit
            //rig.velocity = new Vector2(0, 0);

            //rig.gravityScale = gravityScale;
            

            //apply the force to that bad boy and watch the player get knocked back

            //---old add velocity---
            //rig.AddForce(Vector2.Scale(kbDirection, new Vector2(kbPower, knockbackY)), ForceMode2D.Impulse);
            //---old add velocity---

            rig.velocity = Vector2.Scale(kbDirection, new Vector2(kbPower, knockbackY));
        }
    }

    public void UpdateHealth(float hp, float maxHP)
    {
        healthBar.localScale = new Vector3((hp / maxHP), 1f);
    }

    public void UpdateSoul(float soul, float maxHP)
    {
        //increase first soul bar to max
        if (soul <= 100)
        {
            soulBarLvl1.localScale = new Vector3((soul / maxHP), 1f);
            soulBarLvl2.localScale = new Vector3(0, 1f);
            soulBarLvl3.localScale = new Vector3(0, 1f);
        }
        if (soul > 100 && soul <= 200)
        {
            //increase the second soul bar
            soulBarLvl1.localScale = new Vector3((maxHP / maxHP), 1f);
            soulBarLvl2.localScale = new Vector3(((soul - 100) / maxHP), 1f);
            soulBarLvl3.localScale = new Vector3(0, 1f);
        }
        if (soul > 200)
        {
            soulBarLvl1.localScale = new Vector3((maxHP / maxHP), 1f);
            soulBarLvl2.localScale = new Vector3((maxHP / maxHP), 1f);
            soulBarLvl3.localScale = new Vector3((soul - 200) / maxHP, 1f);
        }

    }

    //increase soul for the player
    public void Soul(float soulInput)
    {
        soul += soulInput;
        if (soul > maxSoul)
        {
            soul = maxSoul;
        }
    }

    //deal damage to the player
    public void Damage(float dmg)
    {
        if (!invincible)
        {
            if (!damageDealt)
            {
                health -= dmg;
            }
            damageDealt = true;
        }
    }

    public void FlipPlayer()
    {
        if (facingLeft && !playerFlipped)
        {
            this.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3(-1f, 1f, 1f));
            playerFlipped = true;
        }
        else if (!facingLeft && playerFlipped)
        {
            this.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3(-1f, 1f, 1f));
            playerFlipped = false;
        }

    }

    //the player has died big oof
    public void Ripperoni(float hp)
    {
        if (hp <= 0)
        {
            //Make a coroutine to play death animation and display game over
            Time.timeScale = 0f;

            deathText.enabled = true;

            //eventually replace this with a seperate game over screen/UI element
            if (Input.GetButtonDown("Jump"))
            {
                SceneManager.LoadScene(scene.buildIndex);
            }

        }
    }
}
