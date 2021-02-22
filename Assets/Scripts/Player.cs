using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    private bool turnComplete;

    public Player()
    {
        turnComplete = true;
    }

    public void StartTurn()
    {
        turnComplete = false;
    }

    public void EndTurn()
    {
        turnComplete = true;
    }
    
    public bool TurnComplete
    {
        get => turnComplete;
    }
    
}

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Player player = (Player)target;
        if(!player.TurnComplete && GUILayout.Button("End Turn"))
        {
            player.EndTurn();
        }
    }
}
