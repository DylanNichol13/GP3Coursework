using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SQLtest : MonoBehaviour
{
    //path to DB
    private string conn;
    //Connection to DB
    private IDbConnection dbConn;

    private GameController controller;

    // Use this for initialization
    void Start()
    {
        //Initialize the script
        Init();
    }

    //Initialize on start
    private void Init()
    {
        //Declare the path to DB
        conn = "URI=file:" + Application.dataPath + "/test.db";
        //Create highscore table if required
        CreateTable();
        //Get the controller component from scene
        controller = GameObject.Find("Controller").GetComponent<GameController>();
    }

    //Create highscore table
    private void CreateTable()
    {
        //Connect and open DB
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();
        //Create command for queries
        IDbCommand cmd = dbConn.CreateCommand();
        //Create new highscore table, requiring name and score vars if none exists. Has an autoincrementing key
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS highscore (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(20), score INT);";
        cmd.ExecuteNonQuery();
        //Close the DB
        dbConn.Close();
    }

    public void ClearLeaderboard()
    {
        //Open DB
        dbConn.Open();
        //New command
        IDbCommand cmd = dbConn.CreateCommand();
        //Delate the table 'highscore'
        cmd.CommandText = "DROP TABLE IF EXISTS highscore;";
        cmd.ExecuteNonQuery();
        //Close db and dispose commands to finish
        cmd.Dispose();
        dbConn.Close();
        //Go to the menu
        GameObject.Find("Controller").GetComponent<GameController>().EnableMenu();
    }

    public void AddToDatabase()
    {
        //Get name from Input Field
        string name = GameObject.Find("InputField").transform.GetChild(2).gameObject.GetComponent<Text>().text;
        //Get Score
        float score = GetComponent<PlayerEnergyScript>().GetEnergy();
        //OPEN DB
        dbConn.Open();
        //Create command
        IDbCommand cmd = dbConn.CreateCommand();
        //Insert new record to DB
        cmd.CommandText = "insert into highscore (name, score) VALUES (@Name, @Score);";
        //Pass name and score parameters
        SqliteParameter setName = new SqliteParameter("@Name", name);
        SqliteParameter setScore = new SqliteParameter("@Score", score);
        //Add to SQLite
        cmd.Parameters.Add(setName);
        cmd.Parameters.Add(setScore);
        //Execute
        cmd.ExecuteNonQuery();
        //Close DB
        dbConn.Close();
        //Show menu UI panel
        controller.EnableMenu();
    }

    //Get the top 5 scores and return
    public List<LeaderboardListing> GetTopFive()
    {
        //New list for leaderboard
        List<LeaderboardListing> list = new List<LeaderboardListing>();
        //Open the db
        dbConn.Open();
        IDbCommand cmd = dbConn.CreateCommand();
        //Select 5 records from highscore in order of descending score
        cmd.CommandText = "select * from highscore order by score desc limit 5;";
        IDataReader reader = cmd.ExecuteReader();
        //Read the results
        while (reader.Read())
        {
            //Get values
            float id_ = reader.GetFloat(0);
            string name_ = reader.GetString(1);
            float score_ = reader.GetFloat(2);
            //Add a new listing using these values
            LeaderboardListing newListing = new LeaderboardListing(name_, score_, (int)id_);
            //Add to list
            list.Add(newListing);
        }
        //Close DB
        dbConn.Close();
        //Dispose the reader
        reader.Dispose();
        //Return compiled list
        return list;
    }
}
