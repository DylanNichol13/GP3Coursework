﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {
    //Array of obstacles
    ArrayList obstacles;
    //Parent object for obstacles
    GameObject obstacleContainer;
    //Renderer of world
    Renderer worldRenderer;

    //Set in inspector
    [SerializeField]
    Vector3 blackHoleSize;

	// Use this for initialization
	public void StartNewGame () {
        //Get the renderer comp from world map
        worldRenderer = GameObject.Find("GameWorld").GetComponent<Renderer>();
        //Clear old objects
        ClearObjects();
        //Generating obstacle container
        obstacleContainer = CreateObstacleContainer();
        //Generate blue cube obstacles
        obstacles = CreateObstacles();
        //Generating red sphere obstacles
        GenerateObstacles();
        //Gen a black hole
        GenerateBlackHole();
	}
    //Called on start to clear objects from a previous session
    private void ClearObjects()
    {
        //Check if there is already a container, 
        if (obstacleContainer != null)
            //If there is, destroy this, thus removing old obstacles
            GameObject.Destroy(obstacleContainer);
    }
    //called on start to create a container in hierarchy
    private GameObject CreateObstacleContainer()
    {
        //Initialize the object
        GameObject newObj = new GameObject();
        //Change the name of new object
        newObj.name = "Obstacle Container";
        //Return
        return newObj;
    }

    private ArrayList CreateObstacles()
    {
        //Initialize array list
        ArrayList obs = new ArrayList();
        //Add objects to arraylist
        for(int i = 0; i < 6; i++)
        {
            GameObject newObstacle = CreateObstacle(i);
            obs.Add(newObstacle);
        }
        //Return array list
        return obs;
    }

    private GameObject CreateObstacle(int id)
    {
        //Declaration of new gameobject
        GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Randomize the obstacle starting position
        newObj.transform.position = GetRandomWorldPos();
        //Get renderer and rigidbody components
        Renderer renderer = newObj.GetComponent<Renderer>();
        Rigidbody rb = newObj.AddComponent<Rigidbody>();
        //Set material color
        renderer.material.color = Color.blue;
        //Set name based on ID
        newObj.name = "New Obstacle " + id;
        //Set the parent for tidy hierarchy
        newObj.transform.parent = obstacleContainer.transform;
        //Set tag of blue obj
        newObj.tag = "blueObj";
        //Add mushroom script to the object
        newObj.AddComponent<MushroomScript>();
        //Return the new object
        return newObj;
    }

    //Generate objects randomly on the map
    private void GenerateObstacles()
    {
        //Randomize the count of obstacles
        int amount = Random.Range(5, 13);
        for (int i = 0; i < amount; i++) {
            //Creat the object
            GameObject redSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //Randomize the obstacle starting position
            redSphere.transform.position = GetRandomWorldPos();
            //Get renderer and rigidbody components
            Renderer renderer = redSphere.GetComponent<Renderer>();
            Rigidbody rb = redSphere.AddComponent<Rigidbody>();
            //Set material color
            renderer.material.color = Color.red;
            //Set the parent for tidy hierarchy
            redSphere.transform.parent = obstacleContainer.transform;
            //Set tag of red object
            redSphere.tag = "redObj";
            //Set object name in hierarchy
            redSphere.name = "redSphere: " + i;
        }
    }

    //Generate a black hole
    private void GenerateBlackHole()
    {
        //Create cylinder primitive
        GameObject blackHole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //Set size to predetermined size
        blackHole.transform.localScale = blackHoleSize;
        //Get a random position for the black hole
        blackHole.transform.position = GetRandomWorldPos();
        
        //Get components
        Renderer renderer = blackHole.GetComponent<Renderer>();
        //Add rigidbody comp
        Rigidbody rb = blackHole.AddComponent<Rigidbody>();
        //Disable Capsule Collider
        blackHole.GetComponent<CapsuleCollider>().enabled = false;
        //Add box collision
        blackHole.AddComponent<BoxCollider>();
        //Freeze position and rotations
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //Set to black
        renderer.material.color = Color.black;
        //Set the parent for tidy hierarchy
        blackHole.transform.parent = obstacleContainer.transform;
        //Change name and tags
        blackHole.name = "blackHole";
        blackHole.tag = "blackHole";
    }

    //Get a random position to spawn objects
    Vector3 GetRandomWorldPos()
    {
        //Get random position coords depending on the mesh boundaries
        float x = Random.Range(worldRenderer.bounds.min.x, worldRenderer.bounds.max.x);
        float z = Random.Range(worldRenderer.bounds.min.z, worldRenderer.bounds.max.z);

        //Return the new position with a default Y value
        return new Vector3(x, worldRenderer.bounds.max.y, z);
    }
}
