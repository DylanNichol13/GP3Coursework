using UnityEngine;
using System.Collections;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SQLtest : MonoBehaviour {

    private string cString;

	// Use this for initialization
	void Start () {
        cString = "URI=file:" + Application.dataPath + "/DB.sqlite";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void GetScore()
    {
        using(IDbConnection dbC = new SqliteConnection(cString))
        {
            dbC.Open();
            using(IDbCommand dbCmd = dbC.CreateCommand())
            {
                string sqlQuery = 
            }
        }
    }
}
