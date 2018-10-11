using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {
    //Array of obstacles
    ArrayList obstacles;
    //Parent object for obstacles
    GameObject obstacleContainer;

	// Use this for initialization
	void Start () {
        //Generating obstacle container
        obstacleContainer = CreateObstacleContainer();
        //Generate blue cube obstacles
        obstacles = CreateObstacles();
        //Generating red sphere obstacles
        GenerateObstacles();
	}

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
        newObj.transform.position = new Vector3(Random.Range(-20, 20), 5, Random.Range(-20, 20));
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
            redSphere.transform.position = new Vector3(Random.Range(-20, 20), 5, Random.Range(-20, 20));
            //Get renderer and rigidbody components
            Renderer renderer = redSphere.GetComponent<Renderer>();
            Rigidbody rb = redSphere.AddComponent<Rigidbody>();
            //Set material color
            renderer.material.color = Color.red;
            //Set tag of red object
            redSphere.tag = "redObj";
            //Set object name in hierarchy
            redSphere.name = "redSphere: " + i;
        }
    }
}
