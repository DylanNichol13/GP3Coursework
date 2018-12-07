using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SQLSaveData : MonoBehaviour {
    private string cString;
    string conn;
    IDbConnection dbConn;

    private GameObject player;
    private GameObject enemy;
    private PlayerEnergyScript playerEnergy;
    private EnemyPlayerScript enemyScript;
    private GameObject inputField;
    private Text inputText;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        enemy = GameObject.Find("EnemyPlayer");
        enemyScript = enemy.GetComponent<EnemyPlayerScript>();
        playerEnergy = player.GetComponent<PlayerEnergyScript>();
        inputField = GameObject.Find("SaveName_Input");
        inputText = inputField.transform.GetChild(1).GetComponent<Text>();

        conn = "URI=file:" + Application.dataPath + "/test.db";
        CreateTables();

        testies();
    }

    private void testies()
    {
        int lastID;

        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "SELECT * FROM highscore WHERE id = (SELECT MAX(id)  FROM highscore); ";
        cmd.ExecuteNonQuery();

        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            lastID = (int)reader.GetFloat(0);
            print(lastID + " HERE");
        }
        dbConn.Close();
        reader.Dispose();
    }


	private void CreateTables()
    {
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "CREATE TABLE IF NOT EXISTS playerData (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(20), energy FLOAT, posX FLOAT, posY FLOAT, posZ FLOAT);";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS enemyData (id INT, energy FLOAT, posX FLOAT, posY FLOAT, posZ FLOAT);";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS mushroomData (id INT, maxAge FLOAT, age FLOAT, posX FLOAT, posY FLOAT, posZ FLOAT, scaleX FLOAT, scaleY FLOAT, scaleZ FLOAT);";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS blackHoleData (id INT, posX F, posY FLOAT, posZ FLOAT);";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS spawnerData (id INT, currentTimer FLOAT);";
        cmd.ExecuteNonQuery();

        dbConn.Close();
    }

    public void SavePlayerData()
    {
        string saveName = inputText.text;
        float energy = playerEnergy.GetEnergy();
        Vector3 position = player.transform.position;

        dbConn.Open();
        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "INSERT INTO playerData (name, energy, posX, posY, posZ) VALUES (@Name, @Energy, @PosX, @PosY, @PosZ);";

        cmd.Parameters.Add(new SqliteParameter("@Name", saveName));
        cmd.Parameters.Add(new SqliteParameter("@Energy", energy));
        cmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
        cmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
        cmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));

        cmd.ExecuteNonQuery();

        dbConn.Close();
    }

    public void SaveEnemyData()
    {
        dbConn.Open();
        IDbCommand cmd = dbConn.CreateCommand();

        int id = GetSaveID();
        float energy = enemyScript.GetEnemyEnergy();
        Vector3 position = enemy.transform.position;    

        cmd.CommandText = "INSERT INTO enemyData (id, energy, posX, posY, posZ) VALUES (@Id, @Energy, @PosX, @PosY, @PosZ);";

        cmd.Parameters.Add(new SqliteParameter("@Id", id));
        cmd.Parameters.Add(new SqliteParameter("@Energy", energy));
        cmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
        cmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
        cmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));

        cmd.ExecuteNonQuery();

        dbConn.Close();
    }

    public void SaveMushroomData(List<GameObject> mushroomList)
    {
        dbConn.Open();
        IDbCommand cmd = dbConn.CreateCommand();

        int id = GetSaveID();
        float maxAge;
        float age;
        Vector3 position;
        Vector3 scale;
        MushroomScript.Mushroom mushroom;

        foreach (GameObject g in mushroomList) {
            cmd.CommandText = "INSERT INTO mushroomData(id, maxAge, age, posX, posY, posZ, scaleX, scaleY, scaleZ) VALUES (@Id, @MaxAge, @Age, @PosX, @PosY, @PosZ, @ScaleX, @ScaleY, @ScaleZ);";

            mushroom = g.GetComponent<MushroomScript>().mushroom;
           
            maxAge = mushroom.GetMaxAge();
            age = mushroom.GetAge();
            position = g.transform.position;
            scale = g.transform.localScale;

            cmd.Parameters.Add(new SqliteParameter("@Id", id));
            cmd.Parameters.Add(new SqliteParameter("@MaxAge", maxAge));
            cmd.Parameters.Add(new SqliteParameter("@Age", age));
            cmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
            cmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
            cmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));
            cmd.Parameters.Add(new SqliteParameter("@ScaleX", scale.x));
            cmd.Parameters.Add(new SqliteParameter("@ScaleY", scale.y));
            cmd.Parameters.Add(new SqliteParameter("@ScaleZ", scale.z));

            cmd.ExecuteNonQuery();
        }
        dbConn.Close();
    }

    public void SaveBlackHoleData()
    {
        dbConn.Open();
        IDbCommand cmd = dbConn.CreateCommand();

        int id = GetSaveID();
        GameObject blackHole = GameObject.Find("blackHole");
        Vector3 position = blackHole.transform.position;

        cmd.CommandText = "INSERT INTO blackHoleData (id, posX, posY, posZ) VALUES (@Id, @PosX, @PosY, @PosZ);";

        cmd.Parameters.Add(new SqliteParameter("@Id", id));
        cmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
        cmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
        cmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));

        cmd.ExecuteNonQuery();

        dbConn.Close();
    }

    public void SaveObstacleSpawnData()
    {
        ObstacleController obstacleController = GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>();

        dbConn.Open();
        IDbCommand cmd = dbConn.CreateCommand();

        int id = GetSaveID();
        float timer = obstacleController.GetRespawnTimer();

        cmd.CommandText = "INSERT INTO spawnerData(id, currentTimer) VALUES (@Id, @CurrentTimer);";

        cmd.Parameters.Add(new SqliteParameter("@Id", id));
        cmd.Parameters.Add(new SqliteParameter("@CurrentTimer", timer));


        cmd.ExecuteNonQuery();

        dbConn.Close();
    }

    private int GetSaveID()
    {
        int id = -1;

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "SELECT * FROM playerData WHERE id = (SELECT MAX(id)  FROM playerData); ";
        cmd.ExecuteNonQuery();

        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            id = (int)reader.GetFloat(0);
        }
        reader.Dispose();

        return id;
    }
}
