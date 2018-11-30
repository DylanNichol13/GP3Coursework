using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {

    //Is the game being played - true or false
    public static bool instance;

    private void Awake() {
        //On start, game is not being played
        instance = false;
    }

    public void SetInstance()
    {
        instance = true;
    }

    public void DisableInstance()
    {
        instance = false;
    }
}
