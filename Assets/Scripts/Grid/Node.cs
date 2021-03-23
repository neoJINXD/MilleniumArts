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

    public int playerIDOfUnits = -1;     //keep track of which player's units are stored here
                                        //set to -1 if node contains no units
    private List<Unit> unitsInThisNode = new List<Unit>(); //the list of units stored here
	
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
    public void SetUnitList(List<Unit> uL)
    {
        unitsInThisNode = uL;
    }

    public List<Unit> GetUnitList()
    {
        return unitsInThisNode;
    }

    //check to see if unit can be added in the node
    public bool CanAddUnitCheck(Unit unitToAdd)
    {
        if ((unitToAdd.GetUnitPlayerID() == playerIDOfUnits)||(playerIDOfUnits == -1))
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
            unitsInThisNode.Add(unitToAdd);
            playerIDOfUnits = unitToAdd.GetUnitPlayerID();
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
        if (unitsInThisNode.Contains(unitToRemove))
        {
            unitsInThisNode.Remove(unitToRemove);

            if (unitsInThisNode.Count == 0)
            {
                playerIDOfUnits = -1;
            }

            return true;
        }
        else
        {
            return false;
        }
    }
}