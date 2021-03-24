using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : IHeapItem<Node> {
	
    public bool canWalkHere;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public Unit unitInThisNode;
	
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

    //setters and getters for list
    public void SetUnit(Unit unit)
    {
        unitInThisNode = unit;
    }

    public Unit GetUnit()
    {
        return unitInThisNode;
    }

    //check to see if unit can be added in the node
    public bool CanAddUnitCheck(Unit unitToAdd)
    {
        if (unitInThisNode == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //returns true if unit is added, else false if it fails to add
    public bool AddUnit(Unit unitToAdd)
    {
        bool check = CanAddUnitCheck(unitToAdd);

        if (check)
        {
            unitInThisNode = unitToAdd;
            return true;
        }
        else
        {
            return false;
        }
    }

    //returns true if unit is removed, else false if it fails to remove
    public bool RemoveUnit(Unit unitToRemove)
    {
        if (unitInThisNode == unitToRemove)
        {
            unitInThisNode = null;

            return true;
        }
        else
        {
            return false;
        }
    }
}