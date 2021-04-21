using System;
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
    [SerializeField] private GameObject m_gameOverUI;
    [SerializeField] private TMPro.TMP_Text m_winningPlayerText;

    private int index;
    private int turnMana = 3;
    private bool winCondition = false;
    private int winningId;

    //To be able to wait for local player, networked player, and AI player turns
    public IEnumerator Play()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].PlayerId = i;
        }
        
        while (!winCondition)
        {
            foreach (var player in players)
            {
                player.PlayerMana = turnMana;
                player.PlayerMaxMana = turnMana;
            }
            
            for (index = 0; index < players.Count; index++)
            {
                Player player = players[index];
                player.StartTurn();
                Debug.Log("Player " + index + "'s turn. --------------------------------------------");
                yield return new WaitUntil(() => player.TurnComplete);
            }
            turnMana++;

        }
    }

    private void Update()
    {
        foreach (var player in players)
        {
            if (!player.KingAlive)
            {
                winCondition = true;
                winningId = GetOtherPlayer(player.PlayerId).PlayerId;
                
                m_gameOverUI.SetActive(true);
                m_winningPlayerText.SetText($"Player {winningId} wins!");
            }
        }
    }

    private const int infiniteManaValue = 999999;
    
    public void InfinitePlayerMana()
    {
        players[0].PlayerMana = infiniteManaValue;
        players[0].PlayerMaxMana = infiniteManaValue;
    }
    
    public void InfiniteManaForAll()
    {
        turnMana = infiniteManaValue;
        players[0].PlayerMana = infiniteManaValue;
        players[0].PlayerMaxMana = infiniteManaValue; 
        players[1].PlayerMana = infiniteManaValue;
        players[1].PlayerMaxMana = infiniteManaValue;
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

