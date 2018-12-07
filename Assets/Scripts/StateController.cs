using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {
    //Is the game being played - true or false
    public static bool instance;
    //Player rigid body
    private Rigidbody rb;
    //Called on instantiation
    private void Awake() {
        //On start, game is not being played
        instance = false;
        //Get the rigid body of the player to stop or start
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    //Velocity stored for resuming gameplay
    Vector3 velocity;
    //Resume the game
    public void SetInstance()
    {
        //Instance set to true, to indicate game being played
        instance = true;
        //Return the player rigid body velocity to its 'pre-pause' amount
        rb.velocity = velocity;
        //do not constrain the player rigidbody on any axis
        rb.constraints = RigidbodyConstraints.None;
    }

    //Pause the game
    public void DisableInstance()
    {
        //Signal that the game is paused
        instance = false;
        //Store the velocity for resuming
        velocity = rb.velocity;
        //Set velocity to 0 as game is paused
        rb.velocity = new Vector3(0, 0, 0);
        //Freeze movement as game is paused
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
}
