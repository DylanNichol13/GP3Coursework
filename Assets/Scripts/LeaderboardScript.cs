using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour {
    //Script which controls SQLite leaderboard data
    private SQLtest sql;
    //Default gameobject used to add leaderboard listings to populate the scoreboard
    private GameObject leaderboardObj;

	// Use this for initialization
	void Start () {
        //Initialize components held in this script
        Init();
	}
	
	// Called to initialize the script
	private void Init () {
        //Get the SQL highscore data holder
        sql = GameObject.Find("Player").GetComponent<SQLtest>();
        //Get the base leaderboard object, to be duplicated when populating a leaderboard
        leaderboardObj = GameObject.Find("ListingObj");
        //Set initial object to inactive, as this is only used to be duplicated
        leaderboardObj.SetActive(false);
    }

    //Called when wanting a leaderboard dispalyed
    public void DisplayLeaderBoard()
    {
        //Clear any existing leaderboards, to avoid duplication
        ClearExistingLeaderboard();
        //List which will hold leaderboard listings
        List<LeaderboardListing> list = new List<LeaderboardListing>();
        //Get the top 5 scores from the SQLite database
        list = sql.GetTopFive();
        //Loop through each of the top 5 scoreres, recieved from SQlite databse
        foreach(LeaderboardListing l in list)
        {
            //Duplicate the leaderboard listing object for a new listings
            GameObject newObj = Instantiate(leaderboardObj);
            //Set the parent, so that this new object is displayed in the leaderboard UI panel
            newObj.transform.parent = leaderboardObj.transform.parent;
            //Set to active, as default obj is inactive
            newObj.SetActive(true);
            //Set the first text component to the name of the listing, which is entered at game end
            newObj.transform.GetChild(1).GetComponent<Text>().text = l.name;
            //Show the points which this player totalled
            newObj.transform.GetChild(2).GetComponent<Text>().text = l.score.ToString() + " Points";
        }
    }

    //Clear all existing leaderboard data held in the scene
    private void ClearExistingLeaderboard()
    {
        //Get parent container
        Transform container = leaderboardObj.transform.parent;
        //Counter for destroying leaderboard objects
        int index = 0;
        //Get each listing
        foreach (Transform t in container)
        {
            //Do not destroy the prefab object
            if(index > 0)
                //Destroy
                Destroy(t.gameObject);
            //Incremement for next step
            index++;
        }
    }
}
