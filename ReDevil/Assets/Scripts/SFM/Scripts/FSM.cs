using UnityEngine;
using System.Collections;

// <dev notes> 
// Everything that inherits from this class will have its own update function
// the updates and fixed update functions will have their own overrides for different classes that are inheriting this FSM
public class FSM : MonoBehaviour 
{
    //Player Transform
    protected Transform playerTransform;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }

	// Use this for initialization
	void Start () 
    {
        Initialize();
	}
	
	// Update is called once per frame
	void Update () 
    {
        FSMUpdate();
	}

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }    
}
