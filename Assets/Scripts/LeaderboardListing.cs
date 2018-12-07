using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardListing {
    //Name of player
    public string name;
    //Score amount
    public float score;
    //ranking in lb
    public int rank;
    //Constructing a new leaderboard listing
    public LeaderboardListing(string name_, float score_, int rank_)
    {
        //Use name arguement
        name = name_;
        //Use score argument
        score = score_;
        //Use ranking argument
        rank = rank_;
    }
}
