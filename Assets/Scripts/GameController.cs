using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public GameObject[] uiObjects;

    Vector3 hidden = new Vector3(0, 0, 0);
    Vector3 shown = new Vector3(1, 1, 1);

    private ObstacleController obstacleController;
    private Movement movementScript;
    private StateController stateController;
    private EnemyPlayerScript enemy;
    private Text saveInputText;

    const string newGameBtnText = "Create New Game";

    // Use this for initialization
    void Start() {
        ResetUI();
        EnableMenu();
        obstacleController = GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>();
        movementScript = GameObject.Find("Player").GetComponent<Movement>();
        stateController = GetComponent<StateController>();
        enemy = GameObject.Find("EnemyPlayer").GetComponent<EnemyPlayerScript>();
        saveInputText = GameObject.Find("SaveName_Input").transform.GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {

    }

    void ResetUI()
    {
        foreach (GameObject g in uiObjects)
            g.transform.localScale = hidden;
    }

    public void EndGame()
    {
        ResetUI();
        uiObjects[0].transform.localScale = shown;
        stateController.DisableInstance();
    }

    public void EnableMenu()
    {
        ResetUI();
        uiObjects[1].transform.localScale = shown;
    }

    public void StartGame()
    {
        ResetUI();
        obstacleController.StartNewGame();
        enemy.StartGame();
        uiObjects[3].transform.localScale = shown;
        movementScript.StartGame();
        stateController.SetInstance();
        Camera.main.GetComponent<CameraController>().PlayerReset();
    }

    public void EnableGameSelect()
    {
        ResetUI();
        uiObjects[5].transform.localScale = shown;

        GetComponent<LoadGameScript>().PopulateGameList();
    }

    public void EnableScoreboard()
    {
        ResetUI();
        uiObjects[2].transform.localScale = shown;

        GetComponent<LeaderboardScript>().DisplayLeaderBoard();
    }

    public void ShowSaveInputField()
    {
        //Unhide the save input field
        ResetUI();

        uiObjects[4].transform.localScale = shown;
        stateController.DisableInstance();
        saveInputText.text = "";
    }

    public void ConfirmSave()
    {
        ResetUI();
        GetComponent<SQLSaveData>().SavePlayerData();
        GetComponent<SQLSaveData>().SaveMushroomData(GetMushroomList());
        GetComponent<SQLSaveData>().SaveEnemyData();
        GetComponent<SQLSaveData>().SaveBlackHoleData();
        GetComponent<SQLSaveData>().SaveObstacleSpawnData();

        uiObjects[3].transform.localScale = shown;
        stateController.SetInstance();
    }

    public void LoadGame()
    {
        string saveName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        StartGame();
        if (saveName == newGameBtnText)
        {
            
        }
        else
        {
            List<LoadGameScript.SavedGame> savedGameList = GetComponent<SQLLoadData>().GetSavedGames();
            foreach (LoadGameScript.SavedGame s in savedGameList)
            {
                if (s.gameName == saveName)
                {
                    GenerateGameData(s.id);
                }
            }
        }
    }

    private void GenerateGameData(int id)
    {
        print("jewjew");
        GetComponent<LoadGameScript>().GeneratePlayerSaveData(id);
        GetComponent<LoadGameScript>().GenerateEnemySaveData(id);
        GetComponent<LoadGameScript>().GenerateMushroomSaveData(id);
        GetComponent<LoadGameScript>().GenerateBlackHoleData(id);
        GetComponent<LoadGameScript>().GenerateSpawnerData(id);
    }

    private List<GameObject> GetMushroomList()
    {
        List<GameObject> list = new List<GameObject>();
        Transform mushroomContainer = GameObject.Find("Obstacle Container").transform;

        foreach(Transform t in mushroomContainer)
        {
            if (t.gameObject.tag != "blackHole")
                list.Add(t.gameObject);
        }

        return list;
    }
}
