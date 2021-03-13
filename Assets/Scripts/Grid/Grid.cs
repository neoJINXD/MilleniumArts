using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * To test this scrip, run the game in scene view and move the map game object around.
 * You will see the clip movement between grids of the unit.
 */
public class Grid : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform unit;
    // the size of the grid
    [SerializeField] private float size;

    private Vector3 newPosition;
    
    public LayerMask unableToWalkHere;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        // gives us how many nodes we can fit in our grid world size.
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
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
                grid[x, y] = new Node(walkable, worldPoint);
            }
        }
    }

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
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.canWalkHere) ? Color.white : Color.red;

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
