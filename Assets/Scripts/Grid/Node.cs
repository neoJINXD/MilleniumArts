using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool canWalkHere;
    public Vector3 worldPosition;

    public Node(bool canWalkHere, Vector3 worldPosition)
    {
        this.canWalkHere = canWalkHere;
        this.worldPosition = worldPosition;
    }
}
