using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zone.Core.Utils;
using Random = UnityEngine.Random;

public class Grid : Singleton<Grid>
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
	
	[SerializeField]private Unit king1;
	[SerializeField]private Unit king2;

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
        
    }
	
	private void Start()
	{
        CreateGrid();
		GameLoop.instance.GetPlayer(0).King = king1;
		GameLoop.instance.GetPlayer(1).King = king2;
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

    public HashSet<Node> GetPlaceableNodes(Card currentCard)
    {
        HashSet<Node> placeableNodes = new HashSet<Node>();
        List<Node> allyNodes = GetAllyUnitNodes(GameLoop.instance.GetCurrentPlayer().PlayerId);
        foreach (Node node in allyNodes)
        {
            Vector3 nodePos = node.unitInThisNode.transform.position;
            placeableNodes.UnionWith(Pathfinding.instance.GetNodesMinMaxRange(
                nodePos, 
                false, 
                currentCard.minRange, 
                currentCard.maxRange));
        }

        return placeableNodes;
    }

    // overload for explicit unit types
    public HashSet<Node> GetPlaceableNodes(Card currentCard, Unit.UnitTypes unitType)
    {
        HashSet<Node> placeableNodes = new HashSet<Node>();
        List<Node> allyNodes = GetAllyUnitNodes(GameLoop.instance.GetCurrentPlayer().PlayerId);
        List<Node> filteredNodes = new List<Node>();

        foreach (Node node in allyNodes)
        {
            if (node.GetUnit().GetUnitType() == unitType)
                filteredNodes.Add(node);
        }

        foreach (Node node in filteredNodes)
        {
            Vector3 nodePos = node.unitInThisNode.transform.position;
            placeableNodes.UnionWith(Pathfinding.instance.GetNodesMinMaxRange(
                nodePos,
                false,
                currentCard.minRange,
                currentCard.maxRange));
        }

        return placeableNodes;
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

        if (GameManager.instance.networked)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                king1 = PhotonNetwork.Instantiate("Units/King 1", spawnPosP1.position, spawnPosP1.rotation).GetComponent<Unit>();
                king2 = PhotonNetwork.Instantiate("Units/King 2", spawnPosP2.position, spawnPosP2.rotation).GetComponent<Unit>();
            }
        }
        else
        {
            king1 = Instantiate(kingPlayer1, spawnPosP1.transform, false).GetComponent<Unit>();
            king2 = Instantiate(kingPlayer2, spawnPosP2.transform, false).GetComponent<Unit>();
        }

        if (GameManager.instance.networked && PhotonNetwork.IsMasterClient)
        {
            GetComponent<PhotonView>().RPC("FindKings", RpcTarget.All, spawnPosP1.position, spawnPosP2.position);
        }

        if (!GameManager.instance.networked)
        {
            //adding the kings to their respective nodes
            NodeFromWorldPoint(spawnPosP1.position).AddUnit(king1);
            NodeFromWorldPoint(spawnPosP2.position).AddUnit(king2);

            // TODO should prob have these just handled by the prefab except ID
            king1.SetMovementSpeed(5);
            king1.SetCanFly(false);
            king1.SetUnitType(Unit.UnitTypes.King);
            king1.SetUnitPlayerID(0);
            king1.SetMaxHealth(30);
            king1.SetCurrentHealth(30);
            king1.SetDamage(7);
            king1.SetDefence(2);
            king1.SetMinRange(1);
            king1.SetMaxRange(1);
            king1.SetAccuracy(90);
            king1.SetEvasion(30);

            king2.SetMovementSpeed(5);
            king2.SetCanFly(false);
            king2.SetUnitType(Unit.UnitTypes.King);
            king2.SetUnitPlayerID(1);
            king2.SetMaxHealth(30);
            king2.SetCurrentHealth(30);
            king2.SetDamage(7);
            king2.SetDefence(2);
            king2.SetMinRange(1);
            king2.SetMaxRange(1);
            king2.SetAccuracy(90);
            king2.SetEvasion(30);
        }

    }

    [PunRPC]
    private void FindKings(Vector3 spawnPos1, Vector3 spawnPos2)
    {   
        print("Setting up the kings");
        king1 = GameObject.Find("King 1(Clone)").GetComponent<Unit>();
        king2 = GameObject.Find("King 2(Clone)").GetComponent<Unit>();

        //adding the kings to their respective nodes
        NodeFromWorldPoint(spawnPos1).AddUnit(king1);
        NodeFromWorldPoint(spawnPos2).AddUnit(king2);

        // TODO should prob have these just handled by the prefab except ID
        king1.SetMovementSpeed(5);
        king1.SetCanFly(false);
        king1.SetUnitType(Unit.UnitTypes.King);
        king1.SetUnitPlayerID(0);
        king1.SetMaxHealth(30);
        king1.SetCurrentHealth(30);
        king1.SetDamage(7);
        king1.SetDefence(2);
        king1.SetMinRange(1);
        king1.SetMaxRange(1);
        king1.SetAccuracy(90);
        king1.SetEvasion(30);

        king2.SetMovementSpeed(5);
        king2.SetCanFly(false);
        king2.SetUnitType(Unit.UnitTypes.King);
        king2.SetUnitPlayerID(1);
        king2.SetMaxHealth(30);
        king2.SetCurrentHealth(30);
        king2.SetDamage(7);
        king2.SetDefence(2);
        king2.SetMinRange(1);
        king2.SetMaxRange(1);
        king2.SetAccuracy(90);
        king2.SetEvasion(30);
    }

    // might have to use boolean, to change walkable nodes, based on flying and ground units.
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        if (node.gridY + 1 < gridSizeY) //checking up
        {
            if (grid[node.gridX, node.gridY + 1].unitInThisNode == null) //making sure no unit in the node
            {
                neighbours.Add(grid[node.gridX, node.gridY + 1]);
            }
        }

        if (node.gridY - 1 >= 0) //checking down
        {
            if (grid[node.gridX, node.gridY - 1].unitInThisNode == null)
            {
                neighbours.Add(grid[node.gridX, node.gridY - 1]);
            }
        }

        if (node.gridX + 1 < gridSizeX) //checking right
        {
            if (grid[node.gridX + 1, node.gridY].unitInThisNode == null)
            {
                neighbours.Add(grid[node.gridX + 1, node.gridY]);
            }
        }

        if (node.gridX - 1 >= 0) //checking left
        {
            if (grid[node.gridX - 1, node.gridY].unitInThisNode == null)
            {
                neighbours.Add(grid[node.gridX - 1, node.gridY]);
            }
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