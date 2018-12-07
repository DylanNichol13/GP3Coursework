using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyScript : MonoBehaviour {
    //Energy min and max amounts
    private float minEnergy = 0;
    private float maxEnergy = 100;
    //Current Energy
    private float currentEnergy;
    //Get current energy
    public float GetEnergy() { return currentEnergy; }
    public void ResetEnergy() { currentEnergy = minEnergy; }

    //Amount added by collecting a mushroom
    [SerializeField]
    private float mushroomEnergy;
    [SerializeField]
    private float negativeMushroomEnergy;
    [SerializeField]
    private float enemyLeechEnergy;
    //Getter for energy 
    public float GetMushroomEnergy() { return mushroomEnergy; }
    public float GetNegativeEnergy() { return negativeMushroomEnergy; }
    public float GetLeechEnergy() { return enemyLeechEnergy; }
    public void SetPlayerEnergy(float amount) { currentEnergy = amount; }

	// Use this for initialization
	void Start () {
        //Set the initial energy of the player
        SetInitialEnergy();
	}

    //Reset energy to minimum value
    private void SetInitialEnergy()
    {
        //Set to minimum
        currentEnergy = minEnergy;
    }

    //Add energy collected, using the parameter
    public void AddEnergy(float energyToAdd)
    {
        //Add the energy to the current energy amount
        currentEnergy += energyToAdd;
        //Clmap between min and max
        currentEnergy = ClampEnergy();
        //Update the colour of the player object material
        UpdateColour();
    }


    //Update the colour
    private void UpdateColour()
    {
        //Max colour value
        float colourMax = 1;
        //Calculate colour values
        //Take the percentage of the full tank, from the max colour amount to get the correct colour with 0 being black and
        //1 being white
        float colourValue = colourMax - ((currentEnergy / maxEnergy) * colourMax);
        //Create new colour and assign the calculated value
        Color c = new Color(colourValue, colourValue, colourValue);
        //Assign this new colour to the material
        GetComponent<Renderer>().material.color = c;
    }

    //Clamp the player energy to stay between min and max values
    private float ClampEnergy()
    {
        //Get the current energy
        float energy = currentEnergy;
        //Do not let energy go under minimal amount
        if (energy < minEnergy)
            return minEnergy;
        //Do not let energy exceed max
        else if (energy > maxEnergy)
            return maxEnergy;
        //Return the adjusted value
        return energy;
    }
}
