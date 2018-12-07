using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class LoadGameScript : MonoBehaviour {
    //Class for storing game data, to be displayed when viewing games stored in the SQLite saved game database
    public class SavedGame
    {
        //Name attached to the saved game
        public string gameName;
        //Auto incrememented main key associated with the game
        public int id;
        //Current player energy amount
        public float energy;
        //Constructing a new saved game, requires the name, ID and energy of player, which is taken from the SQLite db
        public SavedGame(string name, int ID, float Energy)
        {
            //Set properties to values passed in
            gameName = name;
            id = ID;
            energy = Energy;
        }
    }

    //Base object for populating the saved game list when viewing games available for loading from SQLite db
    private GameObject loadGameBtn;
    //Path the to SQLite database
    private string conn;
    //Connection to the SQLite db using the 'conn' path declared above
    private IDbConnection dbConn;

    // Use this for initialization
    void Start () {
        //Get the required components for the script
        Init();
        //Initialize properties relating to SQLite
        InitSQLite();
    }

    //Used to initialize script, fetching properties from the scene
    private void Init()
    {
        //Get the base button for loading games from menu
        loadGameBtn = GameObject.Find("GameSelect_Container").transform.GetChild(0).gameObject;
    }

    //Initalize values relating to SQLite DB
    private void InitSQLite()
    {
        //Path to the DB
        conn = "URI=file:" + Application.dataPath + "/test.db";
        //Set up connection to the DB using SQlite
        dbConn = (IDbConnection)new SqliteConnection(conn);
    }

    //Used to populate the available games list in load game menu
    public void PopulateGameList()
    {
        //Set a new list of saved game data
        List<SavedGame> savedGameList = new List<SavedGame>();
        // Get the list from load game script
        savedGameList = GetComponent<SQLLoadData>().GetSavedGames();
        //For each saved game loaded from SQLite databaase
        foreach (SavedGame s in savedGameList) {
            //Create a new button object by duplicating original
            GameObject loadListing = GameObject.Instantiate(loadGameBtn);
            //Set the parent, to display in relevant UI panel
            loadListing.transform.parent = loadGameBtn.transform.parent;
            //Display the name associated with this game
            loadListing.transform.GetChild(0).GetComponent<Text>().text = s.gameName;
            //Display the current amount of energy/points belonging to player in this game
            loadListing.transform.GetChild(1).GetComponent<Text>().text = s.energy + " Pts";
        }
    }

    //Generating Game data from the SQLite DB relating to the player object, uses game KEY
    public void GeneratePlayerSaveData(int id)
    {
        //Get the player object, only relevant to this function
        GameObject player = GameObject.Find("Player");
        //Get thhe energy containing script of the player
        PlayerEnergyScript playerEnergy = player.GetComponent<PlayerEnergyScript>();
        //Open the connection to the DB
        dbConn.Open();
        //Create a new command to access the SQLite db
        IDbCommand cmd = dbConn.CreateCommand();
        //Fetch the player data from the playerData Table with the correct game KEY
        cmd.CommandText = "SELECT * FROM playerData WHERE id = @ID;";
        //Add the KEY parameter
        cmd.Parameters.Add(new SqliteParameter("@ID", id));
        //Create a reader to parse SQLite data
        IDataReader reader = cmd.ExecuteReader();
        //Read the fetched data
        while (reader.Read())
        {
            //Get the player energy, held in row 3 and set player energy as this value
            playerEnergy.SetPlayerEnergy(reader.GetFloat(2));
            //Get the 3 position properties for X Y and Z, get in row 4,5,6 and store this data in a new v3 Position
            Vector3 position = new Vector3(reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5));
            //Set the current player position to the newly created v3 Position
            player.transform.position = position;
        }
        //Close the connection of the SQLite DB
        dbConn.Close();
    }

    //Generate game data for the enemy based on the passed id KEY
    public void GenerateEnemySaveData(int id)
    {
        //Get the enemy object to be manipulated
        GameObject enemy = GameObject.Find("EnemyPlayer");
        //Get the enemy controller script
        EnemyPlayerScript enemyScript = enemy.GetComponent<EnemyPlayerScript>();
        //Open a connection to SQLite db
        dbConn.Open();
        //Create a new cmd to reach SQlite 
        IDbCommand cmd = dbConn.CreateCommand();
        //Select the relevent data from enemyData table which contains the desired game ID
        cmd.CommandText = "SELECT * FROM enemyData WHERE id = @ID;";
        //Add the ID parater to the SQLite query
        cmd.Parameters.Add(new SqliteParameter("@ID", id));
        //Create a new reader to parse data
        IDataReader reader = cmd.ExecuteReader();
        //Parse the data
        while (reader.Read())
        {
            //Create a new v3 position to hold X Y & Z properties
            Vector3 position = new Vector3(reader.GetFloat(2), reader.GetFloat(3), reader.GetFloat(4));
            //Set the current position of enemy object to the above v3 Position
            enemy.transform.position = position;
        }
        //Close connection
        dbConn.Close();
    }
	
    //Generate data for mushroom objects based on the passed ID KEY
	public void GenerateMushroomSaveData(int id)
    {
        //Create a new list of mushroom instances
        List<MushroomScript.Mushroom> loadedMushrooms = new List<MushroomScript.Mushroom>();
        //Open the SQLite DB
        dbConn.Open();
        //Create a new command to hold queries
        IDbCommand cmd = dbConn.CreateCommand();
        //Select all mushroom data which contains the relevent key
        cmd.CommandText = "SELECT * FROM mushroomData WHERE id = @ID";
        //Add the key parameter to query
        cmd.Parameters.Add(new SqliteParameter("@ID", id));
        //Create reader to parse data
        IDataReader reader = cmd.ExecuteReader();
        //New mushroom instance which will store data to be added to the list
        MushroomScript.Mushroom newMushroom;
        //Read the acquired data
        while (reader.Read())
        {
            //Holds death age of mushroom with data from SQLite table
            float deathAge = reader.GetFloat(1);
            //Holds current age of mushroom with data from SQLite table
            float age = reader.GetFloat(2);
            //Holds position of mushroom with data from SQLite table
            Vector3 pos = new Vector3(reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5));
            //Holds scale of mushroom with data from SQLite table
            Vector3 scale = new Vector3(reader.GetFloat(6), reader.GetFloat(7), reader.GetFloat(8));
            //Create a new mushroom instance using the constructor, passing age and age of death
            newMushroom = new MushroomScript.Mushroom(age, deathAge);
            //Set the mushroom scale based on SQLite values
            newMushroom.mushroomScale = scale;
            //Set mushroom position based on sqlite values
            newMushroom.mushroomPos = pos;
            //Add mushroom to the lsit
            loadedMushrooms.Add(newMushroom);
        }
        //Close the db
        dbConn.Close();
        //Pass this data to the obstacle creator to create game objects which contain each loaded mushroom
        GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>().CreateObstacles(loadedMushrooms);
    }

    //Generating game data from the black hole saved data using the KEY
    public void GenerateBlackHoleData(int id)
    {
        //Get the blackhole object in game scene
        GameObject blackHole = GameObject.Find("blackHole");
        //Open the SQLite db
        dbConn.Open();
        //Create new command to hold queries
        IDbCommand cmd = dbConn.CreateCommand();
        //Get data from blackhole table which contains relevent ID KEY
        cmd.CommandText = "SELECT * FROM blackHoleData WHERE id = @ID;";
        //Add the ID key parameter to the query
        cmd.Parameters.Add(new SqliteParameter("@ID", id));
        //Create a reader for reader SQL results data
        IDataReader reader = cmd.ExecuteReader();
        //Read the results
        while (reader.Read())
        {
            //Get the 3 position vector values from the SQLite data, store these in a new v3
            Vector3 position = new Vector3(reader.GetFloat(1), reader.GetFloat(2), reader.GetFloat(3));
            //Set the blackhole current position to the store data, held in the new v3
            blackHole.transform.position = position;
        }
        //Close the db
        dbConn.Close();
    }

    //Get game data based on the SQLite saved spawner data, using the given key
    public void GenerateSpawnerData(int id)
    {
        //Get the obstacle controller script from the game scene
        ObstacleController obstacleController = GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>();
        //open the DB
        dbConn.Open();
        //Create new command to hold SQLite queries
        IDbCommand cmd = dbConn.CreateCommand();
        //Select the spawner data with the relevent ID key
        cmd.CommandText = "SELECT * FROM spawnerData WHERE id = @ID;";
        //Pass in Key parameter
        cmd.Parameters.Add(new SqliteParameter("@ID", id));
        //New reader for reading results
        IDataReader reader = cmd.ExecuteReader();
        //Read the results
        while (reader.Read())
        {
            //Set the current time on the mushroom respawner to be that of the float value held in the SQLite spawnerData table
            obstacleController.SetRespawnTimer(reader.GetFloat(1));
        }
        //Close the database
        dbConn.Close();
    }
}
