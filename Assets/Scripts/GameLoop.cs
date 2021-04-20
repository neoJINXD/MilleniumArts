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

    [SerializeField] private List<Player> players = new List<Player>();

    private int index;
    private int turnMana = 3;

    //To be able to wait for local player, networked player, and AI player turns
    public IEnumerator Play()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].PlayerId = i;
        }
        
        while (true)
        {
            foreach (var player in players)
            {
                player.PlayerMana = turnMana;
            }
            
            for (index = 0; index < players.Count; index++)
            {
                Player player = players[index];
                player.StartTurn();
                Debug.Log("Player " + index + "'s turn.");
                yield return new WaitUntil(() => player.TurnComplete);
                turnMana++;
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
	
	public Player GetPlayer(int incomingIndex)
    {
		if(incomingIndex < 0 && incomingIndex >= players.Count)
			return players[0];
		
        return players[incomingIndex];
    }
	
	public Player GetOtherPlayer(int currentPlayer)
	{
		if(currentPlayer == 1)
			return players[0];
		else 
			return players[1];
	}
	
    public void EndCurrentPlayer()
    {
        players[index].EndTurn();
    }

    public List<Player> GetPlayerList()
    {
        return players;
    }
}

