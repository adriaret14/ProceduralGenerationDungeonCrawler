using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Method {NONE, RANDOM_WALK};
public class MapGenerator : MonoBehaviour
{

    [Header("Terrains")]
    [SerializeField] private List<GameObject> terrainPrefabs;

    [Header("Method Selector")]
    [SerializeField] private Method generationMethod;

    [Header("Random Walk Method properties")]
    [SerializeField] private int maxTunnelLength;
    [SerializeField] private int maxTunnels;
    [SerializeField] private bool allowDiagonals;

    [Header("Grids")]
    [SerializeField] private int numberOfGrids;
    [SerializeField] private int cellSize;
    [SerializeField] private int gridSize;
    [SerializeField] private List<CustomGrid> grids;


    void Start()
    {
        for(int i=0; i<numberOfGrids; i++)
        {
            grids.Add(new CustomGrid(gridSize, cellSize, new Vector3(0, 0*(-cellSize), 0)));
            grids[i].initializeGrid(null);
        }

        switch(generationMethod)
        {
            case Method.NONE:

                break;
            case Method.RANDOM_WALK:
                grids[0].assignTerrains(terrainPrefabs);
                grids[0].SetRandowWalkProperties(maxTunnelLength, maxTunnels, allowDiagonals);
                grids[0].createRandomWalkMap();
                break;
        }
    }

    
    void Update()
    {

    }
}
