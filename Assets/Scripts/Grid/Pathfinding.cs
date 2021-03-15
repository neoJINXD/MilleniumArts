using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour 
{
	
	PathRequestManager requestManager;

	delegate int HeuristicFunction(Node a, Node b); //dynamically change heuristic calculation  
	Grid grid;
	
	void Awake() {
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}
	
	
	public void StartFindPath(Vector3 startPos, Vector3 targetPos) 
	{
		StartCoroutine(FindPath(startPos,targetPos, true, GetDistance)); //pass the function to use to calculate hCost
	}
	
	// implement function to click on tile as targetPosition, and be able to select unit.
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, bool canFly, HeuristicFunction heuristicFunc) 
	{

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		
		
		if (startNode.canWalkHere && targetNode.canWalkHere) 
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
					if ((!canFly && !neighbour.canWalkHere) || closedSet.Contains(neighbour)) 
					{
						continue;
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
	
	Vector3[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		//Vector3[] waypoints = SimplifyPath(path);
		Vector3[] waypoints = ConvertToArray(path);
		Array.Reverse(waypoints);
		return waypoints;
		
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
	
	int GetDistance(Node nodeA, Node nodeB) 
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
}