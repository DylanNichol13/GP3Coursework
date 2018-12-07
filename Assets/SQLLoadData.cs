using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SQLLoadData : MonoBehaviour {
    //Path to the DB
    private string conn;
    //Connection to the SQLite db
    private IDbConnection dbConn;

    //Called on startup
    private void Start()
    {
        //Initialize values relating to SQlite
        InitSQLite();
    }

    //Called on start
    private void InitSQLite()
    {
        //Set the path to the DB
        conn = "URI=file:" + Application.dataPath + "/test.db";
        //Create a connection to the db
        dbConn = (IDbConnection)new SqliteConnection(conn);
    }

    //Get the saved games stored in the DB
	public List<LoadGameScript.SavedGame> GetSavedGames()
    {
        //open the database
        dbConn.Open();
        //Create a command to hold queries
        IDbCommand cmd = dbConn.CreateCommand();
        //Set a new list of saved games
        List<LoadGameScript.SavedGame> savedGames = new List<LoadGameScript.SavedGame>();
        //Get all data from the playerData table, as these represent individual saved games
        cmd.CommandText = "SELECT * FROM playerData";
        cmd.ExecuteNonQuery();

        //Reader to read the results
        IDataReader reader = cmd.ExecuteReader();
        //Reading the results
        while (reader.Read())
        {
            //Get the ID Key, which is required to pull data from each area of the saved game and generate game data
            int gameID = reader.GetInt32(0);
            //Get the game name, which will be dispalyed in the load game selection menu
            string gameName = reader.GetString(1);
            //Get the current energy, also stored in the load game selection menu
            float energy = reader.GetFloat(2);
            //Create a new saved game data store which holds the properties retrieved from the SQLite database
            LoadGameScript.SavedGame newSavedGame = new LoadGameScript.SavedGame(gameName, gameID, energy);
            //Add this to the list
            savedGames.Add(newSavedGame);
        }
        //Close the DB
        dbConn.Close();
        //Return list of saved games
        return savedGames;
    }
}
