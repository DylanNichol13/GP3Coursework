using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerScript : MonoBehaviour {
    //Player Object
    //Show in inspector
    [SerializeField]
    private GameObject playerObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Chase the player
        ChasePlayer();
	}

    //Called to chase the player bobject
    private void ChasePlayer()
    {

    }
}
