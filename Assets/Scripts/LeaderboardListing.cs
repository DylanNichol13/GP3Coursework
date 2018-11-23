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

    public LeaderboardListing(string name_, float score_, int rank_)
    {
        name = name_;
        score = score_;
        rank = rank_;
    }
}
