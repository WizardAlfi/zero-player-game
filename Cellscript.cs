using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cellscript : MonoBehaviour
{
    
    public bool isAlive;
    public int aliveNeighbors;

    public int cellID;

    public SpriteRenderer cellRenderer;

    public Grid grid;
    private DataPersistenceManager dataPersistenceManager; 



    public List<GameObject> myNeighbors = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<Grid>();
        dataPersistenceManager = FindObjectOfType<DataPersistenceManager>();

        cellRenderer = gameObject.GetComponent<SpriteRenderer>();

        IsAlive(false);
       
        
    }

    


    public void NewGeneration()
    {
        aliveNeighbors = 0;

        foreach(GameObject neighbor in myNeighbors)
        {
            if(neighbor.GetComponent<Cellscript>().isAlive)
            {
                aliveNeighbors += 1;
            }
        }

        

    }

    public void EndGeneration()
    {

        if(isAlive && aliveNeighbors == 2)
        {
            IsAlive(true);
        }
        else if(isAlive && aliveNeighbors == 3)
        {
            IsAlive(true);
        }
        else if(!isAlive && aliveNeighbors == 3)
        {
            IsAlive(true);
        }
        else
        {
            IsAlive(false);
        }


    }

    


    private void OnMouseOver()
    {
        if(!grid.gameIsPlaying)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                IsAlive(true);
            }

            else if (Input.GetKey(KeyCode.Mouse1))
            {
                IsAlive(false);
            }
        }
    }





    public void IsAlive(bool alive)
    {
        isAlive = alive;
        cellRenderer.enabled = alive;


        int saveSlotAdjustement = dataPersistenceManager.currentSaveSlot * grid.length * grid.length;
        grid.aliveCells[cellID + saveSlotAdjustement] = alive;
    }
}
