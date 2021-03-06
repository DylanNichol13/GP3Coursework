﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    //Movement vector to be applied to Rigidbody
    private Vector3 movement;
    //Get the ground obj
    private GameObject groundObj;
    //Movement target
    private Vector3 target;
    //Get the energy script for reference
    private PlayerEnergyScript energyScript;
    //Game controller
    private GameController controller;
    //Rigid body of object
    private Rigidbody rb;

    //Configurable in inspector
    [SerializeField]
    private float speed;
    //Called on start of application
    private void Start()
    {
        //Get components required in script
        Init();
    }

    //Called to initialize properties by getting components
    private void Init()
    {
        //Get the world object
        groundObj = GameObject.Find("GameWorld");
        //Get the energy script held on this object
        energyScript = GetComponent<PlayerEnergyScript>();
        //Get the game controller
        controller = GameObject.Find("Controller").GetComponent<GameController>();
        //Get rigidbody on this object
        rb = GetComponent<Rigidbody>();
    }

    //Called when starting a new game
    public void StartGame()
    {
        //Set the scale of the player object
        transform.localScale = new Vector3(3, 3, 3);
        //Starting position in center of the map
        transform.position = new Vector3(0, 7.5f, 0);
        //Reset the player energy to 0
        energyScript.ResetEnergy();
    }

	private void FixedUpdate()
    {
        //If the game is being played
        if (StateController.instance)
        {
            //Moving to based on key presses
            CheckMovement();
            //Clamp player movement
            CheckBoundaries();
        }

    }

    private void CheckMovement()
    {
        //Move forward with A or UP
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            //Move forward acording to camera rotations
            rb.AddForce(Camera.main.transform.forward * speed);
        }
        //Move left with A or LEFT
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //Rotate object
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 3, transform.eulerAngles.z);
            //Rotate camera view
            Camera.main.GetComponent<CameraController>().RotateCam(-130);
        }
        //Move right with D or RIGHT
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //Roate gameobject to show movement
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 3, transform.eulerAngles.z);
            //Rotate camera view
            Camera.main.GetComponent<CameraController>().RotateCam(130);
        }
        //Move back with S or DOWN
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            //Moving backwards is slowed
            rb.AddForce(-(Camera.main.transform.forward * speed));
        }
    }

    //Adding movement force 
    private void AddTheForce(Vector3 movement)
    {
        GetComponent<Rigidbody>().AddForce(movement * speed);
    }

    //Check the player against map boundaries
    private void CheckBoundaries()
    {
        //Current X and Z positions
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        float zPos = transform.position.z;
        //Origin X and Z of the game world 
        float groundX = groundObj.transform.position.x;
        float groundZ = groundObj.transform.position.z;
        //Half of the map dimensions, to be added to the origin to suitably wrap
        float halfX = groundObj.GetComponent<Renderer>().bounds.size.x / 2f;
        float halfZ = groundObj.GetComponent<Renderer>().bounds.size.z / 2f;

        //Check against X boundaries and reset if needed
        if (xPos > groundX + halfX)
            xPos = groundX - halfX;
        else if (xPos < groundX - halfX)
            xPos = groundX + halfX;

        //Check against Z boundaries and reset if needed
        if (zPos > groundZ + halfZ)
            zPos = groundZ - halfZ;
        else if (zPos < groundZ - halfZ)
            zPos = groundZ + halfZ;


        //Reset Y if wrapping to avoid falling from map
        if (xPos != transform.position.x || zPos != transform.position.z)
            yPos = 12;
        //Set the posiSetYOnWraption based on any necessary changes
        SetPosition(new Vector3(xPos, yPos, zPos));
    }

    //Set position of the sphere based on boundaries above, to avoid falling off map
    private void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    //Colision start
    private void OnCollisionEnter(Collision col)
    {
        //Find action for the tag of the object
        switch (col.gameObject.tag)
        {
            //Blue object
            case ("blueObj"):
                //Destroy the blue cube
                Debug.Log("You collected a blue cube!");
                Destroy(col.gameObject);
                energyScript.AddEnergy(energyScript.GetMushroomEnergy());
                break;
            //Red object
            case ("redObj"):
                //Destroy the player
                Debug.Log("You hit a red sphere and died!");
                KillPlayer();
                break;
            //Enemy Player
            case ("enemyPlayer"):
                col.gameObject.GetComponent<EnemyPlayerScript>().CollisionWithPlayer();
                break;
            //hit black hole
            case ("blackHole"):
                //Kill player
                KillPlayer();
                break;
            //Hit poison mushroom
            case ("poisonObj"):
                Destroy(col.gameObject);
                energyScript.AddEnergy(energyScript.GetNegativeEnergy());
                break;


        }
    }

    //Kill the player after death criteria met
    private void KillPlayer()
    {
        //End game process
        controller.EndGame();
        //Destroy the object
        transform.localScale = new Vector3(0, 0, 0);
        //Set camera to stop following
        Camera.main.GetComponent<CameraController>().PlayerDied();
    }
}
