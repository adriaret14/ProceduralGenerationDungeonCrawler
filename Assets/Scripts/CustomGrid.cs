using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomGrid : System.Object
{
    private Vector3 gridWorldPos;
    private int cellSize;
    private int gridSize;
    private GameObject[,] grid;

    [Header("Terrain Instances")]
    [SerializeField] private List<GameObject> instances=new List<GameObject>();
    
    [Header("Terrain Prefabs")]
    [SerializeField]private List<GameObject> terrainPrefabs;

    [Header("Random Walk Method")]
    [SerializeField] private int maxTunnelLength;
    [SerializeField] private int maxTunnels;
    [SerializeField] private List<Vector2Int> directions=new List<Vector2Int>();


    [Header("Debug Tools/Info")]
    [SerializeField] private Vector2Int startingPos;


    #region Constructor
    public CustomGrid(int gridSize, int cellSize, Vector3 worldPos)
    {
        this.gridSize = gridSize;
        this.grid = new GameObject[gridSize, gridSize];
        this.cellSize = cellSize;
        this.gridWorldPos = worldPos;
    }

    #endregion

    #region General Purpose Functions
    public void initializeGrid(GameObject initializer)
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = initializer;
            }
        }
    }
    public void printGrid()
    {
        string line = "";
        for (int i = 0; i < gridSize; i++)
        {
            line += "Row " + (i + 1) + ":  ";
            for (int j = 0; j < gridSize; j++)
            {
                //Debug.Log(grid[i, j]);
                line += grid[i, j] + ", ";
            }
            Debug.Log(line);
            line = "";
        }
    }

    public void setValue(Vector2Int pos, GameObject gInstance)
    {
        if (pos.x < gridSize && pos.y < gridSize)
        {
            grid[pos.x, pos.y] = gInstance;
            instances.Add(GameObject.Instantiate(gInstance, new Vector3(gridWorldPos.x + (pos.x * cellSize), gridWorldPos.y, gridWorldPos.z + (pos.y * cellSize)), Quaternion.identity));
        }
    }

    public GameObject getValue(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public void assignTerrains(List<GameObject> terrains)
    {
        terrainPrefabs = terrains;
    }
    #endregion

    #region Random Walk Method
    public void SetRandowWalkProperties(int maxTunnelLength, int maxTunnels, bool allowDiagonals)
    {
        this.maxTunnelLength = maxTunnelLength;
        this.maxTunnels = maxTunnels;
        setDirections(allowDiagonals);
        chooseStartingPoint();
    }
    private void chooseStartingPoint()
    {
        startingPos = new Vector2Int((int)Random.Range(0.0f, (float)gridSize-1.0f), (int)Random.Range(0.0f, (float)gridSize-1.0f));
    }
    private void setDirections(bool allowDiagonals)
    {
        if(allowDiagonals)
        {
            directions.Add(new Vector2Int(0, 1));
            directions.Add(new Vector2Int(0, -1));
            directions.Add(new Vector2Int(1, 0));
            directions.Add(new Vector2Int(-1, 0));
            directions.Add(new Vector2Int(1, 1));
            directions.Add(new Vector2Int(1, -1));
            directions.Add(new Vector2Int(-1, 1));
            directions.Add(new Vector2Int(-1, -1));

        }
        else
        {
            directions.Add(new Vector2Int(0, 1));
            directions.Add(new Vector2Int(0, -1));
            directions.Add(new Vector2Int(1, 0));
            directions.Add(new Vector2Int(-1, 0));
        }
    }

    public void createRandomWalkMap()
    {

        int cont = 0;
        int contStuck = 0;
        int currentTunnelCont = 0;
        int existentTerrainCont = 0;
        int randomTunnelLength = 0;
        int randomDirectionIndex = 0;
        Vector2Int lastPos = new Vector2Int();
        List<Vector2Int> lastDirections = new List<Vector2Int>();

        lastPos = startingPos;
        Debug.Log("Start pos: " + lastPos);

        while(cont < maxTunnels)
        {
            //Choose a random direction from the list
            if (lastDirections.Count <= 0)
            {
                randomDirectionIndex = Random.Range(0, (directions.Count - 1));
            }
            else
            {
                do
                {
                    randomDirectionIndex = Random.Range(0, (directions.Count - 1));
                } while (lastDirections[lastDirections.Count-1] == directions[randomDirectionIndex] || lastDirections[lastDirections.Count - 1] == new Vector2Int(-1, -1) * directions[randomDirectionIndex]);
            }
            lastDirections.Add(directions[randomDirectionIndex]);
            //Choose a random length using the max length for a tunnel
            randomTunnelLength = (int)Random.Range(1.0f, (float)maxTunnelLength);

            ////Create tunnel avoiding grid edges
            for (int i = 0; i < randomTunnelLength; i++)
            {
                if (lastPos.x + directions[randomDirectionIndex].x < 0 || lastPos.x + directions[randomDirectionIndex].x >= gridSize || lastPos.y + directions[randomDirectionIndex].y < 0 || lastPos.y + directions[randomDirectionIndex].y >= gridSize)
                {
                    Debug.Log("Sale del margen del grid");
                    existentTerrainCont++;
                }
                else if (grid[lastPos.x + directions[randomDirectionIndex].x, lastPos.y + directions[randomDirectionIndex].y] != null)
                {
                    Debug.Log("Ya hay otro terreno aqui");
                    existentTerrainCont++;
                }
                else
                {
                    //Create tunnel
                    this.setValue(lastPos + directions[randomDirectionIndex], terrainPrefabs[0]);
                    lastPos = lastPos + directions[randomDirectionIndex];
                    currentTunnelCont++;
                }
            }

            if(currentTunnelCont<=0 && contStuck<=3)
            {
                contStuck++;
            }
            else
            {
                do
                {
                    lastPos = new Vector2Int(Random.Range(0, gridSize - 1), Random.Range(0, gridSize - 1));
                } while (grid[lastPos.x, lastPos.y] == null);

                contStuck = 0;
            }

            //Increment cont
            cont += currentTunnelCont;
            Debug.Log("Random tunnel length: "+ randomTunnelLength + ", RandomDirectionIndex: " + randomDirectionIndex + ", Direccion tomada: " + lastDirections[lastDirections.Count - 1] + ", Contador: " + cont);
            currentTunnelCont = 0;
            existentTerrainCont = 0;
        }
    }
    #endregion

}
