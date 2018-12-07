using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomScript : MonoBehaviour {
    //Max death age across all mushrooms
    public static float maxDeathAge = 40f;

    private float timeElapsed = 0;
    //This mushroom instance
    public Mushroom mushroom;
    //
    float duration = 20;

    //Mushroom Class
    public class Mushroom
    {
        //This mushroom age
        private float age;
        //This mushroom death age
        private float deathAge;
        //Oscillate offset
        public float offset;
        //starting position
        public Vector3 mushroomPos;
        //starting scale
        public Vector3 mushroomScale;

        //Mushroom Constructor
        public Mushroom()
        {
            //Default age
            age = 0;
            //Get death age
            deathAge = Random.Range(1, maxDeathAge);
            //Offset
            offset = Random.Range(1, 380);
        }
        //Constructing a mushroom from saved data
        public Mushroom(float _age, float _deathAge)
        {
            //Default age
            age = _age;
            //Get death age
            deathAge = _deathAge;
            //Offset
            offset = Random.Range(1, 380);
        }

        //Detect when the mushroom reaches half its life
        public bool ReachedHalfLife()
        {
            //Calculate half of life span
            float halfLifeAmount = deathAge / 2;
            //Check if half life has expired
            if (age >= halfLifeAmount)
            {
                return true;
            }
            else return false;
        }

        //Detect when a mushroom has died
        public bool ReachedDeathAge()
        {
            //Check age against age the mushroom should die at
            if (age >= deathAge)
            {
                //Exceded death age
                return true;
            }
            //Mushroom still alive
            else return false;
        }

        //Incrementing age
        public void CalculateAge()
        {
            //Add 1 second each second
            age += 1 * Time.deltaTime;
        }

        public float GetAge() { return age; }
        public float GetMaxAge() { return deathAge; }
    }

    //Called the obstacle controller creates a new mushroom NOT from saved data
    public void NewMushroom()
    {
        //Randomize age, deathage and oscillate offset
        mushroom = new Mushroom();
    }

    //Called each frame
    private void Update()
    {
        //Make sure the game has not been paused
        if (StateController.instance)
        {
            //Grow until at max scale
            GrowMushroom();
            //Update the mushroom age
            mushroom.CalculateAge();
            //Calc elapsed time
            CalcElapsedTime();
            //Oscillate this object 
            Oscillate();
            //Checking the age of the mushroom, to complete half-life and death actions
            if (mushroom.ReachedHalfLife())
                CreatePoisonMushroom();
            if (mushroom.ReachedDeathAge())
                FloatingBehaviour();
        }
    }
    //Max time to complete growth in seconds
    float scalingTime = 8f;
    //Lerping timer, which starts at 0 and reaches 'scalingTime' upon completion
    float elapsedTime = 0;
    //For growing upon instantiation
    private void GrowMushroom()
    {
        //Target scales
        Vector3 targetScale = new Vector3(1, 1, 1);
        //If timer has not completed
        if (scalingTime > elapsedTime)
        {
            //Increase timer
            elapsedTime += Time.deltaTime;
            //Lerp scale, until at max value, to represent steady fluid growng
            transform.localScale = Vector3.Lerp(transform.lossyScale, targetScale, elapsedTime/scalingTime);
        }
    }

    //For shrinking reference to the original scale
    Vector3 startingScale;
    //For floating reference to the original position
    Vector3 startingPos;
    //Target vectors
    Vector3 targetScale, targetPos;
    //Get starting vectors only once
    bool firstIteration = true;
    private void FloatingBehaviour()
    {
        //Only grabbed once
        if(firstIteration)
        {
            //Get original scale and position
            startingScale = transform.localScale;
            startingPos = transform.localPosition;
            //Set target scales and positions
            //scale object in half
            targetScale = new Vector3(startingScale.x / 2, startingScale.y / 2, startingScale.z / 2);
            //Move target position 2 points above on Y axis
            targetPos = new Vector3(startingPos.x, startingPos.y + 2, startingPos.z);
            //Disable gravity to float upwards
            GetComponent<Rigidbody>().useGravity = false;
            //Disable retrieval from happening again
            firstIteration = false;
        }

        //Downscale the mushroom object
        if(transform.localScale.x > targetScale.x) {
            transform.localScale = new Vector3(transform.localScale.x - transform.localScale.x*0.01f, transform.localScale.y - transform.localScale.y * 0.01f, transform.localScale.z - transform.localScale.z * 0.01f);
        }
        //Move object to the target position
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, 2 * Time.deltaTime);      

        //Destroy the object once it has reached the target volume
        if(targetScale.x >= transform.localScale.x)
        {
            //Remove game object from game
            GameObject.Destroy(gameObject);
        }
    }

    private void Oscillate()
    {
        //Calculation using sin of elapsed time and this random offset associated with the mushroom instance
        float osc = Mathf.Sin(timeElapsed + mushroom.offset);
        //Apply rotation using this value
        transform.localEulerAngles = new Vector3(osc, osc, osc);
    }

    //Setting a mushroom to poisionous
    private void CreatePoisonMushroom()
    {
        //Change tag, which is required on the movement script when the player collides with an object
        gameObject.tag = "poisonObj";
        //Change the colour of object to signal poison
        gameObject.GetComponent<Renderer>().material.color = Color.magenta;
    }
    //Calculate the current elapsed time of the mushroom for oscillation
    private void CalcElapsedTime()
    {
        //Increment
        timeElapsed += Time.deltaTime;
    }

}
