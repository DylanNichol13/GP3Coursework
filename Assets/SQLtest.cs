using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SQLtest : MonoBehaviour {

    private string cString;
    string conn;
    IDbConnection dbConn;
    // Use this for initialization
    void Start()
    {
        conn = "URI=file:" + Application.dataPath + "/test.db";

        CreateTable();
    }

    private void CreateTable()
    {
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "DROP TABLE IF EXISTS highscore;";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "CREATE TABLE highscore (name VARCHAR(20), score INT);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "insert into highscore (name, score) VALUES ('baws', 69);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "select * from highscore order by score desc;";

        IDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string name = reader.GetString(0);
            print(name);
        }

        dbConn.Close();
    }

    public void AddToDatabase()
    {
        string name = GameObject.Find("InputField").transform.GetChild(2).gameObject.GetComponent<Text>().text;
        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        cmd.CommandText = "insert into highscore (name, score) VALUES ('" + name + "', 40);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "select * from highscore order by score desc;";

        IDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string name_ = reader.GetString(0);
            print(name_);
        }

        dbConn.Close();

    }
}
