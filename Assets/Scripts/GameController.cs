using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject[] uiObjects;

    Vector3 hidden = new Vector3(0, 0, 0);
    Vector3 shown = new Vector3(1, 1, 1);

    private ObstacleController obstacleController;
    private Movement movementScript;

	// Use this for initialization
	void Start () {
        ResetUI();
        EnableMenu();
        obstacleController = GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>();
        movementScript = GameObject.Find("Player").GetComponent<Movement>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ResetUI()
    {
        foreach (GameObject g in uiObjects)
            g.transform.localScale = hidden;
    }

    public void EndGame()
    {
        ResetUI();
        uiObjects[0].transform.localScale = shown;
    }

    public void EnableMenu()
    {
        ResetUI();
        uiObjects[1].transform.localScale = shown;
    }

    public void StartGame()
    {
        ResetUI();
        obstacleController.StartNewGame();
        movementScript.StartGame();
        Camera.main.GetComponent<CameraController>().PlayerReset();
    }

    public void EnableScoreboard()
    {
        ResetUI();
        uiObjects[2].transform.localScale = shown;

        GetComponent<LeaderboardScript>().DisplayLeaderBoard();
    }
}
