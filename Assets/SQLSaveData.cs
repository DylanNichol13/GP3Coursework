using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SQLSaveData : MonoBehaviour {
    //Path of the DB
    private string conn;
    //Connection to the SQLite DB
    private IDbConnection dbConn;
    //Player object
    private GameObject player;
    //Enemy Object
    private GameObject enemy;
    //Scripts held on objects
    //script to store player energy
    private PlayerEnergyScript playerEnergy;
    //Scripts to store enemy player properties
    private EnemyPlayerScript enemyScript;
    //UI components
    //The input field for saving data
    private GameObject inputField;
    //Text of the input field
    private Text inputText;
    // Use this for initialization
    void Start () {
        //Initialize script by getting components
        Init();
        //Initialize SQLite related components
        InitSQLite();
    }

    //Initialize SQLite for this script
    private void InitSQLite()
    {
        //Get path to DB
        conn = "URI=file:" + Application.dataPath + "/test.db";
        //Create tables if required
        CreateTables();
    }

    private void Init()
    {
        //Get player and enemy objects from scene
        player = GameObject.Find("Player");
        enemy = GameObject.Find("EnemyPlayer");
        //Get scripts which hold values for enemy and player
        enemyScript = enemy.GetComponent<EnemyPlayerScript>();
        playerEnergy = player.GetComponent<PlayerEnergyScript>();
        //Get the input field properties from scene
        inputField = GameObject.Find("SaveName_Input");
        inputText = inputField.transform.GetChild(1).GetComponent<Text>();
    }

    //Called on startup to create tables if they are missing
	private void CreateTables()
    {
        //Open the DB
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();
        //New command to hold queries
        IDbCommand cmd = dbConn.CreateCommand();
        //Create new playerData table
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS playerData (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(20), energy FLOAT, posX FLOAT, posY FLOAT, posZ FLOAT);";
        cmd.ExecuteNonQuery();
        //Create new enemyData table
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS enemyData (id INT, energy FLOAT, posX FLOAT, posY FLOAT, posZ FLOAT);";
        cmd.ExecuteNonQuery();
        //Create new mushroomData table
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS mushroomData (id INT, maxAge FLOAT, age FLOAT, posX FLOAT, posY FLOAT, posZ FLOAT, scaleX FLOAT, scaleY FLOAT, scaleZ FLOAT);";
        cmd.ExecuteNonQuery();
        //Create new blackhole data table
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS blackHoleData (id INT, posX F, posY FLOAT, posZ FLOAT);";
        cmd.ExecuteNonQuery();
        //Create new spawner data table
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS spawnerData (id INT, currentTimer FLOAT);";
        cmd.ExecuteNonQuery();
        //Close the db
        dbConn.Close();
    }

    //Saving data relating to player object
    public void SavePlayerData()
    {
        //Get the save file name from the input field
        string saveName = inputText.text;
        //Get the energy of player
        float energy = playerEnergy.GetEnergy();
        //Get the paused position of the player
        Vector3 position = player.transform.position;
        //Open the DB
        dbConn.Open();
        //Create new command for querying
        IDbCommand cmd = dbConn.CreateCommand();
        //Input values
        cmd.CommandText = "INSERT INTO playerData (name, energy, posX, posY, posZ) VALUES (@Name, @Energy, @PosX, @PosY, @PosZ);";
        //Pass in values through parameters
        cmd.Parameters.Add(new SqliteParameter("@Name", saveName));
        cmd.Parameters.Add(new SqliteParameter("@Energy", energy));
        cmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
        cmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
        cmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));
        //Execute
        cmd.ExecuteNonQuery();
        //Close DB
        dbConn.Close();
    }

    //Saving enemy data to SQLite
    public void SaveEnemyData()
    {
        //Open DB
        dbConn.Open();
        //new command to hold queries
        IDbCommand cmd = dbConn.CreateCommand();
        //Get the save ID Key for future reference
        int id = GetSaveID();
        //Get current enemy energy
        float energy = enemyScript.GetEnemyEnergy();
        //Get current enemy position
        Vector3 position = enemy.transform.position;    
        //Input values to SQLite table
        cmd.CommandText = "INSERT INTO enemyData (id, energy, posX, posY, posZ) VALUES (@Id, @Energy, @PosX, @PosY, @PosZ);";
        //Add the values through the declared parameters
        cmd.Parameters.Add(new SqliteParameter("@Id", id));
        cmd.Parameters.Add(new SqliteParameter("@Energy", energy));
        cmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
        cmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
        cmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));
        //Execute
        cmd.ExecuteNonQuery();
        //Close DB
        dbConn.Close();
    }

    //Saving data for mushrooms in the scene, requires a list of mushroom objects in scene
    public void SaveMushroomData(List<GameObject> mushroomList)
    {
        //Open the DB
        dbConn.Open();
        //Create new command for querying
        IDbCommand cmd = dbConn.CreateCommand();
        //get ID of the save file
        int id = GetSaveID();
        //will hold the max age of individual mushroom
        float maxAge;
        //Will hold the current age of a mushroom
        float age;
        //will hold the position of a mushroom
        Vector3 position;
        //will hold the scale of a mushroom object
        Vector3 scale;
        //The mushroom object which will be referred to
        MushroomScript.Mushroom mushroom;

        //Loop through each game object in the list of mushrooms passed through
        foreach (GameObject g in mushroomList) {
            //Insert the values into SQLite table
            cmd.CommandText = "INSERT INTO mushroomData(id, maxAge, age, posX, posY, posZ, scaleX, scaleY, scaleZ) VALUES (@Id, @MaxAge, @Age, @PosX, @PosY, @PosZ, @ScaleX, @ScaleY, @ScaleZ);";
            //set the mushroom object to the current mushroom attached to gameobject G
            mushroom = g.GetComponent<MushroomScript>().mushroom;
            //Get the max age of the current mushroom
            maxAge = mushroom.GetMaxAge();
            //Get current age of current mushroom
            age = mushroom.GetAge();
            //Get position of game object
            position = g.transform.position;
            //Get scale of current game object
            scale = g.transform.localScale;
            //Add these parameters before executing SQLite command
            cmd.Parameters.Add(new SqliteParameter("@Id", id));
            cmd.Parameters.Add(new SqliteParameter("@MaxAge", maxAge));
            cmd.Parameters.Add(new SqliteParameter("@Age", age));
            cmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
            cmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
            cmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));
            cmd.Parameters.Add(new SqliteParameter("@ScaleX", scale.x));
            cmd.Parameters.Add(new SqliteParameter("@ScaleY", scale.y));
            cmd.Parameters.Add(new SqliteParameter("@ScaleZ", scale.z));
            //execute for this mushroom
            cmd.ExecuteNonQuery();
        }
        //Close the DB
        dbConn.Close();
    }

    //Saving the blackhole data
    public void SaveBlackHoleData()
    {
        //Open the db
        dbConn.Open();
        //new command to hold query
        IDbCommand cmd = dbConn.CreateCommand();
        //get the current saving ID
        int id = GetSaveID();
        //Get the blackhole object in scene
        GameObject blackHole = GameObject.Find("blackHole");
        //Get position of the blackhole in scene
        Vector3 position = blackHole.transform.position;
        //Store values in SQL
        cmd.CommandText = "INSERT INTO blackHoleData (id, posX, posY, posZ) VALUES (@Id, @PosX, @PosY, @PosZ);";
        //Pass parameters relating to the black hole object
        cmd.Parameters.Add(new SqliteParameter("@Id", id));
        cmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
        cmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
        cmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));
        //Execute
        cmd.ExecuteNonQuery();
        //Close DB
        dbConn.Close();
    }

    //Save obstacle spawner data
    public void SaveObstacleSpawnData()
    {
        // get a reference of the obstacle spawner script from scene
        ObstacleController obstacleController = GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>();
        //Open the db
        dbConn.Open();
        //Create a command for holding queries
        IDbCommand cmd = dbConn.CreateCommand();
        //Get the current save ID
        int id = GetSaveID();
        //get the time on the respawner which will be saved
        float timer = obstacleController.GetRespawnTimer();
        //Add data to the SQLite table
        cmd.CommandText = "INSERT INTO spawnerData(id, currentTimer) VALUES (@Id, @CurrentTimer);";
        //Add parameters relating to spawner objet
        cmd.Parameters.Add(new SqliteParameter("@Id", id));
        cmd.Parameters.Add(new SqliteParameter("@CurrentTimer", timer));
        //execute
        cmd.ExecuteNonQuery();
        //close DB
        dbConn.Close();
    }

    //Return the INT Key associated with the current save
    private int GetSaveID()
    {
        //Default value set to -1, will show error if unchanged, meaning no game found
        int id = -1;
        //new command to hold query
        IDbCommand cmd = dbConn.CreateCommand();
        //Get the latest key added, the save data currently being processed
        cmd.CommandText = "SELECT * FROM playerData WHERE id = (SELECT MAX(id)  FROM playerData); ";
        //Execute
        cmd.ExecuteNonQuery();
        //Read the results of query
        IDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            //Get the ID key
            id = (int)reader.GetFloat(0);
        }
        //Dispose reader
        reader.Dispose();
        //return the ID
        return id;
    }
}
