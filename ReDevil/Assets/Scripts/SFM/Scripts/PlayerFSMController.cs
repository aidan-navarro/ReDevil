using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//FSM Class for the Player which also contains variables + unique functions for the player
public class PlayerFSMController : AdvancedFSM

{
    //*******************************************************************
    //Variables for the player
    //*******************************************************************

    //-------------------------------------------------------------------
    //Floor and Wall Collision Variables
    //-------------------------------------------------------------------
    private BoxCollider2D col; //the players box collider
    public LayerMask groundLayer;

    public Text stateText;

    //-------------------------------------------------------------------
    //meter variables
    //-------------------------------------------------------------------
    public float health;

    //get and set functions for health
    public float GetHealth() { return health; }
    public void SetHealth(float inHealth) { health = inHealth; }


    //soul is a meter that builds when hitting enemies.  allows use of soul armaments and soul shot

    public float soul;
    //get and set functions for soul
    public float GetSoul() { return soul; }
    public void SetSoul(float insoul) { soul = insoul; }

    //-------------------------------------------------------------------
    //Variables for taking damage and knockback
    //-------------------------------------------------------------------
    //we need vairables to determine how much damage is taken, how strong the knockback is, and the position of the enemy
    //to ensure we are knocked back in the correct direction
    //then we need a get and set function to transition to knockback state


    private float damage; //this is the damage that will be dealt to the player
    public float GetDamage() { return damage; }
    public void SetDamage(float inDamage) { damage = inDamage; }

    private Vector2 enemyPos; //this is the position of the enemy used to determine what direction to apply knockback
    public Vector2 GetEnemyPos() { return enemyPos; }
    public void SetEnemyPos(Vector2 inEnemyPos) { enemyPos = inEnemyPos; }

    private float knockbackPower; //used to determine strength of the knockback.
    public float GetKnockbackPower() { return knockbackPower; }
    public void SetKnockbackPower(float inKnockbackPower) { knockbackPower = inKnockbackPower; }

    private bool kbTransition; //when this bool value is true, transition to KB State.  Reset to false in iFrames so that we can be knocked back again.
    public bool GetKbTransition() { return kbTransition; }
    public void SetKbTransition(bool inKbTransition) { kbTransition = inKbTransition; }

    private bool immobile; //when this bool value is true, transition to KB State.  Reset to false in iFrames so that we can be knocked back again.
    public bool GetImmobile() { return immobile; }
    public void SetImmobile(bool inImmobile) { immobile = inImmobile; }

    private bool invincible; //to detect if we are in iFrames.  ONLY DO DAMAGE AND KNOCKBACK IF THIS BOOL IS FALSE
    public bool GetInvincible() { return invincible; }
    public void SetInvincible(bool inInvincible) { invincible = inInvincible; }

    //-------------------------------------------------------------------
    //movement variables
    //-------------------------------------------------------------------

    public Rigidbody2D rig;
    private float gravityScale;

    public float moveSpeed;
    //get and set functions for movement speed
    public float GetMoveSpeed() { return moveSpeed; }
    public void SetMoveSpeed(float inMoveSpeed) { moveSpeed = inMoveSpeed; }

    //for determining movement speed in the air
    public float airControl;

    //for determining wall slide speed
    public float slideSpeed;

    //dashing variables
    public float dashSpeed; //how fast do we dash
    public float dashLength; //how far do we dash
    private bool canDash;
    public bool GetCanDash() { return canDash; }
    public void SetCanDash(bool inCanDash) { canDash = inCanDash; }

    private bool dashInputAllowed;
    public bool GetDashInputAllowed() { return dashInputAllowed; }
    public void SetDashInputAllowed(bool inDashInputAllowed) { dashInputAllowed = inDashInputAllowed; }

    private Vector2 dashStartPos; //for tracking the start position of the dash
    public Vector2 GetDashStartPos() { return dashStartPos; }
    public void SetDashStartPos(Vector2 inDashStartPos) { dashStartPos = inDashStartPos; }

    //-------------------------------------------------------------------
    //variables to detect controller input
    //-------------------------------------------------------------------
    [System.NonSerialized]
    public float horizontal;
    [System.NonSerialized]
    public float vertical;

    [System.NonSerialized]
    public bool leftTriggerDown;
    [System.NonSerialized]
    public bool rightTriggerDown;

    //variables for determining direction faced
    [System.NonSerialized]
    public bool facingLeft = false; // This bool is to help position attack hitbox
    private bool playerFlipped = false; //this bool is to flip the sprite in FlipPlayer() Function.

    [System.NonSerialized]
    public int direction = 1; //For calculating direction for other movement options.  Must be set whenever player moves in different direction


    //jump variables
    private bool isGrounded; //variable to determine if the player is grounded.  the player can only jump if on the ground.
    public bool GetisGrounded() { return isGrounded; }
    public void SetisGrounded(bool inIsGrounded) { isGrounded = inIsGrounded; }

    private bool isTouchingWall;
    public bool GetisTouchingWall() { return isTouchingWall; }
    public void SetisTouchingWall(bool inIsTouchingWall) { isTouchingWall = inIsTouchingWall; }

    public float jumpPower;

    

    //initialize FSM
    protected override void Initialize()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        rig = GetComponent<Rigidbody2D>();
        //set value for gravity based on rigs gravity scaling
        gravityScale = rig.gravityScale;

        health = 100;

        leftTriggerDown = false;
        rightTriggerDown = false;

        canDash = true;
        dashInputAllowed = true;
        invincible = false;

        //box collider
        col = GetComponent<BoxCollider2D>();

        ConstructFSM();
    }

    protected override void FSMUpdate()
    {
        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);
    }

    private void ConstructFSM()
    {
        //
        //Create States
        //

        //create the Idling state
        IdlingState idling = new IdlingState();

        //create transitions for the follow state
        // TO ADD: add the transition to when I execute a ground dash attack from idling 
        idling.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if i die while idle, transition to dead
        idling.AddTransition(Transition.Move, FSMStateID.Moving);  // if I start moving on ground while idle, transition to moving
        idling.AddTransition(Transition.Jump, FSMStateID.Jumping); // if i jump while idle, transition to Jump State
        idling.AddTransition(Transition.Dash, FSMStateID.Dashing); // if i press the dash button, transition to dash state
        idling.AddTransition(Transition.DashAttack, FSMStateID.DashAttacking);
        idling.AddTransition(Transition.WallJump, FSMStateID.WallJumping);
        idling.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player
        idling.AddTransition(Transition.GroundAttack1, FSMStateID.GroundFirstStrike);

        //create the Moving state
        MoveState moving = new MoveState();

        //create transitions for the follow state
        // TO ADD: add the transition to when I execute a ground dash attack from moving 

        moving.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if i die while moving, transition to dead
        moving.AddTransition(Transition.Idle, FSMStateID.Idling);  //If i stop moving, transition to idling
        moving.AddTransition(Transition.Jump, FSMStateID.Jumping); // if i jump while idle, transition to jumping
        moving.AddTransition(Transition.Airborne, FSMStateID.Midair); //if i walk off an edge without jumping, transition to midair movement
        moving.AddTransition(Transition.Dash, FSMStateID.Dashing);
        moving.AddTransition(Transition.DashAttack, FSMStateID.DashAttacking); // If I'm moving currently, go into a dash attack
        moving.AddTransition(Transition.WallJump, FSMStateID.WallJumping);
        moving.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        //create the Dashing state
        DashState dashing = new DashState();

        //create transitions for the dash state
        dashing.AddTransition(Transition.NoHealth, FSMStateID.Dead); //if i die while dashing, transition to dead
        dashing.AddTransition(Transition.Idle, FSMStateID.Idling); //if dash ends on the ground OR they hit a wall while on ground, idle
        dashing.AddTransition(Transition.WallSlide, FSMStateID.WallSliding); //if dash ends in midair and hit a wall, wall sliding
        dashing.AddTransition(Transition.Airborne, FSMStateID.Midair); //if the dash ends midair, airborne
        dashing.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        // Addition: State for a ground dash attack
        GroundDashAttack groundDash = new GroundDashAttack();


        //create the Wall Slide state
        WallSlideState wallSliding = new WallSlideState();

        //create transitions for the follow state
        wallSliding.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if i die while wall sliding, transition to dead
        wallSliding.AddTransition(Transition.Idle, FSMStateID.Idling);  //If i land on the ground, transition to idling
        wallSliding.AddTransition(Transition.Airborne, FSMStateID.Midair); //if i move off the wall, transition to airborne
        wallSliding.AddTransition(Transition.WallJump, FSMStateID.WallJumping); //if i jump off the wall, transition to wall jumping
        wallSliding.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        //create the Midair state
        MidairState midair = new MidairState();

        //create transitions for the follow state
        midair.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if i die while midair, transition to dead
        midair.AddTransition(Transition.Idle, FSMStateID.Idling);  //If i land on the ground, transition to idling
        midair.AddTransition(Transition.WallSlide, FSMStateID.WallSliding);  //if i touch a wall while falling, transition to sall sliding
        midair.AddTransition(Transition.Dash, FSMStateID.Dashing);
        midair.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player
        midair.AddTransition(Transition.AirDownStrike, FSMStateID.AirDownStrike);//air down strike

        //create the Jumping state
        JumpingState jumping = new JumpingState();

        //create transitions for the follow state
        jumping.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if i die while jumping, transition to dead
        jumping.AddTransition(Transition.Airborne, FSMStateID.Midair); //if i complete my jump, transition to midair movement
        jumping.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player
        jumping.AddTransition(Transition.AirDownStrike, FSMStateID.AirDownStrike);//air down strike

        //create Wall Jumping State
        WallJumpState wallJumping = new WallJumpState();

        //create transitions for the follow state
        wallJumping.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if i die while jumping, transition to dead
        wallJumping.AddTransition(Transition.Airborne, FSMStateID.Midair); //if i complete my jump, transition to midair movement
        wallJumping.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        //create the knockback state
        KnockbackState knockback = new KnockbackState();

        //create transitions for the knockback state
        knockback.AddTransition(Transition.NoHealth, FSMStateID.Dead);  //if i die while knockback, transition to dead
        knockback.AddTransition(Transition.Idle, FSMStateID.Idling);
        knockback.AddTransition(Transition.Airborne, FSMStateID.Midair);
        knockback.AddTransition(Transition.WallSlide, FSMStateID.WallSliding);
        //if the knockback has occured, transition to iframes;

        GroundAttack1State ga1 = new GroundAttack1State();

        ga1.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        ga1.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack just ends
        ga1.AddTransition(Transition.Dash, FSMStateID.Dashing); // dash cancel
        ga1.AddTransition(Transition.GroundAttack2, FSMStateID.GroundSecondStrike);
        ga1.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        GroundAttack2State ga2 = new GroundAttack2State();

        ga2.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        ga2.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack just ends
        ga2.AddTransition(Transition.Dash, FSMStateID.Dashing); // dash cancel
        ga2.AddTransition(Transition.GroundAttack3, FSMStateID.GroundThirdStrike);
        ga2.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        GroundAttack3State ga3 = new GroundAttack3State();

        ga3.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        ga3.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack just ends
        ga3.AddTransition(Transition.Dash, FSMStateID.Dashing); // dash cancel
        ga3.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        AirDownStrikeState airDownStrike = new AirDownStrikeState();

        airDownStrike.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        airDownStrike.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack ends when landing
        airDownStrike.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        //Create the Dead state
        DeadState dead = new DeadState();
        //there are no transitions out of the dead state


        //Add state to the state list
        AddFSMState(idling);
        AddFSMState(moving);
        AddFSMState(dashing);
        AddFSMState(jumping);
        AddFSMState(wallSliding);
        AddFSMState(midair);
        AddFSMState(wallJumping);
        AddFSMState(knockback);

        //attack state list
        AddFSMState(ga1);
        AddFSMState(ga2);
        AddFSMState(ga3);
        AddFSMState(airDownStrike);


        AddFSMState(dead);

    }

    //Unique functions to the player
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

    public void CheckDashInput()
    {
        //only check for these inputs if the dash has not ended

        //left trigger check
        if (Input.GetAxisRaw("DashLeft") != 0)
        {
            if (!leftTriggerDown)
            {
                leftTriggerDown = true;
            }
        }

        if (Input.GetAxisRaw("DashLeft") == 0)
        {
            leftTriggerDown = false;
            if (!rightTriggerDown)
            {
                dashInputAllowed = true;
            }

        }

        //right trigger check
        if (Input.GetAxisRaw("DashRight") != 0)
        {
            if (!rightTriggerDown)
            {
                rightTriggerDown = true;
            }
        }
        if (Input.GetAxisRaw("DashRight") == 0)
        {
            rightTriggerDown = false;
            //only if the left trigger is not down
            if (!leftTriggerDown)
            {
                dashInputAllowed = true;
            }

        }
    }

    public void KnockbackTransition(float dmg, float kbPower, Vector2 ePos)
    {
        SetDamage(dmg);
        SetKnockbackPower(kbPower);
        SetEnemyPos(ePos);
        kbTransition = true;
    }

    //deal damage to the player
    public void Damage()
    {
            health -= damage;   
    }

    //function to handle any increase or decrease of the soul meter.  when using meter, set value to a negative
    public void SoulCalculator(float soulChange)
    {
        soul += soulChange;

        if(soul >= 300)
        {
            soul = 300;
        }
        if(soul <= 0)
        {
            soul = 0;
        }
    }

    public void UpdateState(string state)
    {
        // On screen display of whatever state that the character is in.
        stateText.text = state;
    }

    public void KnockBack()
    {
        //reinitialize velocity
        rig.velocity = Vector2.zero;

            Vector2 currentPos = this.gameObject.transform.position; //player position

            rig.gravityScale = gravityScale;

            Vector2 kbDirection = (currentPos - enemyPos).normalized;
            kbDirection = new Vector2(Mathf.Abs(kbDirection.x) / kbDirection.x, 1); //hard set y value to 1 to ensure enemy bounces up on knockback every time


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

            rig.velocity = Vector2.Scale(kbDirection, new Vector2(knockbackPower, 10));


    }
}
