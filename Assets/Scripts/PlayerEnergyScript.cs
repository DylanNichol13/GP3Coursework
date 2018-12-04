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
        SetInitialEnergy();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetInitialEnergy()
    {
        currentEnergy = minEnergy;
    }

    public void AddEnergy(float energyToAdd)
    {
        currentEnergy += energyToAdd;
        currentEnergy = ClampEnergy();

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

    private float ClampEnergy()
    {
        float energy = currentEnergy;
        if (energy < minEnergy)
            return minEnergy;
        else if (energy > maxEnergy)
            return maxEnergy;

        return energy;
    }
}
