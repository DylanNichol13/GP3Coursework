using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //The player object
    private GameObject playerObj;
    //Game is still active
    private bool isAlive = true;
    //Set the player to dead
    public void PlayerDied() { isAlive = false; }
    public void PlayerReset() { isAlive = true; }

	// Use this for initialization
	void Start () {
        playerObj = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if(isAlive)
            ChasePlayer();
	}

    private void ChasePlayer()
    {
        transform.position = playerObj.transform.position;

        transform.position = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z-30);
    }
}
