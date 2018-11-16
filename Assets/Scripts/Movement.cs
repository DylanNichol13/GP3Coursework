using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    //Movement vector to be applied to Rigidbody
    private Vector3 movement;
    //Get the ground obj
    private GameObject groundObj;
    //Movement target
    private Vector3 target;

    //Configurable in inspector
    [SerializeField]
    private float speed;
    [SerializeField]
    private GameObject targetHighlighter;

    //Called on start of application
    private void Start()
    {
        //Get the world object
        groundObj = GameObject.Find("GameWorld");
        //Hide target highlighter first
        targetHighlighter.transform.localScale = new Vector3(0, 0, 0);
    }

	private void FixedUpdate()
    {
        //Moving to mouse position
        MouseMovement();
        //Clamp player movement
        CheckBoundaries();
        if (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            targetHighlighter.transform.position = target;
            targetHighlighter.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            targetHighlighter.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    private void MouseMovement()
    {
        if (Input.GetMouseButton(0)) { 
        //Raycast to find the terrain
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //If the ray hits an object
            if (Physics.Raycast(ray, out hit, 1000))
            {
                //Make the movement target the hit location of mouse
                target = hit.point;
                //Apply necessary force to move to the target position
                //AddTheForce((target - transform.position) * 0.5f);
                //Draw debugging line
                Debug.DrawLine(ray.origin, hit.point);
            }
        }
    }

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
                break;
            //Red object
            case ("redObj"):
                //Destroy the player
                Debug.Log("You hit a red sphere and died!");
                Destroy(gameObject);
                Camera.main.GetComponent<CameraController>().PlayerDied();
                break;
        }
    }

    //Continuous collision
    private void OnCollisionStay(Collision col)
    {
    }

    //Collision ended
    private void OnCollisionExit(Collision col)
    {
    }
}
