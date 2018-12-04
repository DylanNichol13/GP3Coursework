using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SQLLoadData : MonoBehaviour {
    private string cString;
    string conn;
    IDbConnection dbConn;


    private void Start()
    {
        conn = "URI=file:" + Application.dataPath + "/test.db";
        dbConn = (IDbConnection)new SqliteConnection(conn);
    }

	public List<LoadGameScript.SavedGame> GetSavedGames()
    {
        dbConn.Open();

        IDbCommand cmd = dbConn.CreateCommand();

        List<LoadGameScript.SavedGame> savedGames = new List<LoadGameScript.SavedGame>();

        cmd.CommandText = "SELECT * FROM playerData";
        cmd.ExecuteNonQuery();
        IDataReader reader = cmd.ExecuteReader();
    
        while (reader.Read())
        {
            int gameID = reader.GetInt32(0);
            string gameName = reader.GetString(1);
            LoadGameScript.SavedGame newSavedGame = new LoadGameScript.SavedGame(gameName, gameID);
            savedGames.Add(newSavedGame);
        }
        dbConn.Close();
        return savedGames;
    }
}
