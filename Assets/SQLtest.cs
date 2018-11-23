using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SQLtest : MonoBehaviour
{

    private string cString;
    string conn;
    IDbConnection dbConn;

    private GameController controller;

    // Use this for initialization
    void Start()
    {
        conn = "URI=file:" + Application.dataPath + "/test.db";

        CreateTable();

        controller = GameObject.Find("Controller").GetComponent<GameController>();
    }

    private void CreateTable()
    {
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "DROP TABLE IF EXISTS highscore;";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "CREATE TABLE IF NOT EXISTS highscore (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(20), score INT);";
        cmd.ExecuteNonQuery();

        dbConn.Close();
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

        cmd.CommandText = "insert into highscore (name, score) VALUES ('" + name + "', " + score + ");";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "select * from highscore order by score desc;";

        IDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            float id_ = reader.GetFloat(0);
            string name_ = reader.GetString(1);
            float score_ = reader.GetFloat(2);
            print(id_ + " " + name_ + " " + score_);
        }
        //Close DB
        dbConn.Close();
        reader.Dispose();
        controller.EnableMenu();
    }


    public List<LeaderboardListing> GetTopFive()
    {
        List<LeaderboardListing> list = new List<LeaderboardListing>();

        dbConn.Open();
        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "select * from highscore order by score desc limit 5;";
        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            float id_ = reader.GetFloat(0);
            string name_ = reader.GetString(1);
            float score_ = reader.GetFloat(2);

            LeaderboardListing newListing = new LeaderboardListing(name_, score_, (int)id_);
            list.Add(newListing);
        }

        dbConn.Close();

        reader.Dispose();
        return list;
    }
}
