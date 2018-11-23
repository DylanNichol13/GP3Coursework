using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour {
    SQLtest sql;
    GameObject leaderboardObj;

	// Use this for initialization
	void Start () {
        sql = GameObject.Find("Player").GetComponent<SQLtest>();
        leaderboardObj = GameObject.Find("ListingObj");
        leaderboardObj.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayLeaderBoard()
    {
        List<LeaderboardListing> list = new List<LeaderboardListing>();
        list = sql.GetTopFive();
        print(list.Count);
        foreach(LeaderboardListing l in list)
        {
            GameObject newObj = Instantiate(leaderboardObj);
            newObj.transform.parent = leaderboardObj.transform.parent;
            newObj.SetActive(true);
            newObj.transform.GetChild(1).GetComponent<Text>().text = l.name;
            newObj.transform.GetChild(2).GetComponent<Text>().text = l.score.ToString();
        }
    }

   
}
