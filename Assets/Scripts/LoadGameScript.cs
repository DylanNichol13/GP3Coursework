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

        public SavedGame(string name, int ID)
        {
            gameName = name;
            id = ID;
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
	
	
}
