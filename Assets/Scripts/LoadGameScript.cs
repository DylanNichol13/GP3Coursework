using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class LoadGameScript : MonoBehaviour {
    public class SavedGame
    {
        public string gameName;
        public int id;
        public float energy;

        public SavedGame(string name, int ID, float Energy)
        {
            gameName = name;
            id = ID;
            energy = Energy;
        }
    }

    private GameObject loadGameBtn;
    private string cString;
    string conn;
    IDbConnection dbConn;

    // Use this for initialization
    void Start () {
        loadGameBtn = GameObject.Find("GameSelect_Container").transform.GetChild(0).gameObject;
        conn = "URI=file:" + Application.dataPath + "/test.db";
        dbConn = (IDbConnection)new SqliteConnection(conn);
    }

    public void PopulateGameList()
    {
        List<SavedGame> savedGameList = new List<SavedGame>();
        // Get the list from load game script
        savedGameList = GetComponent<SQLLoadData>().GetSavedGames();

        foreach (SavedGame s in savedGameList) {
            GameObject loadListing = GameObject.Instantiate(loadGameBtn);
            loadListing.transform.parent = loadGameBtn.transform.parent;
            loadListing.transform.GetChild(0).GetComponent<Text>().text = s.gameName;
            loadListing.transform.GetChild(1).GetComponent<Text>().text = s.energy + " Pts";
        }
    }

    public void GeneratePlayerSaveData(int id)
    {
        GameObject player = GameObject.Find("Player");
        PlayerEnergyScript playerEnergy = player.GetComponent<PlayerEnergyScript>();

        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "SELECT * FROM playerData WHERE id = @ID;";
        cmd.Parameters.Add(new SqliteParameter("@ID", id));

        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            playerEnergy.SetPlayerEnergy(reader.GetFloat(2));
            Vector3 position = new Vector3(reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5));
            player.transform.position = position;
        }

        dbConn.Close();
    }

    public void GenerateEnemySaveData(int id)
    {
        GameObject enemy = GameObject.Find("EnemyPlayer");
        EnemyPlayerScript enemyScript = enemy.GetComponent<EnemyPlayerScript>();

        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "SELECT * FROM enemyData WHERE id = @ID;";
        cmd.Parameters.Add(new SqliteParameter("@ID", id));

        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Vector3 position = new Vector3(reader.GetFloat(2), reader.GetFloat(3), reader.GetFloat(4));
            enemy.transform.position = position;
        }

        dbConn.Close();
    }
	
	public void GenerateMushroomSaveData(int id)
    {
        List<MushroomScript.Mushroom> loadedMushrooms = new List<MushroomScript.Mushroom>();

        dbConn.Open();
        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "SELECT * FROM mushroomData WHERE id = @ID";
        cmd.Parameters.Add(new SqliteParameter("@ID", id));

        IDataReader reader = cmd.ExecuteReader();

        MushroomScript.Mushroom newMushroom;
        while (reader.Read())
        {
            float deathAge = reader.GetFloat(1);
            float age = reader.GetFloat(2);
            Vector3 pos = new Vector3(reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5));
            Vector3 scale = new Vector3(reader.GetFloat(6), reader.GetFloat(7), reader.GetFloat(8));

            newMushroom = new MushroomScript.Mushroom(age, deathAge);
            newMushroom.mushroomScale = scale;
            newMushroom.mushroomPos = pos;

            loadedMushrooms.Add(newMushroom);
        }
        dbConn.Close();
        GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>().CreateObstacles(loadedMushrooms);
    }

    public void GenerateBlackHoleData(int id)
    {
        GameObject blackHole = GameObject.Find("blackHole");

        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "SELECT * FROM blackHoleData WHERE id = @ID;";
        cmd.Parameters.Add(new SqliteParameter("@ID", id));

        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Vector3 position = new Vector3(reader.GetFloat(1), reader.GetFloat(2), reader.GetFloat(3));
            blackHole.transform.position = position;
        }

        dbConn.Close();
    }

    public void GenerateSpawnerData(int id)
    {
        ObstacleController obstacleController = GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>();
        float timer = obstacleController.GetRespawnTimer();

        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "SELECT * FROM spawnerData WHERE id = @ID;";
        cmd.Parameters.Add(new SqliteParameter("@ID", id));

        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            obstacleController.SetRespawnTimer(reader.GetFloat(1));
        }

        dbConn.Close();
    }
}
