using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Serialization;
using Zone.Core.Utils;

public class Pathfinding : Singleton<Pathfinding>
{

	PathRequestManager requestManager;
	[SerializeField] private Material hoveredTile;
	[SerializeField] private Material defaultMat;
	
	public  Node[] waypoints;
    public static int pathLength;

	delegate int HeuristicFunction(Node a, Node b); // dynamically change heuristic calculation  
	public Grid gridRef;
	HeuristicFunction hf;
	
	// for testing
	// [SerializeField] private Transform unit;
	private Vector3 initialPosition;
	private Renderer hoverMat;
	private Renderer unhoverMat;
	private bool mouseOver = false;
	
	// unity crashes when depth is greater than 17, setting restriction.
	// to move to unit class.
	[Range(0,15)] public int minDepthLimit;
	[Range(0,15)] public int maxDepthLimit;

	public enum Heuristic
	{
		TileDistance,
		EuclideanDistance,
		Dijkstra
	}
	void Awake() 
	{
		requestManager = GetComponent<PathRequestManager>();
		gridRef = GetComponent<Grid>();
	}
	
	/*
	* BFS function to add.
	* Instead of calculating all the nodes on the map. Need to set a limit on how far that unit can go, instead of anywhere on the map.
	*/
	public HashSet<Node> BFSLimitSearch(Vector3 startPos, bool canFly, int depth)
	{
		Node startNode = null;
		
		if (gridRef != null) // fixes null reference error, when terminating the game
		{
			startNode = gridRef.NodeFromWorldPoint(startPos);
		}

		Queue<Node> queue = new Queue<Node>();
		HashSet<Node> visited = new HashSet<Node>();

		queue.Enqueue(startNode);
		queue.Enqueue(null);

		int depthCounter = 0;

		while (queue.Count != 0 && depthCounter <= depth)
		{
			Node currentNode = queue.Dequeue();

			// check for depth
			if (currentNode == null)
			{
				depthCounter++;
				queue.Enqueue(null);

				if (queue.Peek() == null)
				{
					// TODO the BFS is done looking since 2 nulls are consecutive
					// return result;
					break;
				}
				continue;
			}


			visited.Add(currentNode);
			var neighbours = gridRef.GetAdjacent(currentNode);

			foreach (var neighbour in neighbours)
			{
				if ((!canFly && !neighbour.canWalkHere) || visited.Contains(neighbour)) 
				{
					continue; //Skips unwalkable nodes when unit cannot fly, or if any node in closed set
					//considers unwalkable nodes if unit can fly, and ignores any node if in closed set
				}
				queue.Enqueue(neighbour);
			}
		}

		return visited;
	}
	
	// get all nodes within a min/max range
	public HashSet<Node> GetNodesMinMaxRange(Vector3 startPos, bool canFly, int minRange, int maxRange)
	{
		Node startNode = null;

		Node origin = null;
		
		
		if (gridRef != null) // fixes null reference error, when terminating the game
		{
			startNode = gridRef.NodeFromWorldPoint(startPos);
			origin = gridRef.NodeFromWorldPoint(startPos);
		}

		Queue<Node> queue = new Queue<Node>();
		HashSet<Node> visited = new HashSet<Node>();
		HashSet<Node> verify = new HashSet<Node>();

		
		queue.Enqueue(startNode);
		queue.Enqueue(null);

		int temp = 0;

		int depthCounter = 0;

		while (queue.Count != 0 && depthCounter <= maxRange)
		{
			Node currentNode = queue.Dequeue();
			
			// check for depth
			if (currentNode == null)
			{
				depthCounter++;
				queue.Enqueue(null);

				if (queue.Peek() == null)
				{
					// TODO the BFS is done looking since 2 nulls are consecutive
					// return result;
					break;
				}
				continue;
			}


			if (depthCounter >= minRange)
				visited.Add(currentNode);
			else
				verify.Add(currentNode);
			
			var neighbours = gridRef.GetAdjacent(currentNode);
			

			foreach (var neighbour in neighbours)
			{
				if ((!canFly && !neighbour.canWalkHere) || visited.Contains(neighbour))
				{
					continue; //Skips unwalkable nodes when unit cannot fly, or if any node in closed set
					//considers unwalkable nodes if unit can fly, and ignores any node if in closed sets
				}
				
				queue.Enqueue(neighbour);
			}
		}
		
		if (minRange != 0)
			visited.Remove(origin);

		if (minRange == 0 && maxRange == 0)
			visited.Add(origin);
		
		foreach (Node n in verify)
			visited.Remove(n);
		
		/*foreach (Node n in gridRef.GetAllUnitNodes())
		{
			if (visited.Contains(n))
			{
				visited.Remove(n);
			}
		}*/


		return visited;
	}
	
	// get all enemy nodes within a range
	public List<Node> GetEnemyUnitNodesInRange(int callingPlayerID, Vector3 startPos, bool canFly, int minRange, int maxRange)
	{
		List<Node> enemyUnitNodes = new List<Node>();

		HashSet<Node> inRange = GetNodesMinMaxRange(startPos, canFly, minRange, maxRange);
			
			
		for (int x = 0; x < gridRef.gridSizeX; x++)
		{
			for (int y = 0; y < gridRef.gridSizeY; y++)
			{
				var currentNode = gridRef.grid[x, y];

				if (currentNode.GetUnit() != null)
				{
					if ((currentNode.GetUnit().GetUnitPlayerID() == callingPlayerID)||(currentNode.GetUnit() == null) || !inRange.Contains(currentNode))
					{
						continue;
					}
					enemyUnitNodes.Add(gridRef.grid[x,y]);
				}
			}
		}

		return enemyUnitNodes;
	}
	
	
	// get all ally nodes within a range
	public List<Node> GetAllyUnitNodesInRange(int callingPlayerID, Vector3 startPos, bool canFly, int minRange, int maxRange)
	{
		List<Node> allyUnitNodes = new List<Node>();
		HashSet<Node> inRange = GetNodesMinMaxRange(startPos, canFly, minRange, maxRange);

		for (int x = 0; x < gridRef.gridSizeX; x++)
		{
			for (int y = 0; y < gridRef.gridSizeY; y++)
			{
				var currentNode = gridRef.grid[x, y];

				if (currentNode.GetUnit() != null)
				{
					if (currentNode.GetUnit().GetUnitPlayerID() == callingPlayerID && inRange.Contains(currentNode))
					{
						allyUnitNodes.Add(currentNode);
					}
				}
			}
		}

		return allyUnitNodes;
	}
	
	public void StartFindPath(Vector3 startPos, Vector3 targetPos, bool canFly, int unitPID, Heuristic desiredHeuristic) 
	{
		switch (desiredHeuristic)
		{
			case Heuristic.EuclideanDistance:
			{
				hf = new HeuristicFunction(GetEuclideanDistance);

				break;
			}
			case Heuristic.TileDistance:
			{
				hf = new HeuristicFunction(GetDistance);
				break;
			}

			case Heuristic.Dijkstra:
			{
				hf = new HeuristicFunction(Dijkstra);
				break;
			}

			default:
			{
				hf = new HeuristicFunction(GetDistance);
				break;
			}
		}
		
		StartCoroutine(FindPath(startPos,targetPos, canFly, unitPID, hf));
		//pass the function to use to calculate hCost
		//can pass boolean to determine flying or normal pathfinding
	}
	
	//also takes in the calling unit's PID
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, bool canFly, int unitPlayerID, HeuristicFunction heuristicFunc) 
	{
		waypoints = new Node[0];
		bool pathSuccess = false;
		
		Node startNode = gridRef.NodeFromWorldPoint(startPos);
		Node targetNode = gridRef.NodeFromWorldPoint(targetPos);

        Unit currentUnit = startNode.GetUnit();


        HashSet<Node> nodesInBfsRange = GetNodesMinMaxRange(startPos, canFly, minDepthLimit, maxDepthLimit);
		
		if (startNode.canWalkHere && targetNode.canWalkHere && nodesInBfsRange.Contains(targetNode)) 
		{
			Heap<Node> openSet = new Heap<Node>(gridRef.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) 
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in gridRef.GetNeighbours(currentNode))
				{
					bool checkHostile = false;
					
					if (neighbour.GetUnit() != null)
					{
						if (neighbour.GetUnit().GetUnitPlayerID() != unitPlayerID)
						{
							checkHostile = true;
						}
					}
					
					if ((!canFly && !neighbour.canWalkHere) || closedSet.Contains(neighbour) || checkHostile) 
					{
						continue; //Skips unwalkable nodes when unit cannot fly, or if any node in closed set or if the unit in the node is hostile
						//considers unwalkable nodes if unit can fly, and ignores any node if in closed set
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = heuristicFunc(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		
		yield return null;

		
		if (pathSuccess) 
		{
			waypoints = RetracePath(startNode, targetNode);
            currentUnit.SetMovementSpeedLeft(currentUnit.GetMovementSpeedLeft() - (waypoints.Length - 1));
        }

		requestManager.FinishedProcessingPath(waypoints, pathSuccess);
	}
	
	//for Ai
	public Node[] AIFindPath(Vector3 startPos, Vector3 targetPos, bool canFly, int unitPlayerID) 
	{
		bool pathSuccess = false;
		HeuristicFunction heuristicFunction = new HeuristicFunction(GetDistance);
		Node startNode = gridRef.NodeFromWorldPoint(startPos);
		Node targetNode = gridRef.NodeFromWorldPoint(targetPos);
		Unit currentUnit = startNode.GetUnit();

		if (startNode.canWalkHere && targetNode.canWalkHere) 
		{
			Heap<Node> openSet = new Heap<Node>(gridRef.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) 
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in gridRef.GetNeighbours(currentNode))
				{
					bool checkHostile = false;
					
					if (neighbour.GetUnit() != null)
					{
						if (neighbour.GetUnit().GetUnitPlayerID() != unitPlayerID)
						{
							checkHostile = true;
						}
					}
					
					if ((!canFly && !neighbour.canWalkHere) || closedSet.Contains(neighbour) || checkHostile) 
					{
						continue; //Skips unwalkable nodes when unit cannot fly, or if any node in closed set or if the unit in the node is hostile
						//considers unwalkable nodes if unit can fly, and ignores any node if in closed set
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = heuristicFunction(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}

		if (pathSuccess) 
		{
			Node[] waypoints = RetracePath(startNode, targetNode);
			currentUnit.SetMovementSpeedLeft(currentUnit.GetMovementSpeedLeft() - (waypoints.Length - 1));
			return waypoints;
		}

		return new[] {startNode};
	}
	
	// Using Hover.cs instead of functions below. Keeping for now, in case decide to use them/modify.
	public void DrawPath()
	{
		if (waypoints != null)
		{
			foreach (var node in waypoints)
			{
				Renderer pathMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
				pathMat.material = hoveredTile;
			}
		}
	}

	public void UnDrawPath()
	{
		if (waypoints != null)
		{
			foreach (var node in waypoints)
			{
				Renderer pathMat = Grid.tileTrack[node.gridX, node.gridY].GetComponent<Renderer>();
				pathMat.material = defaultMat;
			}
		}
	}

	private Node[] RetracePath(Node startNode, Node endNode) 
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		
		path.Add(startNode);
		//Vector3[] waypoints = SimplifyPath(path);
		//Vector3[] waypoints = ConvertToArray(path);
		//Array.Reverse(waypoints);
		path.Reverse();
		return path.ToArray();
	}

	Vector3[] SimplifyPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < path.Count; i ++) {
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	Vector3[] ConvertToArray(List<Node> path)
	{
		List<Vector3> waypoints = new List<Vector3>();

		foreach (Node n in path)
		{
			waypoints.Add(n.worldPosition);
		}

		return waypoints.ToArray();
	}
	
	int GetDistance(Node nodeA, Node nodeB) //Gives tile distance between the nodes
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX-dstY);
		return 14 * dstX + 10 * (dstY-dstX);
	}

	int GetEuclideanDistance(Node a, Node b)
	{
		return Mathf.RoundToInt(Vector3.Distance(a.worldPosition, b.worldPosition));
		// returns the Euclidean distance between the given nodes
	}

	int Dijkstra(Node a, Node b)
	{
		return 0;
	}
	
	// WARNING currently commented out since only used for testing
	// private void OnDrawGizmos()
	// {
	// 	initialPosition = unit.transform.position;
	// 	
	//  	var temp = BFSLimitSearch(new Vector3(initialPosition.x, initialPosition.y, initialPosition.z), false, depthLimit);
	//
	// 	if (temp != null && temp.Count > 0)
	// 	{
	// 		foreach (var node in temp)
	// 		{
	// 			Gizmos.color = Color.green;
	// 			Gizmos.DrawCube(node.worldPosition, Vector3.one * (1 - .1f));
	// 		}
	// 	}
	// }
}