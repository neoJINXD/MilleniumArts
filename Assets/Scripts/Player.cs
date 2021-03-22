using UnityEngine;

/*
    Abstract class to be inherited by a local player, networked player, client player?, and AI player
 */

public /* abstract */ class Player : MonoBehaviour
{
    public Player()
    {
        TurnComplete = true;
    }

    public void StartTurn()
    {
        TurnComplete = false;
    }

    public virtual void EndTurn()
    {
        TurnComplete = true;
    }

    public bool TurnComplete { get; protected set; }
}

