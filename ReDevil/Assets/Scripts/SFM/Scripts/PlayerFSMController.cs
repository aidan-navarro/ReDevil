using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

//FSM Class for the Player which also contains variables + unique functions for the player
public class PlayerFSMController : AdvancedFSM

{
    //*******************************************************************
    //Variables for the player
    //*******************************************************************

    //-------------------------------------------------------------------
    //Floor and Wall Collision Variables
    //-------------------------------------------------------------------
    private CapsuleCollider2D col; //the players box collider
    public LayerMask groundLayer;
    public LayerMask invisWallLayer;
    public LayerMask nurikabeLayer; // specific Nurikabe functionality *** not currently being used rn

    //used for slope functionality
    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D friction;
    private Vector2 colliderSize;
    [SerializeField]
    float slopeCheckDistance;

    private float slopeDownAngle;
    public float GetSlopeDownAngle() { return slopeDownAngle; }
    
    private float slopeDownAngleOld;
    public Vector2 slopeNormalPerp;
    public bool isOnSlope;

    private float slopeSideAngle;

    //-------------------------------------------------------------------
    //Player HUD Variables
    //-------------------------------------------------------------------
    [SerializeField] private Text stateText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text SoulText;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthBackground;
    [SerializeField] private GameObject SoulLv1Bar;
    [SerializeField] private GameObject SoulLv2Bar;
    [SerializeField] private GameObject SoulLv3Bar;
    [SerializeField] private GameObject SoulBackground;
    public GameObject GetSoulBackground() { return SoulBackground; }
    public void ChangeSoulBackgroundColor()
    {
        Debug.Log("TriggerNoSoul");
        StartCoroutine("SoulShiftColor");
    }

    private IEnumerator SoulShiftColor()
    {
        Color tempColor = SoulBackground.GetComponent<Image>().color;
        SoulBackground.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(0.5f);
        SoulBackground.GetComponent<Image>().color = Color.black;

    }

    [SerializeField] private GameObject DashIcon1;
    [SerializeField] private GameObject DashIcon2;
    [SerializeField] private GameObject PauseMenu;

    //-------------------------------------------------------------------
    //Meter variables
    //-------------------------------------------------------------------
    [SerializeField]
    private float health;
    [SerializeField]
    private float MaxHealth;
    //get and set functions for health
    public float GetHealth() { return health; }
    public void SetHealth(float inHealth) { health = inHealth; UpdateHealthHud(); }

    public void UpdateHealthHud()
    {
        healthText.text = health.ToString();
        healthBar.transform.localScale = new Vector3(health / MaxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
    


    //soul is a meter that builds when hitting enemies.  allows use of soul armaments and soul shot
    [SerializeField]
    private float soul;

    [SerializeField]
    private float soulLevel1Limit;
    [SerializeField]
    private float soulLevel2Limit;
    [SerializeField]
    private float soulLevel3Limit;

    //get and set functions for soul
    public float GetSoul() { return soul; }
    public void SetSoul(float insoul) { soul = insoul; UpdateSoulHud(); }

    public void UpdateSoulHud()
    {
        SoulText.text = ((int)soul).ToString();

        SoulLv1Bar.transform.localScale = new Vector3(soul >= soulLevel1Limit ? 1 : soul / soulLevel1Limit, SoulLv1Bar.transform.localScale.y, SoulLv1Bar.transform.localScale.z);
        if (soul > soulLevel1Limit) SoulLv2Bar.transform.localScale = new Vector3(soul >= soulLevel2Limit ? 1 : (soul - soulLevel1Limit) / (soulLevel2Limit - soulLevel1Limit), SoulLv2Bar.transform.localScale.y, SoulLv2Bar.transform.localScale.z);
        else { SoulLv2Bar.transform.localScale = new Vector3(0, SoulLv2Bar.transform.localScale.y, SoulLv2Bar.transform.localScale.z); }
        if (soul > soulLevel2Limit) SoulLv3Bar.transform.localScale = new Vector3(soul >= soulLevel3Limit ? 1 : (soul - soulLevel2Limit) / (soulLevel3Limit - soulLevel2Limit), SoulLv3Bar.transform.localScale.y, SoulLv3Bar.transform.localScale.z);
        else { SoulLv3Bar.transform.localScale = new Vector3(0, SoulLv3Bar.transform.localScale.y, SoulLv3Bar.transform.localScale.z); }
    }

    [SerializeField]
    private List<SoulArmament> soulArmaments;

    [SerializeField]
    private SoulArmament selectedArament;
    public SoulArmament CurrentArament => selectedArament;

    // multiplier for testing
    [SerializeField, Header("Soul Multiplier Settings")]
    private float soulMultiplier;

    public float GetSoulMultiplier() { return soulMultiplier; }
    public void SetSoulMultiplier(float inSoulMultiplier) { soulMultiplier = inSoulMultiplier; }


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

    [SerializeField]private bool kbTransition; //when this bool value is true, transition to KB State.  Reset to false in iFrames so that we can be knocked back again.
    public bool GetKbTransition() { return kbTransition; }
    public void SetKbTransition(bool inKbTransition) { kbTransition = inKbTransition; }

    // TEST Flame Knockback specific

    [SerializeField] private bool flameKnockback;
    public bool GetFlameKB() { return flameKnockback; }
    public void SetFlameKB(bool inFlameKnockback) { flameKnockback = inFlameKnockback; }
    // ---------- dash knockback specific --------------
    [SerializeField]
    private float dashKnockbackPower;

    [SerializeReference]
    private bool dkbTransition;
    public bool GetDKBTransition() { return dkbTransition;  }
    public void SetDKBTransition(bool inDKBTransition)
    {
        dkbTransition = inDKBTransition;
    }

    private bool immobile; //when this bool value is true, transition to KB State.  Reset to false in iFrames so that we can be knocked back again.
    public bool GetImmobile() { return immobile; }
    public void SetImmobile(bool inImmobile) { immobile = inImmobile; }

    private bool invincible; //to detect if we are in iFrames.  ONLY DO DAMAGE AND KNOCKBACK IF THIS BOOL IS FALSE
    public bool GetInvincible() { return invincible; }
    public void SetInvincible(bool inInvincible) { invincible = inInvincible; }

    //-------------------------------------------------------------------
    //movement variables
    //-------------------------------------------------------------------

    private Rigidbody2D rig;
    public Rigidbody2D GetRigidbody2D() { return rig; }

    private float gravityScale;

    [SerializeField]
    private float moveSpeed = 10;
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

    //determines if the player is allowed to dash
    private bool canDash;
    public bool GetCanDash() { return canDash; }
    public void SetCanDash(bool inCanDash) { canDash = inCanDash; }

    //checks if we are allowing a dash input to be read (pressing the controller button)
    private bool dashInputAllowed;
    public bool GetDashInputAllowed() { return dashInputAllowed; }
    public void SetDashInputAllowed(bool inDashInputAllowed) { dashInputAllowed = inDashInputAllowed; }

    private Vector2 dashStartPos; //for tracking the start position of the dash
    public Vector2 GetDashStartPos() { return dashStartPos; }
    public void SetDashStartPos(Vector2 inDashStartPos) { dashStartPos = inDashStartPos; }

    // ----------------- TEST: Omnidirectional Air Dash trajectory -------------------
    // Take the input from move vector, and use a separate dash vector to 
    [SerializeField] private int airDashCount;
    [SerializeField] private int airDashLimit; // set value in inspector
    public int GetAirDashCount() { return airDashCount; }
    public void SetAirDashCount(int dash) { airDashCount = dash; }
    public void IncrementAirDashCount() { airDashCount++; } 
    public void DecrementAirDashCount() { airDashCount--; } // decrementing air dash should only happen when you kill an enemy
    public void ResetAirDashCount() { airDashCount = 0; } // air dash only resets upon touching ground or wall


    // ----------------- END TEST REGION -----------------------

    // Dash Attack Path functions
    [SerializeField]
    protected Vector2 dashPath;
    public Vector2 GetDashPath() { return dashPath; }
    public void SetDashPath(Vector2 inDashPath) { dashPath = inDashPath; } 
 
    //respawn
    public RespawnManager respawnPoint;

    

    //-------------------------------------------------------------------
    //variables to detect controller input
    //-------------------------------------------------------------------
    [System.NonSerialized]
    public Vector2 moveVector;

    [System.NonSerialized]
    public bool leftTriggerDown;
    [System.NonSerialized]
    public bool rightTriggerDown;

    [System.NonSerialized]
    private bool attackButtonDown;
    public bool GetAttackButtonDown() { return attackButtonDown; }
    public void SetAttackButtonDown(bool inAttackButtonDown) { attackButtonDown = inAttackButtonDown; }

    [SerializeField]
    private bool jumpButtonDown;
    public bool GetJumpButtonDown() { return jumpButtonDown; }

    // GROUND + WALL CHECK SPECIFICS
    [SerializeField]
    private bool groundJump;
    public bool GetGroundJump() { return groundJump; }
    public void SetGroundJump(bool inGroundJump) { groundJump = inGroundJump; }

    [System.NonSerialized]
    private bool soulAttackButtonDown;
    public bool GetSoulAttackButtonDown() { return soulAttackButtonDown; }

    //variables for determining direction faced
    [System.NonSerialized]
    public bool facingLeft = false; // This bool is to help position attack hitbox
    private bool playerFlipped = false; //this bool is to flip the sprite in FlipPlayer() Function.

    [System.NonSerialized]
    public int direction = 1; //For calculating direction for other movement options.  Must be set whenever player moves in different direction


    //jump variables
    [SerializeField]
    private bool isGrounded; //variable to determine if the player is grounded.  the player can only jump if on the ground.
    public bool GetisGrounded() { return isGrounded; }
    public void SetisGrounded(bool inIsGrounded) { isGrounded = inIsGrounded; }

    private bool isTouchingWall;
    public bool GetisTouchingWall() { return isTouchingWall; }
    public void SetisTouchingWall(bool inIsTouchingWall) { isTouchingWall = inIsTouchingWall; }

    private bool isTouchingCeiling;
    public bool GetisTouchingCeiling() { return isTouchingCeiling; }
    public void SetisTouchingCeiling(bool inIsTouchingCeiling) { isTouchingCeiling = inIsTouchingCeiling; }

    [SerializeField]
    private bool isTouchingInvisibleWall;
    public bool GetisTouchingInvisibleWall() { return isTouchingInvisibleWall; }
    public void SetisTouchingInvisibleWall(bool inIsTouchingInvisibleWall) { isTouchingInvisibleWall = inIsTouchingInvisibleWall; }

    [SerializeField]
    private bool isTouchingNurikabe;
    public bool GetisTouchingNurikabe() { return isTouchingNurikabe; }
    public void SetisTouchingNurikabe(bool inIsTouchingNurikabe) { isTouchingNurikabe = inIsTouchingNurikabe; } 

    [SerializeField]
    private float jumpPower = 10;
    public float GetJumpPower() { return jumpPower; }
    public void SetJumpPower(float newJumpPower) { jumpPower = newJumpPower; }

    // Player Input
    public PlayerInput playerInput { get; private set; }
    private GameplayControls gameplayControls;

    //Player Sound
    public PlayerSoundManager soundManager;

    // Pause boolean
    private bool isPaused;
    public bool GetIsPaused() { return isPaused; }
    public void SetIsPaused(bool inIsPaused) { isPaused = inIsPaused; }



    //initialize FSM
    protected override void Initialize()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        rig = GetComponent<Rigidbody2D>();

        // game isn't paused at the start 
        isPaused = false;
        PauseMenu.SetActive(false);
        SetIsPaused(false);
        UnPause();

        //set value for gravity based on rigs gravity scaling
        gravityScale = rig.gravityScale;

        SetHealth(MaxHealth);
        dashKnockbackPower = 1;

        leftTriggerDown = false;
        rightTriggerDown = false;

        // counting the amount of airdashes
        airDashCount = 0;
        airDashLimit = 2; // hard code

        canDash = true;
        dashInputAllowed = true;
        invincible = false;


        //capsule collider
        col = GetComponent<CapsuleCollider2D>();
        colliderSize = col.size;

        //Player Input Setup
        gameplayControls = new GameplayControls();
        playerInput = GetComponent<PlayerInput>();

        playerInput.onActionTriggered += OnActionTriggered;

        UpdateHealthHud();
        UpdateSoulHud();

        ConstructFSM();
    }

    private void Awake()
    {
        respawnPoint = FindObjectOfType<RespawnManager>();
        //used to prevent if the respawnpoint isnt initialized.  if the player reads first dont set a respawn point
        if(respawnPoint.initialized)
        {
            transform.position = respawnPoint.respawnPoint;
        }
        
    }

    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (!isPaused)
        {
            if (obj.action.name == gameplayControls.Gameplay.Jump.name)
            {
                //OnJump(obj);
                if (obj.canceled)
                {
                    jumpButtonDown = false;
                    groundJump = false;
                }

                else if (obj.started)
                {
                    jumpButtonDown = true;
                    if (isGrounded)
                    {
                        groundJump = true;
                    } 
                }

            }

            if (obj.action.name == gameplayControls.Gameplay.Movement.name)
            {
                OnMove(obj);
            }

            if (obj.action.name == gameplayControls.Gameplay.Attack.name)
            {
                OnAttack(obj);
            }

            if (obj.action.name == gameplayControls.Gameplay.DashLeft.name)
            {
                OnDashLeft(obj);
            }

            if (obj.action.name == gameplayControls.Gameplay.DashRight.name)
            {
                OnDashRight(obj);
            }

            if (obj.action.name == gameplayControls.Gameplay.ToggleSoulArmament.name)
            {
                if (obj.started)
                {
                    if (soul - selectedArament.SoulCost <= 0.0f)
                    {
                        ChangeSoulBackgroundColor();
                    }
                    else
                    {
                        OnToggleSoulArament(obj);
                    }
                }
            }

            if (obj.action.name == gameplayControls.Gameplay.SoulPowerShot.name)
            {
                OnSoulShot(obj);
            }
        }
        if (obj.action.name == gameplayControls.Gameplay.Pause.name)
        {
            //bool test = Gamepad.current.aButton.wasPressedThisFrame;
            //if (!test)
            //{
            //    Debug.Log("Listen for Input " + test);
            //}
            if (obj.started)
            {
                isPaused = !isPaused;
                // call the function to activate the start menu
                if (isPaused)
                {
                    Pause();
                } else
                {
                    UnPause();
                }
            }
            //else if (obj.canceled)
            //{
            //    Debug.Log("End Input");
            //}
        }
    }

    private void OnSoulShot(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            soulAttackButtonDown = true;
        }
        else if (obj.canceled)
        {
            soulAttackButtonDown = false;  
        }
    }

    private void OnToggleSoulArament(InputAction.CallbackContext obj)
    {
        if (CurrentStateID != FSMStateID.KnockedBack || CurrentStateID != FSMStateID.Dead)
        {
            if (selectedArament.IsActive)
            {
                selectedArament.DeActivateArament();
            }
            else
            {
                selectedArament.ActivateArament();
            }
        }
    }

    private void OnDashRight(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            rightTriggerDown = true;
        }
        else if (obj.canceled)
        {
            rightTriggerDown = false;
            if (!leftTriggerDown)
            {
                dashInputAllowed = true;
            }
        }
    }

    private void OnDashLeft(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            leftTriggerDown = true;
        }
        else if (obj.canceled)
        {
            leftTriggerDown = false;
            if (!rightTriggerDown)
            {
                dashInputAllowed = true;
            }
        }
    }

    private void OnAttack(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            attackButtonDown = false;
        }

        else
        {
            attackButtonDown = true;
        }
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        moveVector = obj.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            jumpButtonDown = false;
        }

        else
        {
            jumpButtonDown = true;
        }

    }

    protected override void FSMUpdate()
    {
        if (selectedArament.IsActive)
        {
            SoulCalculator(-selectedArament.SoulCost * Time.deltaTime);
            if (soul <= 0)
            {
                selectedArament.DeActivateArament();
            }
        }

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
        idling.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if i die while idle, transition to dead
        idling.AddTransition(Transition.Move, FSMStateID.Moving);  // if I start moving on ground while idle, transition to moving
        idling.AddTransition(Transition.Jump, FSMStateID.Jumping); // if i jump while idle, transition to Jump State
        idling.AddTransition(Transition.Dash, FSMStateID.Dashing); // if i press the dash button, transition to dash state
        idling.AddTransition(Transition.DashAttack, FSMStateID.DashAttacking);
        idling.AddTransition(Transition.GroundToAirDashAttack, FSMStateID.GroundToAirDashAttacking);
        idling.AddTransition(Transition.WallJump, FSMStateID.WallJumping);
        idling.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player
        idling.AddTransition(Transition.GroundAttack1, FSMStateID.GroundFirstStrike);
        idling.AddTransition(Transition.SoulShot, FSMStateID.SoulShot);
        idling.AddTransition(Transition.Airborne, FSMStateID.Midair);

        //create the Moving state
        MoveState moving = new MoveState();

        //create transitions for the follow state
        moving.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if i die while moving, transition to dead
        moving.AddTransition(Transition.Idle, FSMStateID.Idling);  //If i stop moving, transition to idling
        moving.AddTransition(Transition.Jump, FSMStateID.Jumping); // if i jump while idle, transition to jumping
        moving.AddTransition(Transition.Airborne, FSMStateID.Midair); //if i walk off an edge without jumping, transition to midair movement
        //moving.AddTransition(Transition.Dash, FSMStateID.Dashing);
        moving.AddTransition(Transition.GroundAttack1, FSMStateID.GroundFirstStrike);
        moving.AddTransition(Transition.DashAttack, FSMStateID.DashAttacking); // If I'm moving currently, go into a dash attack
        moving.AddTransition(Transition.GroundToAirDashAttack, FSMStateID.GroundToAirDashAttacking);
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

        AirDashNormal airDashing = new AirDashNormal();
        //create transitions for the dash state
        airDashing.AddTransition(Transition.NoHealth, FSMStateID.Dead); //if i die while dashing, transition to dead
        airDashing.AddTransition(Transition.Idle, FSMStateID.Idling); //if dash ends on the ground OR they hit a wall while on ground, idle
        airDashing.AddTransition(Transition.WallSlide, FSMStateID.WallSliding); //if dash ends in midair and hit a wall, wall sliding
        airDashing.AddTransition(Transition.Airborne, FSMStateID.Midair); //if the dash ends midair, airborne
        airDashing.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

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
        midair.AddTransition(Transition.AirAttack, FSMStateID.AirStrike);
        //midair.AddTransition(Transition.Dash, FSMStateID.Dashing);
        midair.AddTransition(Transition.AirDashAttack, FSMStateID.AirDashAttacking);
        midair.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player
        midair.AddTransition(Transition.AirDownStrike, FSMStateID.AirDownStrike);//air down strike
        midair.AddTransition(Transition.SoulShot, FSMStateID.SoulShot);

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

        // Addition: State for a ground dash attack
        GroundDashAttack groundDashAttack = new GroundDashAttack();

        groundDashAttack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        groundDashAttack.AddTransition(Transition.Idle, FSMStateID.Idling);
        groundDashAttack.AddTransition(Transition.Move, FSMStateID.Moving);
        groundDashAttack.AddTransition(Transition.Airborne, FSMStateID.Midair);
        groundDashAttack.AddTransition(Transition.WallSlide, FSMStateID.WallSliding);
        groundDashAttack.AddTransition(Transition.DashKnockback, FSMStateID.DashKnockingBack); // if contact with dash attack happens, transition here
        groundDashAttack.AddTransition(Transition.Knockback, FSMStateID.KnockedBack);

        // Addition: State for the air dash.
        AirDashAttack airDashAttack = new AirDashAttack();

        airDashAttack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        airDashAttack.AddTransition(Transition.Idle, FSMStateID.Idling); // we hit the ground
        airDashAttack.AddTransition(Transition.WallSlide, FSMStateID.WallSliding);
        airDashAttack.AddTransition(Transition.Airborne, FSMStateID.Midair); // player starts falling out of dash
        airDashAttack.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); // player gets hit out of it
        airDashAttack.AddTransition(Transition.DashKnockback, FSMStateID.DashKnockingBack); // test for now

        GroundDashKnockback groundDashKnockback = new GroundDashKnockback();

        groundDashKnockback.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if we happen to die during this knockback 
        groundDashKnockback.AddTransition(Transition.Idle, FSMStateID.Idling);
        groundDashKnockback.AddTransition(Transition.Move, FSMStateID.Moving);
        groundDashKnockback.AddTransition(Transition.Dash, FSMStateID.Dashing);
        groundDashKnockback.AddTransition(Transition.AirDash, FSMStateID.AirDashing); // test, airdashcancel
        groundDashKnockback.AddTransition(Transition.Airborne, FSMStateID.Midair); // transition into airborne from the knockback
        groundDashKnockback.AddTransition(Transition.WallSlide, FSMStateID.WallSliding);

        GroundAttack1State ga1 = new GroundAttack1State();

        ga1.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        ga1.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack just ends
        //ga1.AddTransition(Transition.Dash, FSMStateID.Dashing); // dash cancel
        ga1.AddTransition(Transition.DashAttack, FSMStateID.DashAttacking); // dash cancel
        ga1.AddTransition(Transition.GroundToAirDashAttack, FSMStateID.GroundToAirDashAttacking); // dash cancel
        ga1.AddTransition(Transition.GroundAttack2, FSMStateID.GroundSecondStrike);
        ga1.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        GroundAttack2State ga2 = new GroundAttack2State();

        ga2.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        ga2.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack just ends
        //ga2.AddTransition(Transition.Dash, FSMStateID.Dashing); // dash cancel
        ga2.AddTransition(Transition.DashAttack, FSMStateID.DashAttacking); // dash cancel
        ga2.AddTransition(Transition.GroundToAirDashAttack, FSMStateID.GroundToAirDashAttacking); // dash cancel

        ga2.AddTransition(Transition.GroundAttack3, FSMStateID.GroundThirdStrike);
        ga2.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        GroundAttack3State ga3 = new GroundAttack3State();

        ga3.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        ga3.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack just ends
        //ga3.AddTransition(Transition.Dash, FSMStateID.Dashing); // dash cancel
        ga3.AddTransition(Transition.DashAttack, FSMStateID.DashAttacking); // dash cancel
        ga3.AddTransition(Transition.GroundToAirDashAttack, FSMStateID.GroundToAirDashAttacking); // dash cancel
        ga3.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        GroundToAirDashAttack groundToAirDashAttack = new GroundToAirDashAttack();

        groundToAirDashAttack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        groundToAirDashAttack.AddTransition(Transition.Airborne, FSMStateID.Midair);
        groundToAirDashAttack.AddTransition(Transition.Knockback, FSMStateID.KnockedBack);
        groundToAirDashAttack.AddTransition(Transition.WallSlide, FSMStateID.WallSliding);
        groundToAirDashAttack.AddTransition(Transition.DashKnockback, FSMStateID.DashKnockingBack);

        AirAttackState airAttack = new AirAttackState();

        airAttack.AddTransition(Transition.NoHealth, FSMStateID.Dead); // if we die
        airAttack.AddTransition(Transition.Idle, FSMStateID.Idling); // once Air attack ends do we hit the ground
        airAttack.AddTransition(Transition.Airborne, FSMStateID.Midair); // or do we stay in the air
        airAttack.AddTransition(Transition.WallSlide, FSMStateID.WallSliding); // or if we hit the wall
        airAttack.AddTransition(Transition.AirDashAttack, FSMStateID.AirDashAttacking); // should we wait until it goes out of air attack or can we cancel out of it?
        airAttack.AddTransition(Transition.Knockback, FSMStateID.KnockedBack);

        AirDownStrikeState airDownStrike = new AirDownStrikeState();

        airDownStrike.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        airDownStrike.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack ends when landing
        airDownStrike.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player

        SoulShotState soulShotState = new SoulShotState();

        soulShotState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        soulShotState.AddTransition(Transition.Idle, FSMStateID.Idling); //the attack just ends
        soulShotState.AddTransition(Transition.Airborne, FSMStateID.Midair); //if i complete my jump, transition to midair movement
        soulShotState.AddTransition(Transition.Knockback, FSMStateID.KnockedBack); //if i get hit, knock back the player


        //Create the Dead state
        DeadState dead = new DeadState();
        //there are no transitions out of the dead state

        //Add state to the state list
        AddFSMState(idling);
        AddFSMState(moving);
        AddFSMState(dashing);
        AddFSMState(airDashing);
        AddFSMState(jumping);
        AddFSMState(wallSliding);
        AddFSMState(midair);
        AddFSMState(wallJumping);
        AddFSMState(knockback);

        //attack state list
        AddFSMState(groundDashAttack); // adding to the attack states
        AddFSMState(airDashAttack); // adding to state
        AddFSMState(groundToAirDashAttack);
        AddFSMState(groundDashKnockback); // adding right after dash attack
        AddFSMState(airAttack);
        AddFSMState(ga1);
        AddFSMState(ga2);
        AddFSMState(ga3);
        AddFSMState(airDownStrike);
        AddFSMState(soulShotState);

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

    //Functions to set friction material.  This allows you to stand on a slope 

    public void SetFrictionMaterial()
    {
        rig.sharedMaterial = friction;
    }

    public void SetNoFrictionMaterial()
    {
        rig.sharedMaterial = noFriction;
    }

    #region Functions to handle movement on a slope
    public void SlopeCheck()
    {
        //Debug.Log("Checking For Slope");
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);

        if(slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if(slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        if(hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            //Debug.Log("vector along slope:" + slopeNormalPerp);

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }


            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, hit.normal, Color.green);
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
        }
    }

    public void UpdateSlopeDashVelocity(Vector2 dashVector)
    {
        if (isOnSlope)
        {

        } else
        {
            return;
        }
    }
    #endregion
    public void TouchingFloorCeilingWall()
    {
        //equation values to determine if the player is on the ground
        Vector2 feetPos = col.bounds.center;
        feetPos.y -= col.bounds.extents.y;
        isGrounded = Physics2D.OverlapBox(feetPos, new Vector2(col.size.x - 0.2f, 0.1f), 0f, groundLayer.value);

        Vector2 headPos = col.bounds.center;
        headPos.y += col.bounds.extents.y;
        isTouchingCeiling = Physics2D.OverlapBox(headPos, new Vector2(col.size.x - 0.2f, 0.1f), 0f, groundLayer.value);

        //equation values to determine if the player is on a wall
        Vector2 sidePos = col.bounds.center;
        sidePos.x += col.bounds.extents.x * direction;
        isTouchingWall = Physics2D.OverlapBox(sidePos, new Vector2(0.1f, col.size.y - 0.5f), 0f, groundLayer.value);
        Debug.Log("Touching the wall: " + isTouchingWall);


    }

    public void TouchingInvisibleWall()
    {
        ////make temp bools to check top and side
        //Vector2 feetPos = col.bounds.center;
        //feetPos.y -= col.bounds.extents.y;
        //bool isTouchingTop = Physics2D.OverlapBox(feetPos, new Vector2(col.size.x - 0.2f, 0.1f), 0f, invisWallLayer.value);
        //Debug.Log("Touching the top: " + isTouchingTop);

        Vector2 headPos = col.bounds.center;
        headPos.y += col.bounds.extents.y;
        bool isTouchingTop = Physics2D.OverlapBox(headPos, new Vector2(col.size.x - 0.2f, 0.1f), 0f, invisWallLayer.value);
        //Debug.Log("Touching the top: " + isTouchingCeiling);

        //Check if the side is touching an invisible wall
        Vector2 sidePos = col.bounds.center;
        sidePos.x += col.bounds.extents.x * direction;
        bool isTouchingSide = Physics2D.OverlapBox(sidePos, new Vector2(0.1f, col.size.y - 0.2f), 0f, invisWallLayer.value);

        //if either touching side OR top, set is touching invisible wall to true
        if(isTouchingTop || isTouchingSide)
        {
            isTouchingInvisibleWall = true;
        }
        else
        {
            isTouchingInvisibleWall = false;
        }
    }

    // nurikabe test
    public void TouchingNurikabe()
    {
        //equation values to determine if the player is on the ground
        Vector2 feetPos = col.bounds.center;
        feetPos.y -= col.bounds.extents.y;
        isTouchingNurikabe = Physics2D.OverlapBox(feetPos, new Vector2(col.size.x - 0.2f, 0.1f), 0f, nurikabeLayer.value);

        //equation values to determine if the player is on a wall
        Vector2 sidePos = col.bounds.center;
        sidePos.x += col.bounds.extents.x * direction;
        isTouchingNurikabe = Physics2D.OverlapBox(sidePos, new Vector2(0.1f, col.size.y - 0.2f), 0f, nurikabeLayer.value);
    }

    // check for the air dash limit
    public void CheckAirDash()
    {
        if (airDashCount < airDashLimit)
        {
            canDash = true;
        } else
        {
            canDash = false;
        }
    }

    // this alters the visibility of the dash icons in the UI
    public void UpdateDashIcons()
    {
        if (!isPaused)
        {
            switch (airDashCount)
            {
                case 0:
                    DashIcon1.SetActive(true);
                    DashIcon2.SetActive(true);
                    break;
                case 1:
                    DashIcon1.SetActive(true);
                    DashIcon2.SetActive(false);
                    break;
                case 2:
                    DashIcon1.SetActive(false);
                    DashIcon2.SetActive(false);
                    break;
                default:
                    DashIcon1.SetActive(true);
                    DashIcon2.SetActive(true);
                    break;
            }

        } else
        {
            DashIcon1.SetActive(false);
            DashIcon2.SetActive(false);
        }
    }

    //public void CheckDashInput()
    //{
    //    //only check for these inputs if the dash has not ended
    //    //left trigger check
    //    if (Input.GetAxisRaw("DashLeft") != 0)
    //    {
    //        if (!leftTriggerDown)
    //        {
    //            leftTriggerDown = true;
    //        }
    //    }
    //    if (Input.GetAxisRaw("DashLeft") == 0)
    //    {
    //        leftTriggerDown = false;
    //        if (!rightTriggerDown)
    //        {
    //            dashInputAllowed = true;
    //        }
    //    }
    //    //right trigger check
    //    if (Input.GetAxisRaw("DashRight") != 0)
    //    {
    //        if (!rightTriggerDown)
    //        {
    //            rightTriggerDown = true;
    //        }
    //    }
    //    if (Input.GetAxisRaw("DashRight") == 0)
    //    {
    //        rightTriggerDown = false;
    //        //only if the left trigger is not down
    //        if (!leftTriggerDown)
    //        {
    //            dashInputAllowed = true;
    //        }

    //    }
    //}

    // fine for single hit functions, but for lasting projectiles like a flamethrower
    // this will bust some logic
    public void KnockbackTransition(float dmg, float kbPower, Vector2 ePos)
    {
        if (selectedArament.IsActive)
        {
            SoulCalculator(-dmg);
            if (soul <= 0)
            {
                selectedArament.DeActivateArament();
            }
            return;
        }
        else
        {
            SetDamage(dmg);
            SetKnockbackPower(kbPower);
            SetEnemyPos(ePos);
            kbTransition = true;
        }
    }

    //deal damage to the player
   public void TakeDamage()
   {
       health -= damage;
       UpdateHealthHud();
    }

    //function to handle any increase or decrease of the soul meter.  when using meter, set value to a negative
    public void SoulCalculator(float soulChange)
    {
        soul += soulChange;

        if (soul >= soulLevel3Limit)
        {
            soul = soulLevel3Limit;
        }
        if (soul <= 0)
        {
            soul = 0;
        }

        UpdateSoulHud();
    }


    public void UpdateState(string state)
    {
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

    // Custom Knockback code when we hit the enemy
    public void DashKnockback()
    {
        //reinitialize velocity
        //rig.velocity = Vector2.zero;
        Vector2 currentPos = this.gameObject.transform.position; //player position
        Vector2 kbDirection = new Vector2(0, 0);

        if (facingLeft)
        {
            kbDirection = new Vector2(1, 0);
            //DashKnockbackTransition(20, currentPos - kbDirection);
        }
        else if (!facingLeft)
        {
            kbDirection = new Vector2(-1, 0);
            //DashKnockbackTransition(20, currentPos - kbDirection);
        }

        rig.velocity = Vector2.zero;
        rig.gravityScale = gravityScale;

        // Dash Knockback Vector
        if (isGrounded)
        {
            rig.velocity = Vector2.Scale(kbDirection, new Vector2(dashKnockbackPower* 2, 10));
        }
        else
        {
            kbDirection.y = 1;
            rig.velocity = Vector2.Scale(kbDirection, new Vector2(dashKnockbackPower, 12));
        }
        Debug.Log("Result Velocity: " + rig.velocity);

    }

    // code for airdash attack... not in use yet
    public void AirDashKnockback()
    {
        //reinitialize velocity
        rig.velocity = Vector2.zero;

        Vector2 currentPos = this.gameObject.transform.position; //player position

        rig.gravityScale = gravityScale;

        // have the character simply pop upward
        rig.velocity = new Vector2(0.0f, 10.0f);

    }

    // on the second hit of air dash
    public void SideDashKnockback(Vector2 atkVector)
    {
        rig.velocity = Vector2.zero;
        rig.gravityScale = gravityScale;
        Vector2 bounceVector = atkVector;
        bounceVector.x *= -1;
        bounceVector.y = Mathf.Abs(bounceVector.y) * -1;
        rig.AddForce(bounceVector * dashSpeed / 2, ForceMode2D.Impulse);
    }

    // knocking straight back down.  Leave here just in case
    public void AirDashBottomKnockback()
    {
        Debug.Log("BottomKnockback");
        //reinitialize velocity
        rig.velocity = Vector2.zero;
        rig.gravityScale = gravityScale;

        // have the character simply pop downward uppon hitting the bottom
        //rig.velocity = new Vector2(0.0f, -10.0f);
        rig.AddForce(Vector2.down, ForceMode2D.Impulse);
    }

    public void AirDashBottomKnockback2(Vector2 dashVector)
    {
        rig.velocity = Vector2.zero;
        rig.gravityScale = gravityScale;
        Vector2 bounceVector = Vector2.Reflect(dashVector, Vector2.down);
        Debug.Log("Bounce Vector: " + bounceVector);
        rig.AddForce(bounceVector * dashSpeed / 2, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "CheckPoint")
        {
            respawnPoint.respawnPoint = other.transform.position;
        }
    }

    public void PlayPlayerDead()
    {
        StartCoroutine("PlayerDead");
    }

    public IEnumerator PlayerDead()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("DeathScene");
    }

    public void AddSoul(int soulAdd)
    {
        soul += soulAdd;
        Debug.Log("soul: " + soul);
    }
    
    
    public void HealPlayer(int healAmount)
    {
        health += healAmount;
        Debug.Log("health: " + health);
    
        if (health > MaxHealth)
        {
            health = MaxHealth;
        }    
    }
    
    //Coroutine Created due to a bug where character would be set before respawnpoint could be set.  Using the start function did not work
    private IEnumerator StartDelay()
    {
        respawnPoint = FindObjectOfType<RespawnManager>();
        transform.position = respawnPoint.respawnPoint;
        Debug.Log("Respawn Location; " + respawnPoint.respawnPoint + "// ID; " + respawnPoint.rand);
        yield return new WaitForEndOfFrame();
    }


    // --------------- PAUSING GAME FUNCTIONALITY -----------------
    #region Click below to access pause functionality code
    public void Pause()
    {
        // set to false
        stateText.enabled = false;
        healthText.enabled = false;
        SoulText.enabled = false;

        healthBar.SetActive(false);
        healthBackground.SetActive(false);
        SoulLv1Bar.SetActive(false);
        SoulLv2Bar.SetActive(false);
        SoulLv3Bar.SetActive(false);
        SoulBackground.SetActive(false);
        DashIcon1.SetActive(false);
        DashIcon2.SetActive(false);

        // set to true
        PauseMenu.SetActive(true);
        Time.timeScale = 0;

        //Set the first button in the pause menu
        PauseMenu pause = PauseMenu.GetComponent<PauseMenu>();

        //start by clearing the latest selection
        EventSystem.current.SetSelectedGameObject(null);
        //set to the first button in the pause menu
        EventSystem.current.SetSelectedGameObject(pause.retry);
    }

    public void UnPause()
    {
        // set to false
        stateText.enabled =  true;
        healthText.enabled = true;
        SoulText.enabled = true;

        healthBar.SetActive(true);
        healthBackground.SetActive(true);

        SoulLv1Bar.SetActive(true);
        SoulLv2Bar.SetActive(true);
        SoulLv3Bar.SetActive(true);
        SoulBackground.SetActive(true);

        DashIcon1.SetActive(true);
        DashIcon2.SetActive(true);
        // set to false
        PauseMenu.SetActive(false);
        moveVector = Vector2.zero;
        Time.timeScale = 1;

    }
    private void OnApplicationPause(bool pause)
    {
        
    }

    #endregion

}
