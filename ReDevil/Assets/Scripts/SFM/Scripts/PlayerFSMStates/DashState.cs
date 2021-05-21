using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : FSMState
{
    //to ensure the dash only occurs once
    private bool isGrounded;
    private bool onWall;
    private bool dashStarted;
    private float gravScale; //the players gravity scale
    float dashDistance;
    private bool dashEnded; //to let the reason occur that the dash ended
    bool floorOrWall;
    //Constructor
    public DashState()
    {
        stateID = FSMStateID.Dashing;
        dashStarted = false;
        dashEnded = false;
    }

    //Act: What are we doing in this state?
    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("Player State: Dashing");
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        //pc.horizontal = Input.GetAxis("Horizontal");
        //pc.vertical = Input.GetAxis("Vertical");
        bool enterKnockback = pc.GetKbTransition();

        pc.UpdateState("Dashing");

        //to check if we touch a wall to transition to idle and end the dash
        pc.TouchingFloorOrWall();
        onWall = pc.GetisTouchingWall();
        isGrounded = pc.GetisGrounded();

        Debug.Log("touching floor or wall? -> " + onWall);
        //We only want to track this at the very start of the dash so it is not recalculated
        if(!dashStarted)
        {
            pc.SetCanDash(false);
            pc.SetDashInputAllowed(false);

            dashStarted = true;
            dashEnded = false;
            //set the value of gravscale so we can reset the correct value
            gravScale = pc.GetRigidbody2D().gravityScale;

            //set the players gravity scale to 0 to allow the dash to occur properly
            pc.GetRigidbody2D().gravityScale = 0f;

            //ONLY ONCE, set the starting dash position based on the players current transformation. This should only occur the first time
            pc.SetDashStartPos(pc.transform.position);

            //determine direction of the dash.  Only change direction and facing left variables
            if (pc.moveVector.x > 0f)
            {
                pc.direction = 1;
                pc.facingLeft = false;


                pc.FlipPlayer();
            }
            else if (pc.moveVector.x < 0f)
            {
                pc.direction = -1;
                pc.facingLeft = true;

                pc.FlipPlayer();

            }

            // TO ADD: break out of state if we're stuck too long in one position

            else //no input detected.  stop speed and set bool to not moving to transition to idle
            {
                //dont change direction
            }
        }


        //get the dash start pos as set within the if loop
        Vector2 dashSP = pc.GetDashStartPos();

        // dash cancel conditions
        if (isGrounded)
        {
            //calculate the dash total distance
            dashDistance = Mathf.Abs(dashSP.x - pc.transform.position.x);

            //set the velocity to allow the dash to occur
            pc.GetRigidbody2D().velocity = Vector2.right * pc.direction * pc.dashSpeed;
        }
        else
        {
            Vector2 playerPos = new Vector2(pc.transform.position.x, pc.transform.position.y);
            Vector2 dashDiff = dashSP - playerPos;

            //dashDistance = Mathf.Abs(dashSP.x - pc.transform.position.x);
            // instead of checking the x distance, we're instead checking the whole magnitude of the vector
            dashDistance = UsefullFunctions.Vec2Magnitude(dashDiff);
            Debug.Log("Dash Dist: " + dashDistance);
            // velocity must also change to account for the dash position that we set
            // create a boolean to lock any change to the dash vector while we dash
            pc.GetRigidbody2D().velocity = pc.GetDashPath() * pc.dashSpeed;
        }

        //get a variable to determine if we are hitting a wall
        onWall = pc.GetisTouchingWall();

        if(!dashEnded)
        {
            //dashed max distance, end the dash.
            
            if (isGrounded && (dashDistance >= pc.dashLength))
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = gravScale;
                dashStarted = false;
                dashEnded = true;
            } 
            else if (!isGrounded && (dashDistance >= pc.dashLength * pc.dashLength))
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = gravScale;
                dashStarted = false;
                dashEnded = true;
            }

            if (enterKnockback)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = gravScale;
                dashStarted = false;
                dashEnded = true;
            }

            //hit a wall.  end the dash
            if (onWall)
            {
                pc.SetCanDash(true);
                pc.GetRigidbody2D().gravityScale = gravScale;
                dashStarted = false;
                dashEnded = true;
            }
        }
        
    }

    //Reason: Put any possible transitions here
    public override void Reason(Transform player, Transform npc)
    {
        Rigidbody2D rig = player.GetComponent<Rigidbody2D>();
        PlayerFSMController pc = player.GetComponent<PlayerFSMController>();
        //pc.horizontal = Input.GetAxis("Horizontal");
        //pc.vertical = Input.GetAxis("Vertical");

        bool grounded = pc.GetisGrounded();
        bool invincible = pc.GetInvincible();
        bool kbTransition = pc.GetKbTransition();

        

        //only transition if the dash has ended
        if (dashEnded)
        {
            //knockback transition
            if (!invincible && kbTransition)
            {
                pc.PerformTransition(Transition.Knockback);
            }
            //if we hit a wall
            if (onWall)
            {
                pc.PerformTransition(Transition.WallSlide);
                //check if were on the ground or not while hitting the wall to end the dash
            }

            if (grounded && onWall)
            {
                pc.PerformTransition(Transition.Idle);
            }
            //idle transition
            else if (grounded && (dashDistance >= pc.dashLength))
            {
                pc.PerformTransition(Transition.Idle);
            } 
            // specific case if we air dash down and hit the ground... not perfect
            else if (grounded && (dashDistance < pc.dashLength * pc.dashLength) && (pc.GetDashPath().y < 0.0f))
            {
                pc.PerformTransition(Transition.Idle);
                
            }

            //airborne transition when walking off an edge
            else if (!grounded)
            {
                pc.PerformTransition(Transition.Airborne);
            }
        }
        

        //dead transition
        if (pc.GetHealth() <= 0)
        {
            pc.PerformTransition(Transition.NoHealth);
        }

    }
}
