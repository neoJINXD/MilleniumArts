using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Pathfinding : MonoBehaviour 
{
	
	PathRequestManager requestManager;

	delegate int HeuristicFunction(Node a, Node b); // dynamically change heuristic calculation  
	Grid grid;
	HeuristicFunction hf;
	
	// for testing
	// [SerializeField] private Transform unit;
	private Vector3 initialPosition;
	
	// unity crashes when depth is greater than 17, setting restriction.
	// to move to unit class.
	[Range(0,15)] public int depthLimit;

	public enum Heuristic
	{
		TileDistance,
		EuclideanDistance,
		Dijkstra
	}
	void Awake() 
	{
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}
	
	/*
	* BFS function to add.
	* Instead of calculating all the nodes on the map. Need to set a limit on how far that unit can go, instead of anywhere on the map.
	*/
	public HashSet<Node> BFSLimitSearch(Vector3 startPos, bool canFly, int depth)
	{
		Node startNode = null;
		
		if (grid != null) // fixes null reference error, when terminating the game
		{
			startNode = grid.NodeFromWorldPoint(startPos);
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
			var neighbours = grid.GetAdjacent(currentNode);

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
	
	public HashSet<Node> GetNodesMinMaxRange(Vector3 startPos, bool canFly, int minRange, int maxRange)
	{
		Node startNode = null;

		Node origin = null;
		
		
		if (grid != null) // fixes null reference error, when terminating the game
		{
			startNode = grid.NodeFromWorldPoint(startPos);
			origin = grid.NodeFromWorldPoint(startPos);
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
			
			var neighbours = grid.GetAdjacent(currentNode);
			

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
			
		return visited;
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

		Node[] waypoints = new Node[0];
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		HashSet<Node> nodesInBfs = BFSLimitSearch(startPos, canFly, depthLimit);
		
		if (startNode.canWalkHere && targetNode.canWalkHere && nodesInBfs.Contains(targetNode)) 
		{
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
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
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode))
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
		}
		
		requestManager.FinishedProcessingPath(waypoints, pathSuccess);
	}
	
	Node[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
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