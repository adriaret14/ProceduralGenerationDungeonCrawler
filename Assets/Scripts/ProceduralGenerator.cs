using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    [Header("Common Ecosystem Prefabs")]
    [SerializeField] private GameObject portal;

    [Header("Volcano Prefabs")]
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject ground4Sides;
    [SerializeField] private GameObject ground3Sides;
    [SerializeField] private GameObject ground2SidesOpposite;
    [SerializeField] private GameObject ground2SidesContiguous;
    [SerializeField] private GameObject ground1Side;
    [SerializeField] private GameObject lavaPlane;


    [Header("Material")]
    [SerializeField] private Material exitLevelMaterial;
    [SerializeField] private Material entryLevelMaterial;
    [SerializeField] private Material lavaMaterial;


    [Header("Random Walk Generator")]
    [SerializeField] private int numberOfGrids;
    [SerializeField] private Vector3 positionFirstGrid;
    [SerializeField] private int gridsDistance;
    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeZ;
    [SerializeField] private int cellSize;
    [SerializeField] private int numberOfTiles;

    [SerializeField] private List<Vector2Int> directions;
    private List<int[,]> preGrids=new List<int[,]>();
    private List<GameObject[,]> grids=new List<GameObject[,]>();



    void Start()
    {
        directions = new List<Vector2Int> { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        GenerateGrids();
        RandomWalkMethod();
        PrintPreGrids();
        InstantiatePrefabsOnGrids();
    }


    void Update()
    {
        
    }

    /// <summary>
    /// Generate the number of preGrids and grids and add them to the lists
    /// The preGrids contain only numbers that lately will be replaced for assets
    /// The grids contain GameObjectsthat create the map
    /// </summary>
    private void GenerateGrids()
    {
        for(int i=0; i<numberOfGrids; i++)
        {
            preGrids.Add(new int[gridSizeX, gridSizeZ]);
            grids.Add(new GameObject[gridSizeX, gridSizeZ]);

            for (int j = 0; j < gridSizeX; j++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    preGrids[i][j, z] = 0;
                    grids[i][j, z] = null;
                }
            }
        }

        

    }

    /// <summary>
    /// Generate each procedural levels for all the grids created.
    /// Each grid contain one procedural level.
    /// STEPS:
    /// 1-Iterate over all pregrids
    /// 2-Create a random point start for the first level
    /// 3-Start walking and generating future tiles assigning 1 to each tile cell if the cell position is empty and correct
    /// </summary>
    private void RandomWalkMethod()
    {
        Vector2Int lastPosition=new Vector2Int();
        Vector2Int lastDirection=new Vector2Int();
        Vector2Int newDirection= new Vector2Int();
        List<Vector2Int> lastDirectionError = new List<Vector2Int>();
        int tilesCont=0;

        for (int i = 0; i < preGrids.Count; i++)
        {
            if (i == 0)
            {
                //Set the initial position
                lastPosition = GetRandomGridPosition();
                preGrids[i][lastPosition.x, lastPosition.y] = 3;
                Debug.Log("Inicio en celda: " + lastPosition);
                lastDirection = new Vector2Int(0, 0);

                //Loop until we reach the number of tiles
                while(tilesCont < numberOfTiles)
                {
                    //Get a random direction
                    do
                    {
                        newDirection = GetRandomDirection();

                        //If there has been a last error direction just make sure to dont repeat it
                        if (lastDirectionError.Count > 0)
                        {
                            for(int j=0;j<lastDirectionError.Count; j++)
                            {
                                if(newDirection==lastDirectionError[j])
                                {
                                    newDirection = lastDirection;
                                    break;
                                }
                            }
                        }
                        
                    } while (newDirection==lastDirection || newDirection==(lastDirection * new Vector2Int(-1, -1)));

                    //Check the new position is inside the grid
                    if ((lastPosition.x + newDirection.x < gridSizeX && lastPosition.y + newDirection.y < gridSizeZ && lastPosition.x + newDirection.x >= 0 && lastPosition.y + newDirection.y >= 0))
                    {

                        //If the grid cell was on 0 value, then update the cont
                        if (preGrids[i][lastPosition.x + newDirection.x, lastPosition.y + newDirection.y] == 0)
                        {
                            //Change the grid cell value to 1
                            preGrids[i][lastPosition.x+newDirection.x, lastPosition.y+newDirection.y] = 1;
                            tilesCont++;
                            
                        }
                        
                        lastPosition += newDirection;
                        lastDirection = newDirection;
                        lastDirectionError.Clear();


                    }
                    else
                    {
                        //The new direction means a position outside the grid
                        lastDirectionError.Add(newDirection);
                    }
                   
                }
                preGrids[i][lastPosition.x, lastPosition.y] = 2;
            }
            else
            {
                //Set the startingPosition at the lastPosition of the previous grid
                preGrids[i][lastPosition.x, lastPosition.y] = 3;
                Debug.Log("Inicio en celda: " + lastPosition);
                lastDirection = new Vector2Int(0, 0);

                //Loop until we reach the number of tiles
                while (tilesCont < numberOfTiles)
                {
                    //Get a random direction
                    do
                    {
                        newDirection = GetRandomDirection();

                        //If there has been a last error direction just make sure to dont repeat it
                        if (lastDirectionError.Count > 0)
                        {
                            for (int j = 0; j < lastDirectionError.Count; j++)
                            {
                                if (newDirection == lastDirectionError[j])
                                {
                                    newDirection = lastDirection;
                                    break;
                                }
                            }
                        }

                    } while (newDirection == lastDirection || newDirection == (lastDirection * new Vector2Int(-1, -1)));

                    //Check the new position is inside the grid
                    if ((lastPosition.x + newDirection.x < gridSizeX && lastPosition.y + newDirection.y < gridSizeZ && lastPosition.x + newDirection.x >= 0 && lastPosition.y + newDirection.y >= 0))
                    {

                        //If the grid cell was on 0 value, then update the cont
                        if (preGrids[i][lastPosition.x + newDirection.x, lastPosition.y + newDirection.y] == 0)
                        {
                            //Change the grid cell value to 1
                            preGrids[i][lastPosition.x+newDirection.x, lastPosition.y+newDirection.y] = 1;

                            tilesCont++;
                        }

                        
                        lastPosition += newDirection;
                        lastDirection = newDirection;
                        lastDirectionError.Clear();


                    }
                    else
                    {
                        //The new direction means a position outside the grid
                        lastDirectionError.Add(newDirection);
                    }

                }

                preGrids[i][lastPosition.x, lastPosition.y] = 2;
            }
            tilesCont = 0;
        }
    }

    /// <summary>
    /// Get a random position of a grid
    /// </summary>
    /// <returns>Vector2Int with a random grid position</returns>
    private Vector2Int GetRandomGridPosition()
    {
        Vector2Int output=new Vector2Int(Random.Range(0, gridSizeX - 1), Random.Range(0, gridSizeZ - 1));

        return output;
    }


    private Vector2Int GetRandomDirection()
    {
        int index = Random.Range(0, directions.Count);

        return directions[index];
    }

    private void InstantiatePrefabsOnGrids()
    {
        int randomRotationGround = 0;
        Quaternion rotationGround=new Quaternion();
        for (int i = 0; i < preGrids.Count; i++)
        {
            for (int j = 0; j < gridSizeX; j++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    if (preGrids[i][j, z] != 0)
                    {

                        //Comprobar alrededores y instanciar el prefab adecuado
                        CheckSurroundingsAndInstantiate(i, j, z);
                        

                        //grids[i][j, z] = Instantiate(ground4Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), rotationGround);
                    }
                    else
                    {
                        //InstantiateDeeperCoverGround(i, j, z);
                    }

                    if (preGrids[i][j, z] == 2)
                    {
                        //Salida de nivel
                        grids[i][j, z].GetComponent<Renderer>().material = exitLevelMaterial;
                        Instantiate(portal, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i) - 9.5f, positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(35f, Vector3.up));

                    }
                    else if (preGrids[i][j, z] == 3)
                    {
                        grids[i][j, z].GetComponent<Renderer>().material = entryLevelMaterial;
                        //Instantiate(portal, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i) - 9.5f, positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(35f, Vector3.up));

                    }
                }
            }
            InstantiateDeeperCoverGround(i);
        }
        
    }

    private void InstantiateDeeperCoverGround(int i)
    {
        Vector3 coverPos = new Vector3(positionFirstGrid.x + ((gridSizeX * cellSize) / 2), positionFirstGrid.y + (cellSize * i)+ 3.0f, positionFirstGrid.z + ((gridSizeZ*cellSize) / 2));
        
        GameObject lavaPlaneInstance = Instantiate(lavaPlane, coverPos, Quaternion.AngleAxis(-90f, Vector3.right));
        lavaPlaneInstance.transform.localScale *= (gridSizeX+4);
        float defaultValueShader = lavaPlaneInstance.GetComponent<Renderer>().material.GetFloat("Vector1_64CE64ED");
        Debug.Log("Valor default del shader: "+ defaultValueShader.ToString());
       lavaPlaneInstance.GetComponent<Renderer>().material.SetFloat("Vector1_64CE64ED", defaultValueShader*(gridSizeX+4));

    }
    //private void InstantiateDeeperCoverGround(int i, int j, int z)
    //{
    //    grids[i][j, z] = Instantiate(lavaPlane, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
    //}


    private void CheckSurroundingsAndInstantiate(int i, int j, int z)
    {
        bool top = false;
        bool bot = false;
        bool left = false;
        bool right = false;

        bool topOut = false;
        bool botOut = false;
        bool leftOut = false;
        bool rightOut = false;
        int contZeros = 0;

        //Check grid boundings
        if((j-1)<0)
        {
            topOut = true;
            contZeros++;
        }
       if((j+1)>=gridSizeX)
        {
            botOut = true;
            contZeros++;
        }
        if ((z - 1) < 0)
        {
            leftOut = true;
            contZeros++;
        }
        if ((z + 1) >= gridSizeX)
        {
            rightOut = true;
            contZeros++;
        }


        //Check the surrounding of the position
        if(!topOut)
        {
            if (preGrids[i][j - 1, z] != 0)
            {
                top = true;
            }
            else
                contZeros++;
        }
        if(!botOut)
        {
            if (preGrids[i][j + 1, z] != 0)
            {
                bot = true;
            }
            else
                contZeros++;
        }
        if(!leftOut)
        {
            if (preGrids[i][j, z - 1] != 0)
            {
                left = true;
            }
            else
                contZeros++;
        }
        if(!rightOut)
        {
            if (preGrids[i][j, z + 1] != 0)
            {
                right = true;
            }
            else
                contZeros++;
        }

        switch(contZeros)
        {
            case 0:
                //grids[i][j, z] = Instantiate(ground4Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                grids[i][j, z] = Instantiate(ground4Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                break;
            case 1:
                if(!top)
                {
                    //grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 90f, 0));
                    //grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    //grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 90f, Space.World);
                }
                else if(!right)
                {
                    //grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 180f, 0));
                    //grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 90f, Space.World);
                }
                else if(!bot)
                {
                    //grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 270f, 0));
                    //grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 180f, Space.World);
                }
                else if(!left)
                {
                    //grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    //grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground3Sides, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 270f, Space.World);
                }
                break;
            case 2:
                if(left && right)
                {
                    //grids[i][j, z] = Instantiate(ground2SidesOpposite, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    //grids[i][j, z] = Instantiate(ground2SidesOpposite, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground2SidesOpposite, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 90f, Space.World);
                }
                else if (top && bot)
                {
                    //grids[i][j, z] = Instantiate(ground2SidesOpposite, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 90f, 0));
                    //grids[i][j, z] = Instantiate(ground2SidesOpposite, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground2SidesOpposite, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                }
                else
                {
                    if(left && top)
                    {
                        //grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 180f, 0));
                        //grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                        grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                        grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 90f, Space.World);
                    }
                    else if(top && right)
                    {
                        //grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 270f, 0));
                        //grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                        grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                        grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 180f, Space.World);
                    }
                    else if(right && bot)
                    {
                        //grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                        //grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                        grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                        grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 270f, Space.World);
                    }
                    else if(bot && left)
                    {
                        //grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 90f, 0));
                        //grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                        grids[i][j, z] = Instantiate(ground2SidesContiguous, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                        //grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 90f, Space.World);
                    }
                }
                break;
            case 3:
                if(left)
                {
                    //grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 90f, 0));
                    //grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 180f, Space.World);
                }
                else if(right)
                {
                    //grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 270f, 0));
                    //grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    //grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 180f, Space.World);
                }    
                else if(top)
                {
                    //grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    //grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 270f, Space.World);
                }  
                else if(bot)
                {
                    //grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.Euler(-90f, 180f, 0));
                    //grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.identity);
                    grids[i][j, z] = Instantiate(ground1Side, new Vector3(positionFirstGrid.x + (cellSize * j), positionFirstGrid.y + (cellSize * i), positionFirstGrid.z + (cellSize * z)), Quaternion.AngleAxis(-90f, Vector3.right));
                    grids[i][j, z].gameObject.transform.Rotate(Vector3.up, 90f, Space.World);
                }
                break;
        }

    }

    private void PrintPreGrids()
    {
        string line = "";


        for (int i = 0; i < preGrids.Count; i++)
        {
            Debug.Log("PreGrid Number:"+i);
            for (int j = 0; j < gridSizeX; j++)
            {
                line += "Row " + j + ":  ";
                for (int z = 0; z < gridSizeZ; z++)
                {
                    line += preGrids[i][j, z] + ", ";
                }
                Debug.Log(line);
                line = "";
            }
        }
    }
}
