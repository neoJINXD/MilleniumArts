using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop
{
    private GameLoop()
    {}

    public static GameLoop Instance
    {
        get { return instance ??= new GameLoop(); }
    }
    
    private static GameLoop instance;

    private List<Player> players = new List<Player>();

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
