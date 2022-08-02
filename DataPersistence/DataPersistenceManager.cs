using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{

    [Header("File storage")]
    [SerializeField] private string fileName; 


    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    [Header("Save slot info")]
    public int amountOfSaveSlots;
    public int currentSaveSlot;

    private int inputKey;

    public static DataPersistenceManager instance { get; private set; }


    


    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one Data Persitance Manager Found");
        }
        instance = this;
    }

    private void Start()
    {
        currentSaveSlot = 0;
        amountOfSaveSlots = 5;

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        NewGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load any saved data from a file using the data handler 
        this.gameData = dataHandler.Load();


         // if no data can be loaded, initialize to a new game
        if(this.gameData == null)
        {
            Debug.Log("No data was found. Starting New Game");
            NewGame();
        }


        // push the loaded data to all other scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }


    }
    public void SaveGame()
    {
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SavedData(gameData);
        }

        // save that data to a file using the data handler
        dataHandler.Save(gameData);
        Debug.Log(dataHandler.dataDirPath);
    }




    private void Update()
    {
        #region Input
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        if(Input.anyKeyDown)
        {

            if(CheckIfInputIsInt(Input.inputString) && int.Parse(inputKey.ToString()) <= amountOfSaveSlots && inputKey > 0) 
            {
                currentSaveSlot = inputKey - 1;
                LoadGame();
            }
        }

        #endregion

    }

    bool CheckIfInputIsInt(string input)
    {
        return int.TryParse(input, out inputKey);
    }



    //private void OnApplicationQuit()
    //{
    //    SaveGame();
    //}



    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
