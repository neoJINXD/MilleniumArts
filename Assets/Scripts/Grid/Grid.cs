using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform unit;
    // the size of the grid
    [SerializeField] private float size;

    private Vector3 newPosition;
    
    // implement dictionary class. ?

    public bool displayGridGizmos;
    public LayerMask unableToWalkHere;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

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

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                // returns true if collision
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unableToWalkHere));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // might have to use boolean, to change walkable nodes, based on flying and ground units.
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

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