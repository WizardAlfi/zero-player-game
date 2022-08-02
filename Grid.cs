using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour, IDataPersistence
{

    public int length;

    public GameObject cellPrefab;

    public bool gameIsPlaying = false;

    public float delay;

    public bool[] aliveCells;


    private DataPersistenceManager dataPersistenceManager;


    [SerializeField] Camera mainCamera;


    [Header("Lists")]
    public List<GameObject> cells = new List<GameObject>();
    public List<Vector2Int> cellPositions = new List<Vector2Int>();


    void Start()
    {
        dataPersistenceManager = FindObjectOfType<DataPersistenceManager>();

        aliveCells = new bool[length * length * dataPersistenceManager.amountOfSaveSlots];
        

        CreateCells();
        CreateNeighbors();

        mainCamera.orthographicSize = length / 2;
        //Camera
        mainCamera.transform.position = new Vector3(length / 2, length / 2, -0.5f);

    }



    public void LoadData(GameData data)
    {
        this.aliveCells = data.aliveCells;
        
        ReloadCells();
    }

    public void SavedData(GameData data)
    {
        data.aliveCells = this.aliveCells;
     
    }






    

    private void CreateCells()
    {
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                GameObject newCell = Instantiate(cellPrefab);

                newCell.transform.position = new Vector3(x, y);
                cells.Add(newCell);
                cellPositions.Add(new Vector2Int(x, y));
                newCell.GetComponent<Cellscript>().cellID = cells.IndexOf(newCell);
            }

        }
    }

    void CreateNeighbors()
    {
        foreach(GameObject cell in cells)
        {
            Cellscript cellScript = cell.GetComponent<Cellscript>();
            Vector2Int cellPos = cellPositions[cells.IndexOf(cell)];
            

            for (int x = cellPos.x - 1; x < cellPos.x + 2; x++)
            {
                for(int y = cellPos.y - 1; y < cellPos.y + 2; y++)
                {
                    Vector2Int neighborPos = new Vector2Int(x, y);

                    if (neighborPos != cellPos)
                    {
                        if (cellPositions.Contains(neighborPos) == true)
                        {
                            GameObject neighbor = cells[cellPositions.IndexOf(neighborPos)];
                            cellScript.myNeighbors.Add(neighbor);

                        }

                    }
                }
            }
        }
    }

    void ReloadCells()
    {
        
        foreach(GameObject cell in cells)
        {
            Cellscript cellScript = cell.GetComponent<Cellscript>();
            int saveSlotAdjustement = dataPersistenceManager.currentSaveSlot * length * length;
            cellScript.IsAlive(aliveCells[cellScript.cellID + saveSlotAdjustement]);
        }
    }


   

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameIsPlaying = false;
            foreach(GameObject cell in cells)
            {
                cell.GetComponent<Cellscript>().IsAlive(false);
            }
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            gameIsPlaying = false;
            MakePerlinNoise();
        }


        if (Input.GetKeyDown(KeyCode.Space) && !gameIsPlaying)
        {
            gameIsPlaying = true;
            StartCoroutine(StartGame());
        }

        else if (Input.GetKeyDown(KeyCode.Space) && gameIsPlaying)
        {
            gameIsPlaying = false;
        }
    }


    void MakePerlinNoise()
    {
        for(int x = 0; x < length; x ++)
        {
            for (int y = 0; y < length; y++)
            {


                float noiseValue = Mathf.PerlinNoise((float)x, (float)y);

                //noiseValue = Mathf.Floor(noiseValue);
                Debug.Log(noiseValue);
                
            }
        }
    }



    IEnumerator StartGame()
    {

        while(gameIsPlaying)
        {
            foreach (GameObject cell in cells)
            {
                cell.GetComponent<Cellscript>().NewGeneration();
            }

            foreach (GameObject cell in cells)
            {
                cell.GetComponent<Cellscript>().EndGeneration();

            }

            yield return new WaitForSeconds(delay);

        }
       
        
    }


   

}
