using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform unit;
    // the size of the grid
    [SerializeField] private float size;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileUnwalkablePrefab;
    [SerializeField] private Unit kingPlayer1;
    [SerializeField] private Unit kingPlayer2;
    [SerializeField] private Transform[] kingSpawnP1;
    [SerializeField] private Transform[] kingSpawnP2;

    public static GameObject[,] tileTrack;
    public static GameObject[] tileCollides;

    private Vector3 newPosition;

    // implement dictionary class. ?

    public bool displayGridGizmos;
    public LayerMask unableToWalkHere;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Node[,] grid;
    private float nodeDiameter;
    public int gridSizeX, gridSizeY;

    void Awake()
    {
        
        nodeDiameter = nodeRadius * 2;
        // gives us how many nodes we can fit in our grid world size.
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void LateUpdate()
    {
        newPosition.x = Mathf.Floor(target.position.x / size) * size;
        newPosition.y = Mathf.Floor(target.position.y / size) * size;
        newPosition.z = Mathf.Floor(target.position.z / size) * size;

        unit.transform.position = newPosition;
    }

    //returns a list of all unit nodes
    public List<Node> GetAllUnitNodes()
    {
        List<Node> allUnitNodes = new List<Node>();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x,y].GetUnit() == null)
                {
                    continue;
                }
                
                allUnitNodes.Add(grid[x,y]);
            }
        }

        return allUnitNodes;
    }

    //returns a list of all ally unit nodes
    public List<Node> GetAllyUnitNodes(int callingPlayerID)
    {
        List<Node> allyUnitNodes = new List<Node>();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x,y].GetUnit() != null)
                {
                    if (grid[x,y].GetUnit().GetUnitPlayerID() == callingPlayerID)
                    {
                        allyUnitNodes.Add(grid[x,y]);
                    }
                }
            }
        }
        return allyUnitNodes;
    }
    
    

    //returns a list of all enemy unit nodes
    //pass the calling player's ID, NOT the enemy player ID
    public List<Node> GetEnemyUnitNodes(int callingPlayerID)
    {
        List<Node> enemyUnitNodes = new List<Node>();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x, y].GetUnit() != null)
                {
                    if ((grid[x,y].GetUnit().GetUnitPlayerID() == callingPlayerID)||(grid[x,y].GetUnit() == null))
                    {
                        continue;
                    }
                
                    enemyUnitNodes.Add(grid[x,y]);
                }
            }
        }

        return enemyUnitNodes;
    }
    

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        tileTrack = new GameObject[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                
                // returns true if collision
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unableToWalkHere));
                
                if (walkable)
                {
                    tileTrack[x, y] = Instantiate(tilePrefab, worldPoint, quaternion.Euler(0, 0, 0), gridParent);
                }
                else
                {
                    tileTrack[x, y] = Instantiate(tileUnwalkablePrefab, worldPoint, quaternion.Euler(0, 0, 0), gridParent);
                }
                
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }

        CreateKings();
    }

    private void CreateKings()
    {
        int randPos = Random.Range(0, kingSpawnP1.Length);
        
        Transform spawnPosP1 = kingSpawnP1[randPos];
 
        Transform spawnPosP2 = kingSpawnP2[randPos];

        kingPlayer1.SetUnitPlayerID(0);
        kingPlayer2.SetUnitPlayerID(1);
        Instantiate(kingPlayer1, spawnPosP1.transform, false);
        Instantiate(kingPlayer2, spawnPosP2.transform, false);
        NodeFromWorldPoint(spawnPosP1.transform.position).AddUnit(kingPlayer1);
        NodeFromWorldPoint(spawnPosP2.transform.position).AddUnit(kingPlayer2);
        //adding the kings to their respective nodes
    }

    // might have to use boolean, to change walkable nodes, based on flying and ground units.
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int checkX = node.gridX + 1;
        int checkY = node.gridY;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(grid[checkX, checkY]);
        }
        
        checkX = node.gridX - 1;
        checkY = node.gridY;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(grid[checkX, checkY]);
        }
        
        checkX = node.gridX;
        checkY = node.gridY + 1;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(grid[checkX, checkY]);
        }
        
        checkX = node.gridX;
        checkY = node.gridY - 1;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(grid[checkX, checkY]);
        }

        return neighbours;
    }

    public List<Node> GetAdjacent(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // need to check if at an edge.
        if (node.gridY + 1 < gridSizeY)
            neighbours.Add(grid[node.gridX, node.gridY + 1]);
        if (node.gridY - 1 >= 0)
            neighbours.Add(grid[node.gridX, node.gridY - 1]);
        if (node.gridX + 1 < gridSizeX)
            neighbours.Add(grid[node.gridX + 1, node.gridY]);
        if (node.gridX - 1 >= 0)
            neighbours.Add(grid[node.gridX - 1, node.gridY]);
        
        return neighbours;
    }

    // converts a world position to a node on the grid
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        
        // has to always be clamped between 0 and 1 to make sure not outside of grid array.
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];

    }
    
    void OnDrawGizmos() 
    {
        Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

        if (grid != null && displayGridGizmos) 
        {
            foreach (Node n in grid) 
            {
                Gizmos.color = (n.canWalkHere)?Color.white:Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
            }
        }
    }
}