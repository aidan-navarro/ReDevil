using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the manager for all sounds used by the player.  
//In order to use one of the sounds, call the sound manager object in your player object and use one of the funcitons from this class.
//Your code should look something like this: playerVariable.soundManager.PlaySound();

public class PlayerSoundManager : MonoBehaviour
{
    //used to stop all sounds when moving to next scene
    FMOD.Studio.Bus masterBus;

    //Player Movement Sounds
    FMOD.Studio.EventInstance running;
    bool isRunning; //bool to determine when to play next footstep in running event
    public float footstepTimer;

    FMOD.Studio.EventInstance jumping;
    FMOD.Studio.EventInstance landing;

    //Player Attack Sound
    


    // Start is called before the first frame update
    void Start()
    {
        //initialize the fmod bus
        masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");

        //initialize the player movement sounds
        running = FMODUnity.RuntimeManager.CreateInstance("event:/PLAYER/Player_Run");
        jumping = FMODUnity.RuntimeManager.CreateInstance("event:/PLAYER/Player_Jump");
        landing = FMODUnity.RuntimeManager.CreateInstance("event:/PLAYER/Player_Land");

        //initialize variables
        isRunning = false;
    }

    //stop all sound events
    public void StopAllSounds()
    {
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Debug.Log("Stopping all sounds");
    }

    //---------------------------------------
    //Player Movement Sounds
    //---------------------------------------

    //Running Sound Functions
    public void PlayRun()
    {
        if(!isRunning)
        {
            StartCoroutine(Running());
        }
        
    }

    public void StopRun()
    {
        running.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public IEnumerator Running()
    {
        isRunning = true;
        running.start();
        yield return new WaitForSeconds(footstepTimer);
        isRunning = false;
    }

    //Jumping Related Sound Functions
    public void PlayJump()
    {
        jumping.start();
    }
    
    public void PlayLanding()
    {
        landing.start();
    }
}
