using UnityEngine;
using System.Collections;

public class Node {
	
    public bool canWalkHere;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
	
    public Node(bool canWalkHere, Vector3 worldPosition, int gridX, int gridY)
    {
        this.canWalkHere = canWalkHere;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }
}