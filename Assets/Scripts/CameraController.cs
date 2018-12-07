using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //The player object
    private GameObject playerObj;
    //Game is still active
    private bool isAlive;
    //Set the player to dead
    public void PlayerDied() { isAlive = false; }
    public void PlayerReset() { isAlive = true; }

	// Use this for initialization
	void Start () {
        //Call the initialization
        Init();
	}

    void Init()
    {
        //Get the player object to chase
        playerObj = GameObject.Find("Player");
        //Set is alive to true, therefore allowing the chase camera to follow the player
        isAlive = true;
    }
	
	// Update is called once per frame
	void Update () {  
        //Updating the Camera every frame to chase the player if applicable
        ChasePlayer();
	}

    private void ChasePlayer()
    {
        //If the player is alive
        if (isAlive)
        {
            //Set the position of the camera to follow the player object
            transform.parent.position = playerObj.transform.position;
        }       
    }

    //Called to rotate the camera on player input, requires a speed factor 
    public void RotateCam(float speed)
    {
        //Rotate around y Axis the speed by the delta time to remain consistant throughout various frame rates
        transform.parent.Rotate(0, speed*Time.deltaTime, 0);
    }
}
