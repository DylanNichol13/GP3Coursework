using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    //map boundaries
    private float xBound = 45;
    private float zBound = 50;
    //Sphere movement speed
    private float speed = 10f;
    //Movement vector to be applied to Rigidbody
    private Vector3 movement;

	void FixedUpdate()
    {
        //Moving to mouse position
        MouseMovement();
        //Clamp player movement
        CheckBoundaries(xBound, zBound);
    }

    void MouseMovement()
    {
        //Raycast to find the terrain
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //If the ray hits an object
        if (Physics.Raycast(ray, out hit, 1000))
        {
            //Make the movement target the hit location of mouse
            Vector3 target = hit.point;
            //Apply necessary force to move to the target position
            AddTheForce((target - transform.position) *0.01f);
            //Draw debugging line
            Debug.DrawLine(ray.origin, hit.point);
        }
    }

    void AddTheForce(Vector3 movement)
    {
        GetComponent<Rigidbody>().AddForce(movement * speed);
    }

    //Check the player against map boundaries
    void CheckBoundaries(float x, float z)
    {
        //Current X and Z positions
        float xPos = transform.position.x;
        float zPos = transform.position.z;

        //Check against X boundaries and reset if needed
        if (xPos > x)
            xPos = x;
        else if (xPos < -x)
            xPos = -x;

        //Check agains Z boundaries and reset if needed
        if (zPos > z)
            zPos = z;
        else if (zPos < -z)
            zPos = -z;

        //Set the position based on any necessary changes
        SetPosition(new Vector3(xPos, transform.position.y, zPos));
    }

    //Set position of the sphere based on boundaries above, to avoid falling off map
    void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    //Colision start
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Collision Enter");
        //Find action for the tag of the object
        switch (col.gameObject.tag)
        {
            //Blue object
            case ("blueObj"):
                //Destroy the blue cube
                Debug.Log("You collected a blue cube!");
                Destroy(col.gameObject);
                break;
            //Red object
            case ("redObj"):
                //Destroy the player
                Debug.Log("You hit a red sphere and died!");
                Destroy(gameObject);
                break;
        }
    }

    //Continuous collision
    void OnCollisionStay(Collision col)
    {
        Debug.Log("Collding");
    }

    //Collision ended
    void OnCollisionExit(Collision col)
    {
        Debug.Log("Collision Ended");
    }
}
