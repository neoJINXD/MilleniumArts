using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Game loop for the host only, p2p client has no gameloop instance
 */

public class GameLoop
{
    private GameLoop()
    {
        
    }

    public static GameLoop Instance
    {
        get { return instance ??= new GameLoop(); }
    }
    
    private static GameLoop instance;

    private List<Player> players = new List<Player>();

    //To be able to wait for local player, networked player, and AI player turns
    public IEnumerator Play()
    {
        while (true)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                player.StartTurn();
                Debug.Log("Player " + i + "'s turn.");
                yield return new WaitUntil(() => player.TurnComplete);
            }
        }
    }

    public void AddPlayer(Player toAdd)
    {
        players.Add(toAdd);
    }
}
