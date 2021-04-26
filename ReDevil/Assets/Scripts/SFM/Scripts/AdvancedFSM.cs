using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is adapted and modified from the FSM implementation class available on UnifyCommunity website
/// The license for the code is Creative Commons Attribution Share Alike.
/// It's originally the port of C++ FSM implementation mentioned in Chapter01 of Game Programming Gems 1
/// You're free to use, modify and distribute the code in any projects including commercial ones.
/// Please read the link to know more about CCA license @http://creativecommons.org/licenses/by-sa/3.0/
/// </summary>

public enum Transition
{
    None = 0,
    //states for the player movement
    NoHealth,
    Idle,
    Move,
    Jump,
    Airborne,
    Dash,
    WallSlide,
    WallJump,
    Knockback,

    //states for player attacks
    GroundAttack1,
    GroundAttack2,
    GroundAttack3,
    AirDownStrike,

    //states for the enemy
    EnemyNoHealth,


    //attack states for individual minion ememies
    NodeppouIdle,
    NodeppouAttack,

    ShutenDojiIdle,
    ShutenDojiAttack,

    BasanIdle,
    BasanAttack,

    ChochinObakeIdle,
    ChochinObakeAttack,

    WaniguchiIdle,
    WaniguchiAttack,


}

public enum FSMStateID
{
    None = 0,
    //states for the player
    Dead, //character is dead
    Idling, //the player is stationary 
    Moving,
    Jumping, //the player is jumping
    Midair, //the player is out of jump, or is falling
    Dashing, //the player is dashing
    WallSliding, //the player is sliding on a wall
    WallJumping, //the player is jumping off the wall
    KnockedBack, //the player is being knocked back and taking damage

    //player attacks
    GroundFirstStrike,
    GroundSecondStrike,
    GroundThirdStrike,
    AirDownStrike,

    //for enemy
    EnemyDead,

    //attacks for individual minion enemies

    //Nodeppou
    NodeppouIdling,
    NodeppouAttacking,

    //ShutenDoji
    ShutenDojiIdling,
    ShutenDojiAttacking,

    BasanIdling,
    BasanAttacking,

    //ChochinObake
    ChochinObakeIdling,
    ChochinObakeAttacking,

    //Waniguchi
    WaniguchiIdling,
    WaniguchiAttacking,

}

public class AdvancedFSM : FSM 
{
    private List<FSMState> fsmStates;

    //The fsmStates are not changing directly but updated by using transitions
    private FSMStateID currentStateID;
    public FSMStateID CurrentStateID { get { return currentStateID; } }

    private FSMState currentState;
    public FSMState CurrentState { get { return currentState; } }

    public AdvancedFSM()
    {
        fsmStates = new List<FSMState>();
    }

    /// <summary>
    /// Add New State into the list
    /// </summary>
    public void AddFSMState(FSMState fsmState)
    {
        // Check for Null reference before deleting
        if (fsmState == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }

        // First State inserted is also the Initial state
        //   the state the machine is in when the simulation begins
        if (fsmStates.Count == 0)
        {
            fsmStates.Add(fsmState);
            currentState = fsmState;
            currentStateID = fsmState.ID;
            return;
        }

        // Add the state to the List if it´s not inside it
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == fsmState.ID)
            {
                Debug.LogError("FSM ERROR: Trying to add a state that was already inside the list");
                return;
            }
        }

        //If no state in the current then add the state to the list
        fsmStates.Add(fsmState);
    }

    /// <summary>
    /// This method delete a state from the FSM List if it exists, 
    ///   or prints an ERROR message if the state was not on the List.
    /// </summary>
    public void DeleteState(FSMStateID fsmState)
    {
        // Check for NullState before deleting
        if (fsmState == FSMStateID.None)
        {
            Debug.LogError("FSM ERROR: bull id is not allowed");
            return;
        }

        // Search the List and delete the state if it´s inside it
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == fsmState)
            {
                fsmStates.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: The state passed was not on the list. Impossible to delete it");
    }

    /// <summary>
    /// This method tries to change the state the FSM is in based on
    /// the current state and the transition passed. If current state
    ///  doesn´t have a target state for the transition passed, 
    /// an ERROR message is printed.
    /// </summary>
    public void PerformTransition(Transition trans)
    {
        // Check for NullTransition before changing the current state
        if (trans == Transition.None)
        {
            Debug.LogError("FSM ERROR: Null transition is not allowed");
            return;
        }

        // Check if the currentState has the transition passed as argument
        FSMStateID id = currentState.GetOutputState(trans);
        if (id == FSMStateID.None)
        {
            //Debug.LogError("FSM ERROR: Current State does not have a target state for this transition");
            return;
        }

        // Update the currentStateID and currentState		
        currentStateID = id;
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == currentStateID)
            {
                currentState = state;
                currentState.EnterStateInit();
                break;
            }
        }
    }
}
