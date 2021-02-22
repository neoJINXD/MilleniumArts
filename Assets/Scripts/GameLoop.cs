using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameLoop
{
    private GameLoop()
    {}

    public static GameLoop Instance
    {
        get { return instance ?? (instance = new GameLoop()); }
    }
    
    private static GameLoop instance;

    private bool isPlaying;
    private List<Player> players = new List<Player>();

    public async void Play()
    {
        foreach (var player in players)
        {
            player.StartTurn();
            await player.AwaitTurn();
        }
    }

    public void AddPlayer(Player toAdd)
    {
        players.Add(toAdd);
    }
}
