using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerScript : MonoBehaviour
{
    [SerializeField]
    private float chaseCoolDown;
    public float coolDownTimer;

    [SerializeField]
    private float speed;

    private GameObject playerObj;
    private PlayerEnergyScript energyScript;
    private Movement movementScript;

    //Energy min and max amounts
    private float minEnergy = 0;
    private float maxEnergy = 100;
    private float currentEnergy = 0;

    // Use this for initialization
    void Start()
    {
        //Get player obj
        playerObj = GameObject.Find("Player");
        //Get player comps
        energyScript = playerObj.GetComponent<PlayerEnergyScript>();
        movementScript = playerObj.GetComponent<Movement>();
        //Set cooldown timer to 0 to start following 
        coolDownTimer = 0;
        //Get first colour
        UpdateColour();
    }

    public void StartGame()
    {
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
        //otherwise, focus on reducing cooldown
        else
            ReduceCoolDown();
    }

    //Called to chase the player bobject
    private void ChasePlayer()
    {
        //Move towards player
        transform.position = Vector3.MoveTowards(transform.position, playerObj.transform.position, Time.deltaTime * speed);
    }

    //When the player collides, this is called from the player movement script
    public void CollisionWithPlayer()
    {
        print(CanCollide());
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
    private Vector3 GetRandomStartPos()
    {
        //Get the renderer comp from world map
        Renderer worldRenderer = GameObject.Find("GameWorld").GetComponent<Renderer>();
        //Min values of mesh
        float minX = worldRenderer.bounds.min.x;
        float minY = worldRenderer.bounds.min.y;
        float minZ = worldRenderer.bounds.min.z;
        //Max vals of mesh
        float maxX = worldRenderer.bounds.max.x;
        float maxY = worldRenderer.bounds.max.y;
        float maxZ = worldRenderer.bounds.max.z;
        //Randomize X and Z around edge of mesh using an offset for max possible distance
        float xPos = UnityEngine.Random.value > 0.5f ? 
            UnityEngine.Random.Range(minX, minX + offsetMax) :
            UnityEngine.Random.Range(maxX, maxX - offsetMax);

        float zPos = UnityEngine.Random.value > 0.5 ?
            UnityEngine.Random.Range(minZ, minZ + offsetMax) :
            UnityEngine.Random.Range(maxZ, maxZ - offsetMax);
        //Return random pos
        return new Vector3(xPos, minY, zPos);
    }


    private bool CanCollide()
    {
        if (coolDownTimer <= 0)
        {
            return true;
        }
        else return false;
    }

    private void StartChaseCoolDown()
    {
        coolDownTimer = chaseCoolDown;
    }

    private void ReduceCoolDown()
    {
        coolDownTimer -= Time.deltaTime;
    }

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

    private void AddEnergy(float energy)
    {
        currentEnergy += energy;
    }


    //Colision start
    private void OnCollisionEnter(Collision col)
    {
        //Find action for the tag of the object
        switch (col.gameObject.tag)
        {
            case "blackHole":
                StartGame();
                break;
        }
    }
}
