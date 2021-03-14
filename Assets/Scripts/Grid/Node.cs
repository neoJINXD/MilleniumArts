using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {
	
    public bool canWalkHere;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;
	
    public Node(bool canWalkHere, Vector3 worldPosition, int gridX, int gridY) {
        this.canWalkHere = canWalkHere;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fCost 
    {
        get 
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare) 
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) 
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}