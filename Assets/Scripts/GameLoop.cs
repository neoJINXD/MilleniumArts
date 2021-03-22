using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zone.Core.Utils;

/*
    Game loop for the host only, p2p client has no gameloop instance
 */

public class GameLoop : Singleton<GameLoop>
{
    private GameLoop()
    {}

    private List<Player> players = new List<Player>();

    private int index;

    //To be able to wait for local player, networked player, and AI player turns
    public IEnumerator Play()
    {
        while (true)
        {
            for (index = 0; index < players.Count; index++)
            {
                Player player = players[index];
                player.StartTurn();
                Debug.Log("Player " + index + "'s turn.");
                yield return new WaitUntil(() => player.TurnComplete);
            }
        }
    }

    public void AddPlayer(Player toAdd)
    {
        players.Add(toAdd);
    }

    public Player AddReturnPlayer(Player toAdd)
    {
        players.Add(toAdd);
        return toAdd;
    }

    public Player GetCurrentPlayer()
    {
        return players[index];
    }
    public void EndCurrentPlayer()
    {
        players[index].EndTurn();
    }
}

