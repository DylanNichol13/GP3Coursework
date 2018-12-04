using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {

    //Is the game being played - true or false
    public static bool instance;
    //Player rigid body
    private Rigidbody rb;
    private void Awake() {
        //On start, game is not being played
        instance = false;
        //Get the rigid body of the player to stop or start
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
    }


    Vector3 velocity;
    public void SetInstance()
    {
        instance = true;
        rb.velocity = velocity;
        rb.constraints = RigidbodyConstraints.None;
    }

    public void DisableInstance()
    {
        instance = false;
        velocity = rb.velocity;
        rb.velocity = new Vector3(0, 0, 0);
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
}
