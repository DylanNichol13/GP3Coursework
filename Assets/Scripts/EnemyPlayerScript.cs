using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerScript : MonoBehaviour
{
    [SerializeField]
    //Cool down required to excede before the enemy can chase player after catching
    private float chaseCoolDown;
    //Current time on the cool down, when reaches 0, enemy can chase again
    public float coolDownTimer;
    [SerializeField]
    //The speed that the enemy moves at
    private float speed;
    //Player game object
    private GameObject playerObj;
    //Script which holds details of the player energy
    private PlayerEnergyScript energyScript;
    //Script which holds details of player movements
    private Movement movementScript;
    //Energy min and max amounts
    private float minEnergy = 0;
    private float maxEnergy = 100;
    //Current energy amount of enemy
    private float currentEnergy = 0;

    //Getter and setter for enemy energy
    public float GetEnemyEnergy() { return currentEnergy; }
    public void SetEnemyEnergy(float amount) { currentEnergy = amount; }

    // Use this for initialization
    void Start()
    {
        //Call to initialize upon starting
        Init();
    }

    //Called upon instantiation
    private void Init()
    {
        //Get player obj
        playerObj = GameObject.Find("Player");
        //Get player script components
        //Energy script
        energyScript = playerObj.GetComponent<PlayerEnergyScript>();
        //movement script
        movementScript = playerObj.GetComponent<Movement>();
        //Set cooldown timer to 0 to start following 
        coolDownTimer = 0;
        //Update the colour of the enemy material, used here to get the starting colour based on energy
        UpdateColour();
    }

    //When a new game is started and not loaded from a saved file
    public void StartGame()
    {
        //Randomize the starting position
        transform.position = GetRandomStartPos();
    }

    // Update is called once per frame
    void Update()
    {
        //If chase is off cooldown
        if (CanCollide() && StateController.instance)
        {
            //Chase the player
            ChasePlayer();
        }
        //otherwise, focus on reducing cooldown first before chasing
        else
            ReduceCoolDown();
    }

    //Called to chase the player bobject
    private void ChasePlayer()
    {
        //Move towards player based on the speed value
        transform.position = Vector3.MoveTowards(transform.position, playerObj.transform.position, Time.deltaTime * speed);
    }

    //When the player collides, this is called from the player movement script
    public void CollisionWithPlayer()
    {
        //Check that the enemy is off cooldown
        if (CanCollide())
        {
            //Set chase on cooldown
            StartChaseCoolDown();
            //Leech X amount of energy from player
            energyScript.AddEnergy(energyScript.GetLeechEnergy());
            //add energy to this enemy object, this being the negative of the leeched amount from the player obj
            AddEnergy(-energyScript.GetLeechEnergy());
            //Update colour
            UpdateColour();
        }
    }

    //Max possible offset of position
    [SerializeField]
    private float offsetMax;
    //Called to return a random starting position
    private Vector3 GetRandomStartPos()
    {
        //Get the renderer comp from world map
        Renderer worldRenderer = GameObject.Find("GameWorld").GetComponent<Renderer>();
        //Get the minimum values on each axis based on the game world mesh size
        float minX = worldRenderer.bounds.min.x;
        float minY = worldRenderer.bounds.min.y;
        float minZ = worldRenderer.bounds.min.z;
        //Get the maximum values of each axis using the game world mesh size
        float maxX = worldRenderer.bounds.max.x;
        float maxY = worldRenderer.bounds.max.y;
        float maxZ = worldRenderer.bounds.max.z;
        //Randomize X and Z around edge of mesh using an offset for max possible distance
        float xPos = UnityEngine.Random.value > 0.5f ? 
            UnityEngine.Random.Range(minX, minX + offsetMax) :
            UnityEngine.Random.Range(maxX, maxX - offsetMax);
        //Get random Z position around the edge of the Z axis of the mesh
        float zPos = UnityEngine.Random.value > 0.5 ?
            UnityEngine.Random.Range(minZ, minZ + offsetMax) :
            UnityEngine.Random.Range(maxZ, maxZ - offsetMax);
        //Return the new random pos
        return new Vector3(xPos, minY, zPos);
    }

    //Called on a collision to check if the enemy is elligible for stealing energy
    private bool CanCollide()
    {
        //Check the cooldown timer to see if it has run out, thus allowing a collision
        if (coolDownTimer <= 0)
        {
            return true;
        }
        //Otherwise, cannot drain energy & ignores collision behaviour
        else return false;
    }

    //Called to reset the chase cooldown timer after successfully colliding with the player
    private void StartChaseCoolDown()
    {
        //Set the timer back to the base cool down time
        coolDownTimer = chaseCoolDown;
    }

    //Reducing the cooldown every second when it requires reduction & the enemy cannot collide with the player
    private void ReduceCoolDown()
    {
        //Reduce timer every second
        coolDownTimer -= Time.deltaTime;
    }

    //Called to update the colour at the start of the game & successful collision with the player
    public void UpdateColour()
    {
        //Max colour value
        float colourMax = 1;
        //Calculate colour values
        //Take the percentage of the full tank, from the max colour amount to get the correct colour with 0 being black and
        //1 being white
        float colourValue = 0.3f + ((currentEnergy / maxEnergy) * colourMax);
        print(colourValue);
        //Create new colour and assign the calculated value
        Color c = new Color(0, 0, colourValue);
        //Assign this new colour to the material
        GetComponent<Renderer>().material.color = c;
    }

    //Adding stolen energy to the enemy from the player, requires an amount to add
    private void AddEnergy(float energy)
    {
        //Increase the current energy value by the amount stolen
        currentEnergy += energy;
    }


    //Colision start
    private void OnCollisionEnter(Collision col)
    {
        //Find action for the tag of the object
        switch (col.gameObject.tag)
        {
            //After colliding with a black hole -
            case "blackHole":
                //Position is randomized using the random position criteria
                StartGame();
                break;
        }
    }
}
