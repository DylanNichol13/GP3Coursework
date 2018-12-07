using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    /*Parent UI objects which can be shown or hidden to swap out UI phases
    Indexed as follows: [0] Score screen, [1] Main menu, [2] score board UI which shows top 5 scorers
    [3] Game UI which contains the save button, [4] contains 'save game name' input field, [5] scrolling 
    list which contains all of the saved games along with 'create new game' button*/
    public GameObject[] uiObjects;
    //Vector for hiding UI panels
    Vector3 hidden = new Vector3(0, 0, 0);
    //Vector for showing UI panels
    Vector3 shown = new Vector3(1, 1, 1);
    //Input field text
    private Text saveInputText;

    //Scripts to acquire
    //Script which controls the spawning of mushrooms and blackholes
    private ObstacleController obstacleController;
    //Script which conrols player movement
    private Movement movementScript;
    //State controller for game instances
    private StateController stateController;
    //Script for enemy properties
    private EnemyPlayerScript enemy;

    //Text of the 'create a new game' button in the menu UI
    const string newGameBtnText = "Create New Game";

    // Use this for initialization
    void Start() {
        //Initialize the script
        Init();
        //Hide all UI panels
        ChangeUI(1);
        //Enable the main menu
        EnableMenu();
    }

    //Called to initialize
    private void Init()
    {
        //Get components
        //obstacles script
        obstacleController = GameObject.Find("ObstacleCreator").GetComponent<ObstacleController>();
        //player movement script
        movementScript = GameObject.Find("Player").GetComponent<Movement>();
        //State controller script
        stateController = GetComponent<StateController>();
        //enemy controller script
        enemy = GameObject.Find("EnemyPlayer").GetComponent<EnemyPlayerScript>();

        //Input field text, which will be referenced when saving games to database
        saveInputText = GameObject.Find("SaveName_Input").transform.GetChild(1).GetComponent<Text>();
    }

    //Called when changing UI panels, hides all UI 
    void ChangeUI(int id)
    {
        //Go through each UI object
        for(int i = 0; i < uiObjects.Length; i++)
        {
            //If the incrementation has reached the ID passed in as argument, this is the UI panel to be displayed
            if (i == id)
            {
                //Set to display
                uiObjects[i].transform.localScale = shown;
            }
            //Otherwise, UI not desired,
            else
                //Set to hide
                uiObjects[i].transform.localScale = hidden;
        }
    }

    //Show the end game score panel
    public void EndGame()
    {
        //Enable [0] score screen and hide rest
        ChangeUI(0);
        //Pause the game
        stateController.DisableInstance();
    }

    //Show the main menu, where the player can choose to play or view scores
    public void EnableMenu()
    {
        //Enable [1] main menu and hide others
        ChangeUI(1);
    }

    //Called when starting a game
    public void StartGame()
    {
        //Show [3] the game UI and hide others
        ChangeUI(3);
        //Call the spawners belonging to the obstacle controller script
        obstacleController.StartNewGame();
        //Initialize the enemy object
        enemy.StartGame();
        //Initialize the player movement controller
        movementScript.StartGame();
        //Unpause the game through the state controller
        stateController.SetInstance();
        //Set player 'isAlive' on camera to true, to allow chase camera
        Camera.main.GetComponent<CameraController>().PlayerReset();
    }

    //Show the game select menu for loading or creating a new game
    public void EnableGameSelect()
    {
        //SHow saved games UI scrolling list and hide other UI panels
        ChangeUI(5);
        //Populate the game list with details on the saved data from the database
        GetComponent<LoadGameScript>().PopulateGameList();
    }

    //Show the scoreboard menu
    public void EnableScoreboard()
    {
        //Show the scoreboard UI and hide other panels
        ChangeUI(2);
        //Populate the leaderboard with top5 scorers from database
        GetComponent<LeaderboardScript>().DisplayLeaderBoard();
    }

    //Called to show the input field for saving a game 
    public void ShowSaveInputField()
    {
        //Unhide the save input field and hide others
        ChangeUI(4);
        //Pause the game
        stateController.DisableInstance();
        //Reset the text of the input field
        saveInputText.text = "";
    }

    //Called when confirming the save of the game
    public void ConfirmSave()
    {
        //Set the UI back to the gameplay UI
        ChangeUI(3);
        //Saving data in the SQLite databse for each saved component
        //Save player data
        GetComponent<SQLSaveData>().SavePlayerData();
        //Save mushroom data
        GetComponent<SQLSaveData>().SaveMushroomData(GetMushroomList());
        //Save enemy player data
        GetComponent<SQLSaveData>().SaveEnemyData();
        //Save data of black hole
        GetComponent<SQLSaveData>().SaveBlackHoleData();
        //Save the data of the obstacle spawn timer
        GetComponent<SQLSaveData>().SaveObstacleSpawnData();
        //Unpause the game
        stateController.SetInstance();
    }

    //Called after clicking a button in the game select UI, either for creating a new game or loading a game
    public void LoadGame()
    {
        //Get the text of hte button which was clicked, to determine the desired behaviour
        string saveName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        //Start the game, set the starting behaviour of game objects
        StartGame();
        //Detect if the button pressed is not the 'create new game' button, therefore requiring data loading from SQLite database
        if (saveName != newGameBtnText)
        {
            //Get a list of available game data from the sqlite saved game database
            List<LoadGameScript.SavedGame> savedGameList = GetComponent<SQLLoadData>().GetSavedGames();
            //Loop through each loaded game data detected
            foreach (LoadGameScript.SavedGame s in savedGameList)
            {
                //If the saved game name matches the saved game associated with the button pressed
                if (s.gameName == saveName)
                {
                    //Get the key of the save data and generate game data based on this key
                    GenerateGameData(s.id);
                }
            }
        }
    }

    //Generate game data based on the given SQLite db key
    private void GenerateGameData(int id)
    {
        //Get saved data for the player based on key
        GetComponent<LoadGameScript>().GeneratePlayerSaveData(id);
        //Get saved data for enemy based on key
        GetComponent<LoadGameScript>().GenerateEnemySaveData(id);
        //Get saved data for mushroom based on key
        GetComponent<LoadGameScript>().GenerateMushroomSaveData(id);
        //Get blackhole data based on key
        GetComponent<LoadGameScript>().GenerateBlackHoleData(id);
        //Get spawner data based on key
        GetComponent<LoadGameScript>().GenerateSpawnerData(id);
    }

    //Get a list of mushroom objects to save
    private List<GameObject> GetMushroomList()
    {
        //Create a new list of gameobjects to hold the mushroom isntances
        List<GameObject> list = new List<GameObject>();
        //Get the container of the mushrooms and obstacles
        Transform mushroomContainer = GameObject.Find("Obstacle Container").transform;
        //Foreach child object of the container
        foreach(Transform t in mushroomContainer)
        {
            //Detect if this object is the black hole, as it should not be stored with mushrooms
            if (t.gameObject.tag != "blackHole")
                //If not, add this mushroom to the list
                list.Add(t.gameObject);
        }
        //Return completed list
        return list;
    }
}
