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
    public List<TrapOrItem> trapsOrItemsInThisNode = new List<TrapOrItem>();
	
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

    //setters and getters for unit in this node
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
    
    // setters and getters for list of traps or items in this node
    public void SetTrapOrItemList(List<TrapOrItem> toiList)
    {
        trapsOrItemsInThisNode = toiList;
    }

    public List<TrapOrItem> GetTrapOrItemList()
    {
        return trapsOrItemsInThisNode;
    }

    //adds the trap or item to node
    public void AddTrapOrItem(TrapOrItem toiToAdd)
    {
        trapsOrItemsInThisNode.Add(toiToAdd);
    }

    //returns true if given item or trap is in this node and removes it, or else returns false
    public bool RemoveTrapOrItem(TrapOrItem toiToRemove)
    {
        if (trapsOrItemsInThisNode.Contains(toiToRemove))
        {
            trapsOrItemsInThisNode.Remove(toiToRemove);
            return true;
        }
        else
        {
            return false;
        }
    }
}